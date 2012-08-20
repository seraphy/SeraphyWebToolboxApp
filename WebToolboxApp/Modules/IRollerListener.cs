using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebToolboxApp.Modules
{
    /// <summary>
    /// ログファイルのローリングまたはクローズのための定期イベントのリスナ
    /// </summary>
    public interface IRollerListener
    {
        /// <summary>
        /// 定期イベントが発生した場合に呼び出される
        /// </summary>
        void RollTick();
    }
}