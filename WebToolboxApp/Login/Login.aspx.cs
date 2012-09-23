using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Web.Security;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace WebToolboxApp.Login
{
    /// <summary>
    /// ログインページ
    /// </summary>
    public partial class Login : System.Web.UI.Page
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private TraceSource AppLog = new TraceSource("AppLog");

        /// <summary>
        /// セッションにログインパスワードのソルトを保存するキー名
        /// </summary>
        private const string PASSWORD_SALT = "PASSWORD_SALT";

        /// <summary>
        /// 認証に失敗したIP情報
        /// </summary>
        private class RejectIpInfo
        {
            /// <summary>
            /// 失敗した回数
            /// </summary>
            private volatile int errCount;

            /// <summary>
            /// 最後に失敗した日時.
            /// まだ失敗していなければnull
            /// </summary>
            private DateTime? lastError;

            /// <summary>
            /// IPアドレス
            /// </summary>
            public string Ip { set; get; }

            /// <summary>
            /// エラー回数
            /// </summary>
            public int ErrorCount
            {
                get
                {
                    return errCount;
                }
                set
                {
                    errCount = value;
                }
            }

            /// <summary>
            /// エラー回数を増やし、且つ、
            /// 現在日時を最後のエラー時刻に設定する.
            /// </summary>
            public void AddErrorCount()
            {
                lock (this)
                {
                    lastError = DateTime.Now;
                    errCount++;
                }
            }

            /// <summary>
            /// 指定した日時を期限切れ日時とし、
            /// 期限切れのエラーをリセットする.
            /// </summary>
            /// <param name="dt">期限切れになる日時、
            /// 一般的には現在時刻から数分前を指定する.</param>
            /// <returns></returns>
            public bool ClearErrorCountIfExpired(DateTime dt)
            {
                lock (this)
                {
                    if (errCount > 0)
                    {
                        if (lastError < dt)
                        {
                            errCount = 0;
                            lastError = null;
                            return true;
                        }
                    }
                }
                return false;
            }

            /// <summary>
            /// 診断用
            /// </summary>
            /// <returns>診断用文字列</returns>
            public override string ToString()
            {
                var buf = new StringBuilder();
                buf.Append(Ip);
                buf.Append("=");
                buf.Append(errCount);
                buf.Append("(").Append(lastError).Append(")");
                return buf.ToString();
            }
        }


        /// <summary>
        /// IPアドレスをキーとし、エラー情報を値とするマップ
        /// </summary>
        private static Dictionary<string, RejectIpInfo> RejectIpInfoMap
            = new Dictionary<string, RejectIpInfo>();

        /// <summary>
        /// エラーを許容する回数
        /// </summary>
        public const int MAX_RETRY = 3;

        /// <summary>
        /// エラーをリセットする分数
        /// </summary>
        public const int RESET_TIME = 1;


        /// <summary>
        /// ログインページの表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // リクエスト元IPアドレス
            string ip = Request.ServerVariables["REMOTE_ADDR"];

            // 拒否IPを復帰させる
            DateTime expired = DateTime.Now - TimeSpan.FromMinutes(RESET_TIME);
            lock (RejectIpInfoMap)
            {
                var resetIps = new List<string>();
                foreach (KeyValuePair<string, RejectIpInfo> entry in RejectIpInfoMap)
                {
                    if (entry.Value.ClearErrorCountIfExpired(expired))
                    {
                        resetIps.Add(entry.Key);
                    }
                }
                foreach (string resetIp in resetIps)
                {
                    RejectIpInfoMap.Remove(resetIp);
                }

                RejectIpInfo info;
                if (RejectIpInfoMap.TryGetValue(ip, out info))
                {
                    if (info.ErrorCount >= MAX_RETRY)
                    {
                        Response.StatusCode = 401; // 権限なし
                        Response.End();
                        return;
                    }
                }
            }

            if (!IsPostBack)
            {
                // 初回表示の場合
                // パスワードをハッシュ化するときに使用するソルトを生成する.
                RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
                var salt = new byte[18];
                rngCsp.GetBytes(salt);
                string strSalt = Convert.ToBase64String(salt);
                SALT.Value = strSalt;

                Session[PASSWORD_SALT] = strSalt;
            }
            else
            {
                // フォームからポストバックされた場合、
                // パスワードを検証する.
                string strHash = HashedPassword.Value;
                byte[] hash = Convert.FromBase64String(strHash);

                string strSalt = (Session[PASSWORD_SALT] as string) ?? "";
                if (strSalt != SALT.Value)
                {
                    Response.StatusCode = 403; // Forbidden
                    Response.End();
                    return;
                }

                var userName = UserName.Text;
                if ("ADMIN" == userName)
                {
                    SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                    string password = "hello";
                    byte[] passwordWithSalt = Encoding.UTF8.GetBytes(strSalt + "@" + password);
                    byte[] result = sha1.ComputeHash(passwordWithSalt);

                    bool matched = result.SequenceEqual(hash);
                    if (matched)
                    {
                        AppLog.TraceEvent(TraceEventType.Information, 100, "認可されました。");
                        // 認証OK
                        //FormsAuthentication.SetAuthCookie(userName, false);
                        //Response.Redirect("~/Admin/Default.aspx", true);
                        FormsAuthentication.RedirectFromLoginPage(userName, false);
                        return;
                    }
                }

                lock (RejectIpInfoMap)
                {
                    RejectIpInfo info;
                    if (!RejectIpInfoMap.TryGetValue(ip, out info))
                    {
                        info = new RejectIpInfo();
                        info.Ip = ip;
                        RejectIpInfoMap.Add(ip, info);
                    }
                    info.AddErrorCount();
                    AppLog.TraceEvent(TraceEventType.Information, 300, "ログインに失敗しました。" + info);
                }
            }
        }
    }
}