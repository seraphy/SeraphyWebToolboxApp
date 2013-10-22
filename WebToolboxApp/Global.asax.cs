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
        /// <summary>
        /// ロガー
        /// </summary>
        private TraceSource AppLog = new TraceSource("AppLog");

        /// <summary>
        /// 定期ローラー
        /// </summary>
        private TickRoller fileLoggerRoller;

        /// <summary>
        /// アプリケーションの開始時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Application_Start(object sender, EventArgs e)
        {
            // アプリケーションのスタートアップで実行するコードです
            fileLoggerRoller = new TickRoller();
            System.Diagnostics.Trace.WriteLine("Application_Start");
        }

        /// <summary>
        /// アプリケーションの終了時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Application_End(object sender, EventArgs e)
        {
            //  アプリケーションのシャットダウンで実行するコードです
            System.Diagnostics.Trace.WriteLine("Application_End");
        }

        /// <summary>
        /// ASP.NET が要求に応答するときに、実行の HTTP パイプライン チェインの
        /// 最初のイベントとして発生します。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void Application_BeginRequest(object source, EventArgs e)
        {
            // リクエスト開始時のログ
            if (AppLog.Switch.ShouldTrace(TraceEventType.Start))
            {
                HttpContext context = HttpContext.Current;
                HttpRequest req = context.Request;

                AppLog.TraceEvent(TraceEventType.Start, 1001,
                    "BEGIN REQUEST: " + req.RawUrl);
            }
        }

        /// <summary>
        /// ASP.NET が要求に応答するときに、実行の HTTP パイプライン チェインの
        /// 最後のイベントとして発生します。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void Application_EndRequest(object source, EventArgs e)
        {
            // リクエスト完了時のログ
            if (AppLog.Switch.ShouldTrace(TraceEventType.Stop))
            {
                HttpContext context = HttpContext.Current;
                HttpRequest req = context.Request;

                // リクエスト開始から完了までにかかった時間をログに記録する.
                DateTime beginProcessDt = context.Timestamp;
                var now = DateTime.Now;
                TimeSpan consumeTime = now - beginProcessDt;

                AppLog.TraceEvent(TraceEventType.Stop, 1002,
                    "END REQUEST(経過時間:" + consumeTime + "):" + req.RawUrl);
            }
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
                    // リクエストURI
                    var context = HttpContext.Current;
                    if (context != null)
                    {
                        try
                        {
                            var uri = context.Request.Url;
                            if (uri != null)
                            {
                                AppLog.TraceEvent(TraceEventType.Critical, 600,
                                    "error location=" + uri.ToString());
                            }
                        }
                        catch
                        {
                            // 無視する.
                        }
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

        // ※ セキュリティプロバイダのメンバーシップとロールマネージャを使用するため、以下のコードは不要
        //void Application_PostAuthenticateRequest(object sender, EventArgs e)
        //{
        //    // 現在ログインしているユーザIDを取得
        //    if (Context.Request.IsAuthenticated)
        //    {
        //        var identity = Context.User.Identity;
        //        string[] roles = Roles.GetRolesForUser();

        //        //Context.User = new GenericPrincipal(identity, roles);
        //        //Thread.CurrentPrincipal = Context.User;
        //    }
        //}
    }
}
