using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading;
using System.Configuration;
using System.Web.Configuration;
using System.Web.Hosting;

namespace WebToolboxApp.Modules
{
    /// <summary>
    /// ファイルにログを記録するトレースリスナ.
    /// appSettingsの「ExpireLogDays」でログファイルの有効期限を指定できる.
    /// 新しいログファイルを作成する時点で古いログファイルは有効期限切れのものを物理削除する.
    /// </summary>
    /// <summary>
    /// ファイルにログを記録するトレースリスナ.
    /// appSettingsの「ExpireLogDays」でログファイルの有効期限を指定できる.
    /// 新しいログファイルを作成する時点で古いログファイルは有効期限切れのものを物理削除する.
    /// </summary>
    public class FileLogger : System.Diagnostics.TraceListener, IRollerListener
    {
        /// <summary>
        /// デフォルトの有効期限日は15日
        /// </summary>
        internal const int DEFAULT_EXPIRE_LOG_DAYS = 15;

        /// <summary>
        /// デフォルトのログファイル名
        /// </summary>
        internal readonly string DEFAULT_LOG_NAME = "application.log";

        /// <summary>
        /// 排他制御用
        /// </summary>
        private readonly object lockObj = new object();

        /// <summary>
        /// 設定上のアプリケーション相対のログファイル名
        /// </summary>
        private string _baseName;

        /// <summary>
        /// 実際のログファイルの物理位置
        /// </summary>
        private string _realPath;

        /// <summary>
        /// ログファイルを作成した日
        /// </summary>
        private int _currentLogDay;

        /// <summary>
        /// ログ出力先、閉じていればnull
        /// </summary>
        private System.IO.TextWriter _wr;

        /// <summary>
        /// 最後に書き込んだ時刻
        /// </summary>
        private DateTime _lastWrite = DateTime.Now;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="path">ログファイルの出力先ディレクトリとファイル名を示すひな型、
        /// 実際のファイル名は日付が付与される</param>
        public FileLogger(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = DEFAULT_LOG_NAME;
            }
            this._baseName = path;
        }

        /// <summary>
        /// ログ出力先の相対ディレクトリを示す.
        /// 実パスにするためにはHostingEnvironment.MapPathを使用すること.
        /// </summary>
        public string LogDirectory
        {
            get
            {
                return _baseName;
            }
        }

