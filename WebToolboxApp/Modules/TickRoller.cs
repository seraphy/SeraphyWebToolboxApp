using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Diagnostics;
using System.Web.Configuration;

namespace WebToolboxApp.Modules
{
    /// <summary>
    /// 定期的にトレースリスナに通知を送るローラー.
    /// ホストが登録解除されるときにドメインホストから
    /// クローズの通知をうけ終了することができるようにしている.
    /// </summary>
    public class TickRoller : IRegisteredObject
    {
        /// <summary>
        /// 通知間隔のタイマー
        /// </summary>
        private System.Threading.Timer _timer;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TickRoller()
        {
            string tickRollingSpan = WebConfigurationManager.AppSettings["TickRollingSpan"];
            TimeSpan span;
            if (!TimeSpan.TryParse(tickRollingSpan, out span))
            {
                span = TimeSpan.FromSeconds(10);
            }

            HostingEnvironment.RegisterObject(this);
            _timer = new System.Threading.Timer(OnTimerElapsed);
            _timer.Change(TimeSpan.Zero, span);
        }

        /// <summary>
        /// タイマーイベントのハンドラ
        /// </summary>
        /// <param name="sender"></param>
        private static void OnTimerElapsed(object sender)
        {
            foreach (TraceListener listener in System.Diagnostics.Trace.Listeners)
            {
                try
                {
                    var l = listener as IRollerListener;
                    if (l != null)
                    {
                        // トレースリスナーが通知を受け取られる場合は
                        // 通知を渡す.
                        l.RollTick();
                    }
                }
                catch
                {
                    // 無視する.
                }
            }
        }

        /// <summary>
        /// アプリケーションドメインホストから停止を通知される.
        /// </summary>
        /// <param name="immediate"></param>
        public void Stop(bool immediate)
        {
            // タイマーを解除する.
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }

            // すべてのトレースリスナをクローズする.
            foreach (TraceListener listener in System.Diagnostics.Trace.Listeners)
            {
                try
                {
                    listener.Close();
                }
                catch
                {
                    // 無視する.
                }
            }

            // オブジェクトの登録を解除する.
            HostingEnvironment.UnregisterObject(this);
        }
    }
}