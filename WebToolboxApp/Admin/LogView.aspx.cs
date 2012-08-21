using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebToolboxApp.Modules;
using System.Web.Hosting;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace WebToolboxApp.Admin
{
    /// <summary>
    /// ログファイルの表示・ダウンロード・一括削除ページ
    /// </summary>
    public partial class LogView : System.Web.UI.Page
    {
        /// <summary>
        /// 設定ファイルにより出力レベルを調整可能なトレースログ
        /// </summary>
        private static readonly TraceSource AppLog = new TraceSource("AppLog");

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                initFileList();
            }
        }

        /// <summary>
        /// ログフォルダを取得する.
        /// 該当がなければnull
        /// </summary>
        /// <returns></returns>
        private string GetLogDirectory()
        {
            foreach (System.Diagnostics.TraceListener l in System.Diagnostics.Trace.Listeners)
            {
                var fileLogger = l as FileLogger;
                if (fileLogger != null)
                {
                    string path = HostingEnvironment.MapPath(fileLogger.LogDirectory);
                    return Path.GetDirectoryName(path);
                }
            }
            return null;
        }

        /// <summary>
        /// ログフォルダ上のファイル一覧
        /// </summary>
        private void initFileList()
        {
            try
            {
                string logDir = GetLogDirectory();
                if (!string.IsNullOrEmpty(logDir))
                {
                    var dirInfo = new DirectoryInfo(logDir);
                    FileInfo[] fileInfos = dirInfo.GetFiles();
                    Array.Sort(fileInfos, (a, b) => (b.LastWriteTime - a.LastWriteTime).Milliseconds);
                    FileListView.DataSource = fileInfos;
                    FileListView.DataBind();
                }
            }
            catch (Exception ex)
            {
                AppLog.TraceEvent(TraceEventType.Warning, 400, "ログ一覧取得失敗: " + ex);
                FileListView.DataSource = new FileInfo[0];
                FileListView.DataBind();
            }
        }

        /// <summary>
        /// リストビューのアイテム中からのコマンドを受け取る
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FileListView_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                DoDownload((e.CommandArgument as string) ?? "");
            }
        }

        /// <summary>
        /// ログファイルをダウンロードする.
        /// </summary>
        /// <param name="fileName"></param>
        private void DoDownload(string fileName)
        {
            string logDir = GetLogDirectory();
            if (string.IsNullOrEmpty(logDir))
            {
                return;
            }

            fileName = fileName.Replace("/", "_").Replace("\\", "_").Replace(":", "_");
            string path = Path.Combine(logDir, fileName);
            if (File.Exists(path))
            {
                var fileInfo = new FileInfo(path);
                long contentSize = fileInfo.Length;

                using (var inpStm = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    Response.BufferOutput = false;
                    Response.ContentType = "application/octet-stream";

                    // IEではファイル名に英数字以外を使う場合はSJISで想定されるため、ヘッダの文字コードをSJISに設定
                    // http://support.microsoft.com/kb/436616/ja
                    Response.HeaderEncoding = Encoding.GetEncoding("Shift_JIS");
                    Response.AddHeader("Content-Length", contentSize.ToString());
                    Response.AddHeader("Content-Disposition",
                        "attachment; filename=" + fileName + ";size=" + contentSize);

                    // [MS] Content-Disposition: attachemnt と Cache-Control: no-cache によるダウンロードの問題
                    // http://support.microsoft.com/?scid=kb;ja;436605&spid=2073&sid=204
                    // http://support.microsoft.com/kb/939251/ja
                    Response.CacheControl = "public";

                    Response.Flush();

                    // 内容の転送処理
                    long actualSize = 0;
                    using (var outStm = Response.OutputStream)
                    {
                        var buf = new byte[4096];
                        int rd;
                        do
                        {
                            rd = inpStm.Read(buf, 0, buf.Length);
                            actualSize += rd;
                            if (rd > 0)
                            {
                                outStm.Write(buf, 0, rd);
                                outStm.Flush();
                            }
                        } while (rd > 0);
                    }

                    AppLog.TraceEvent(TraceEventType.Verbose, 200,
                        "fileSize=" + contentSize + "/actual=" + actualSize);

                    // 後続の処理を確実に終了するにはレガシーな、この方法が最も手軽で安全です.
                    Response.End();
                }
            }
        }

        /// <summary>
        /// 削除可能なログファイルをすべて削除する.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PurgeButton_Command(object sender, CommandEventArgs e)
        {
            string logDir = GetLogDirectory();
            if (!string.IsNullOrEmpty(logDir))
            {
                var dirInfo = new DirectoryInfo(logDir);
                FileInfo[] fileInfos = dirInfo.GetFiles();
                foreach (FileInfo fileInfo in fileInfos)
                {
                    try
                    {
                        fileInfo.Delete();
                    }
                    catch (Exception ex)
                    {
                        AppLog.TraceEvent(TraceEventType.Warning, 400, "ログ削除に失敗: " + ex);
                    }
                }
            }
            initFileList();
        }
    }
}