        /// <summary>
        /// ログファイルを現在日時で作成する.
        /// すでに開かれている場合はファイルは一旦閉じられる.
        /// ファイルは読み書き共有モードで開かれる.
        /// </summary>
        public void renew()
        {
            // 現在のものを閉じる.
            lock (lockObj)
            {
                if (_wr != null)
                {
                    try
                    {
                        _wr.WriteLine("[CONTINUE]");
                        _wr.Close();
                    }
                    catch (Exception)
                    {
                        // 無視する.
                    }
                    _wr = null;
                }

                try
                {
                    // 現在日を取得
                    var now = DateTime.Now;

                    // ファイル名のテンプレートを分解し
                    // 日付を適用する.
                    string dirName = Path.GetDirectoryName(_baseName);
                    string extName = Path.GetExtension(_baseName);
                    string baseName = Path.GetFileNameWithoutExtension(_baseName);

                    string fname = baseName + "-" + now.ToString("yyyy-MM-dd") + extName;
                    string path = Path.Combine(dirName, fname);

                    // 現在のコンテキストを得る.

                    // ウェブアプリケーション相対パスを解釈し実パスを得る
                    string realPath = HostingEnvironment.MapPath(path);

                    // 実パスが、まだ実在しなければ作成する
                    string realDir = Path.GetDirectoryName(realPath);
                    if (!Directory.Exists(realDir))
                    {
                        Directory.CreateDirectory(realDir);
                    }
                    else
                    {
                        // 古いログを削除する.
                        purgeLogs(realDir, baseName, extName);
                    }

                    // ファイルを作成(追記)する.
                    var file = File.Open(realPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    _wr = new StreamWriter(file);

                    // ファイルパス、作成日時を記憶する.
                    _currentLogDay = now.Day;
                    _realPath = realPath;
                }
                catch (Exception ex)
                {
                    Fail("ApplicationLog configuration error: " + _baseName, ex.ToString());
                    _wr = null;
                }
            }
        }

        /// <summary>
        /// 古いログファイルを削除する.
        /// 指定したディレクトリ上にある、ファイル名の先頭と末尾が合致するファイルで、
        /// 設定ファイルのExpireLogDaysで指定された日数以前のファイルを物理削除する.
        /// </summary>
        /// <param name="realDir">ディレクトリ</param>
        /// <param name="baseName">ファイル名の頭</param>
        /// <param name="extName">ファイル名の末尾</param>
        private void purgeLogs(string realDir, string baseName, string extName)
        {
            // 古いログファイルを削除する.
            int exireLogDays = DEFAULT_EXPIRE_LOG_DAYS;
            try
            {
                string strExpreLogDays = WebConfigurationManager.AppSettings["ExpireLogDays"];
                int.TryParse(strExpreLogDays, out exireLogDays);
            }
            catch (Exception)
            {
                // 準備できなくても継続する。
            }

            // 有効期限日
            DateTime limit = DateTime.Now.AddDays(-exireLogDays);

            foreach (string oldPath in Directory.GetFiles(realDir))
            {
                string oldName = Path.GetFileName(oldPath);
                if (oldName.StartsWith(baseName) && oldName.EndsWith(extName))
                {
                    // 先頭と末尾がログファイル名のパターンと一致する
                    DateTime lastModified = File.GetLastWriteTime(oldPath);
                    if (lastModified < limit)
                    {
                        // ログファイルの最終更新日時が有効期限日よりも以前であれば削除する.
                        // (ログファイルを書き終わった時点からの計算となる.)
                        try
                        {
                            File.Delete(oldPath);
                        }
                        catch
                        {
                            // 削除できなくても継続する。
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ログファイルのローテートの判定と実施。
        /// 日付が変わった場合、現在のログファイルを閉じて新しい日付でログファイルを再開する。
        /// </summary>
        private void checkedRolling()
        {
            lock (lockObj)
            {
                if (_wr != null)
                {
                    var now = DateTime.Now;
                    if (_currentLogDay != now.Day)
                    {
                        renew();
                    }
                }
            }
        }

        /// <summary>
        /// 定期的にログローテートの確認と、ログファイルの解放を行う.
        /// </summary>
        public void RollTick()
        {
            lock (lockObj)
            {
                if (_wr != null)
                {
                    // フラッシュ
                    _wr.Flush();

                    // ファイルを開いておく維持時間
                    TimeSpan keepOpenTime = TimeSpan.Parse(
                        WebConfigurationManager.AppSettings["KeepLogOpenTime"]);

                    // 最後の書き込み以降、一定時間経過していたら
                    // ファイルを閉じる
                    TimeSpan span = DateTime.Now - _lastWrite;
                    if (span > keepOpenTime)
                    {
                        // ログファイルのクローズ
                        Close();
                    }
                    else
                    {
                        // ローテートの確認
                        checkedRolling();
                    }
                }
            }
        }

        protected void openEnsure()
        {
            lock (lockObj)
            {
                // ログファイルのオープン
                if (_wr == null)
                {
                    renew();
                }
                else
                {
                    // ログファイル名のローテート判定
                    checkedRolling();
                }
            }
        }

        /// <summary>
        /// 文字列の書き込み.
        /// 日付が変わっている場合はログはローテートされる.
        /// </summary>
        /// <param name="o">文字列</param>
        public override void Write(string o)
        {
            try
            {
                lock (lockObj)
                {
                    // ログファイルを開く
                    openEnsure();

                    if (_wr != null)
                    {
                        _wr.Write(o);
                        _lastWrite = DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                Fail("ApplicationLog write error: " + _realPath, ex.ToString());
            }
        }

        /// <summary>
        /// 日付等情報つきの文字列の書き込み, 改行あり.
        /// 日付が変わっている場合はログはローテートされる.
        /// </summary>
        /// <param name="o">文字列</param>
        public override void WriteLine(string o)
        {
            try
            {
                // 書式化されたメッセージヘッダを作成する
                var buf = MakeMessageHead();

                // 本文
                buf.Append(o);

                // 出力
                lock (lockObj)
                {
                    // ログファイルを開く
                    openEnsure();

                    if (_wr != null)
                    {
                        _wr.WriteLine(buf.ToString());
                        _lastWrite = DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                Fail("ApplicationLog write error: " + _realPath, ex.ToString());
            }
        }

        /// <summary>
        /// メッセージヘッダを作成する.
        /// 日付、タイムスタンプ、スレッド、プロセス、リモートIP、ログインIDの文字列を返す.
        /// </summary>
        /// <returns>メッセージヘッダの文字列バッファ</returns>
        protected StringBuilder MakeMessageHead()
        {
            var buf = new StringBuilder();

            // 日付
            if (TraceOutputOptions.HasFlag(TraceOptions.DateTime))
            {
                buf.Append(DateTime.Now.ToString());
                buf.Append(" # ");
            }
            // タイムスタンプ
            if (TraceOutputOptions.HasFlag(TraceOptions.Timestamp))
            {
                buf.Append("(");
                buf.Append(System.Diagnostics.Stopwatch.GetTimestamp());
                buf.Append(") ");
            }
            // プロセスID
            if (TraceOutputOptions.HasFlag(TraceOptions.ThreadId))
            {
                buf.Append("P");
                buf.Append(Process.GetCurrentProcess().Id);
                buf.Append(" # ");
            }
            // スレッドID
            if (TraceOutputOptions.HasFlag(TraceOptions.ThreadId))
            {
                buf.Append("T");
                buf.Append(Thread.CurrentThread.ManagedThreadId);
                buf.Append(" # ");
            }

            // コンテキストに関連する情報を取得する.
            // リクエストと直接関連ない初期化等の呼び出しでは
            // コンテキストのアクセスは無効となり例外が発生するためtryで囲む.
            string remoteAddr = "not-related";
            String userName = "";
            string handlerName = "";
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                try
                {
                    // リモートIPの表示
                    // アクセスしてきたクライアントのIPアドレスを記録する.
                    // PROXY経由の場合、そちらを優先する.
                    // CLIENT_IP, X_FORWARDED_FOR, X_HOSTのすべてのリクエストヘッダを確認する.
                    // http://en.wikipedia.org/wiki/X-Forwarded-For
                    var proxyRemoteAddresses = new string[]
                    {
                        context.Request.ServerVariables["REMOTE_ADDR"], // 直IP
                        context.Request.ServerVariables["HTTP_CLIENT_IP"], // 一部のProxyサーバ
                        context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"], // Apache2等、通常
                        context.Request.ServerVariables["HTTP_X_HOST"], // Apache1.3系(旧)
                    };

                    // CLIENT_IP, X_FORWARDED_FOR, X_HOSTを結合
                    remoteAddr = string.Join(", ",
                        proxyRemoteAddresses.Where(
                            addr => !string.IsNullOrWhiteSpace(addr)
                        ).ToArray());

                     // ログインユーザ名の表示
                    var user = context.User;
                    if (user != null)
                    {
                        var identity = user.Identity;
                        if (identity != null)
                        {
                            userName = identity.Name;
                        }
                    }

                    // 画面ハンドラの表示 (ハンドラがなければ空)
                    if (context != null && context.Handler != null)
                    {
                        Type typ = context.Handler.GetType();
                        handlerName = typ.Name;
                    }
                }
                catch
                {
                    // リクエストと関連していない.
                }
            }

            buf.Append("IP:");
            buf.Append(remoteAddr);
            buf.Append(" # ");

            buf.Append("User:");
            buf.Append(userName);
            buf.Append(" # ");

            buf.Append("page:");
            buf.Append(handlerName);
            buf.Append(" # ");

            return buf;
        }

        /// <summary>
        /// フラッシュする.
        /// </summary>
        public override void Flush()
        {
            try
            {
                lock (lockObj)
                {
                    if (_wr != null)
                    {
                        _wr.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                Fail("ApplicationLog write error: " + _realPath, ex.ToString());
            }
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        public override void Close()
        {
            lock (lockObj)
            {
                try
                {
                    if (_wr != null)
                    {
                        _wr.Flush();
                        _wr.Close();
                    }
                }
                catch (Exception)
                {
                    // クローズの失敗は復旧しようもないので
                    // 無視する.
                }
                _wr = null;
            }
        }
    }
}