using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebToolboxApp
{
    public partial class HtmlEscape : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TxtTabSize.Text = "4";
        }

        protected void BtnClear_Click(object sender, EventArgs e)
        {
            TxtResult.Text = "";
            TxtSource.Text = "";
        }

        protected void BtnCalcurate_Click(object sender, EventArgs e)
        {
            int tabsize = 4;
            int.TryParse(TxtTabSize.Text, out tabsize);

            string mode = DrpConvertMode.SelectedValue;

            string src = TxtSource.Text;

            // タブの単純なスペースへの置き換え、インデント考慮なし
            string tabspc = " ";
            for (int idx = 1; idx < tabsize; idx++)
            {
                tabspc += " ";
            }
            string dest = src.Replace("\t", tabspc);

            // 変換
            if (mode == "Full" || mode == "XML")
            {
                // フルセット
                dest = dest.Replace("&", "&amp;");
                dest = dest.Replace("<", "&gt;");
                dest = dest.Replace(">", "&lt;");
                dest = dest.Replace("\"", "&quot;");
            }
            else
            {
                // 最低限
                dest = dest.Replace("&", "&amp;");
                dest = dest.Replace("<", "&gt;");
            }

            if (mode == "XML")
            {
                // XML(aposも含む)
                dest = dest.Replace("'", "&apos;");
            }

            TxtResult.Text = dest;
        }
    }
}