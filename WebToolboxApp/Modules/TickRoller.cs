using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Diagnostics;

namespace WebToolboxApp.Modules
{
    public class TickRoller : IRegisteredObject
    {
        private System.Threading.Timer _timer;

        public TickRoller()
        {
            TimeSpan span = TimeSpan.Parse("00:00:10");
            HostingEnvironment.RegisterObject(this);
            _timer = new System.Threading.Timer(OnTimerElapsed);
            _timer.Change(TimeSpan.Zero, span);
        }

        private static void OnTimerElapsed(object sender)
        {
            foreach (TraceListener listener in System.Diagnostics.Trace.Listeners)
            {
                try
                {
                    var l = listener as IRollerListener;
                    if (l != null)
                    {
                        l.RollTick();
                    }
                }
                catch
                {
                    // 無視する.
                }
            }
        }

        public void Stop(bool immediate)
        {
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
            HostingEnvironment.UnregisterObject(this);
        }
    }
}