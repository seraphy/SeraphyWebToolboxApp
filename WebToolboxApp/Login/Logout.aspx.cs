using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebToolboxApp.Login
{
    public partial class Logout : System.Web.UI.Page
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private TraceSource AppLog = new TraceSource("AppLog");

        protected void Page_Load(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Response.Redirect("~/Login/Login.aspx");
        }
    }
}