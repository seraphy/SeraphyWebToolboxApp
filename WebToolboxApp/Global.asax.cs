using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Security.Principal;
using System.Threading;
using System.Diagnostics;
using WebToolboxApp.Modules;

namespace WebToolboxApp
{
    public class Global : System.Web.HttpApplication
    {
        private FileLoggerRoller fileLoggerRoller;

        void Application_Start(object sender, EventArgs e)
        {
            // アプリケーションのスタートアップで実行するコードです
            fileLoggerRoller = new FileLoggerRoller();
            System.Diagnostics.Trace.WriteLine("Application_Start");
        }

        void Application_End(object sender, EventArgs e)
        {
            //  アプリケーションのシャットダウンで実行するコードです
            System.Diagnostics.Trace.WriteLine("Application_End");
        }

        /// <summary>
        /// エラー発生時,
        /// ただしWebMethodでの例外は、ここではハンドルされない.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Application_Error(object sender, EventArgs e)
        {
            // ハンドルされていないエラーが発生したときに実行するコードです

            Exception objErr = Server.GetLastError();
            if (objErr != null)
            {
                // エラーのロギング
                try
                {
                    // ロガー
                    var AppLog = new TraceSource("AppLog");

                    // リクエストURI
                    var uri = HttpContext.Current.Request.Url;
                    if (uri != null)
                    {
                        AppLog.TraceEvent(TraceEventType.Critical, 600,
                            "error location=" + uri.ToString());
                    }

                    // すべてのハンドルされていない例外をログに記録する
                    int nest = 0;
                    for (; objErr != null; objErr = objErr.InnerException)
                    {
                        AppLog.TraceEvent(TraceEventType.Critical, 600,
                            string.Format("Global#Application_Error[{0}]: {1}", nest, objErr.ToString()));
                        nest++;
                    }
                }
                catch (Exception logException)
                {
                    // ロギングに失敗した場合はトレースログに送ってみる
                    System.Diagnostics.Trace.WriteLine("Global#EventLogError: " + logException);
                }
            }
        }

        void Session_Start(object sender, EventArgs e)
        {
            // 新規セッションを開始したときに実行するコードです

        }

        void Session_End(object sender, EventArgs e)
        {
            // セッションが終了したときに実行するコードです 
            // メモ: Web.config ファイル内で sessionstate モードが InProc に設定されているときのみ、
            // Session_End イベントが発生します。 session モードが StateServer か、または 
            // SQLServer に設定されている場合、イベントは発生しません。

        }

        void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            // 現在ログインしているユーザIDを取得
            if (Context.Request.IsAuthenticated)
            {
                var identity = Context.User.Identity;
                var name = identity.Name ?? "";
                System.Diagnostics.Debug.Write("loginId=" + name);
                if (name == "ADMIN")
                {
                    var roles = new String[] { "ADMIN" };
                    Context.User = new GenericPrincipal(identity, roles);
                    Thread.CurrentPrincipal = Context.User;
                }
            }
        }
    }
}
