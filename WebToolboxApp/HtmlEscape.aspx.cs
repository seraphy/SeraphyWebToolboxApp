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
            if (!IsPostBack)
            {
                TxtTabSize.Text = "4";
            }
        }

        protected void BtnClear_Click(object sender, EventArgs e)
        {
            TxtResult.Text = "";
            TxtSource.Text = "";
        }

        /// <summary>
        /// 変換する.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnCalcurate_Click(object sender, EventArgs e)
        {
            // タブサイズ
            int tabsize = 4;
            int.TryParse(TxtTabSize.Text, out tabsize);

            // 変換モード
            string mode = DrpConvertMode.SelectedValue;

            // 対象テキスト
            string src = TxtSource.Text;

            // タブ展開
            string dest = src;
            dest = expandTab(dest, tabsize);
            dest = escapeHTML(dest, mode);

            TxtResult.Text = dest;
        }

        /// <summary>
        /// HTML, XMLのエスケープを行う.
        /// </summary>
        /// <param name="src">対象文字列</param>
        /// <param name="mode">変換モード</param>
        /// <returns></returns>
        protected string escapeHTML(string src, string mode)
        {
            string dest = src;

            // 変換
            if (mode == "Full" || mode == "XML")
            {
                // フルセット
                dest = dest.Replace("&", "&amp;");
                dest = dest.Replace("<", "&gt;");
                dest = dest.Replace(">", "&lt;");
                dest = dest.Replace("\"", "&quot;");
            }
            else if (mode == "Simple")
            {
                // 最低限
                dest = dest.Replace("&", "&amp;");
                dest = dest.Replace("<", "&gt;");
            }
            else if (mode == "None")
            {
                // なにもしない
            }
            else
            {
                throw new ArgumentException("不明なモード: " + mode);
            }

            if (mode == "XML")
            {
                // XML(aposも含む)
                dest = dest.Replace("'", "&apos;");
            }
            return dest;
        }

        /// <summary>
        /// タブを展開する.
        /// </summary>
        /// <param name="text">展開するテキスト</param>
        /// <param name="tabsize">タブ幅</param>
        /// <returns></returns>
        protected string expandTab(string text, int tabsize)
        {
            if (tabsize < 1)
            {
                // タブサイズが1以下ならば変換しない.
                return text;
            }

            var buf = new System.Text.StringBuilder();

            int col = 0;

            int len = text.Length;
            for (int idx = 0; idx < len; idx++)
            {
                char c = text[idx];
                if (c == '\r' || c == '\n')
                {
                    col = 0;
                    buf.Append(c);
                }
                else if (c == '\t')
                {
                    int nextcol = (((col + 1) / tabsize) + 1) * tabsize;
                    for (; col < nextcol; col++)
                    {
                        buf.Append(' ');
                    }
                }
                else
                {
                    int charsiz = System.Text.Encoding.UTF8.GetByteCount(new char[] { c });
                    col += (charsiz > 1) ? 2 : 1; // ASCII以外は2バイトとみなす. (半角カナは無視)
                    buf.Append(c);
                }
            }

            return buf.ToString();
        }
    }
}