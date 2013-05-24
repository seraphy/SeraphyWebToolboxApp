using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebToolboxApp
{
    public partial class SqlEnclosure : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                TxtReplaceChars.Text = "@@";
                TxtPattern.Text = "strSQL = strSQL & \"@@\" & vbCrLf";
            }
        }

        protected void BtnClear_Click(object sender, EventArgs e)
        {
            TxtSource.Text = "";
        }

        protected void BtnConvert_Click(object sender, EventArgs e)
        {
            string replacePattern = TxtPattern.Text;
            string replacechars = TxtReplaceChars.Text;

            string input = TxtSource.Text;
            var buf = new StringBuilder();

            string[] lines = input.Split(new string[] { "\n" }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                String trimedLine = line.TrimEnd(new char[] { '\r', '\n' });
                string conv = replacePattern.Replace(replacechars, trimedLine);
                buf.AppendLine(conv);
            }

            TxtResult.Text = buf.ToString();
        }

        protected void BtnDeconvert_Click(object sender, EventArgs e)
        {
            string replacePattern = TxtPattern.Text;
            string replacechars = TxtReplaceChars.Text;

            string[] patterns = replacePattern.Split(
                new string[] { replacechars }, StringSplitOptions.None);

            string input = TxtResult.Text;
            var buf = new StringBuilder();

            string[] lines = input.Split(new string[] { "\n" }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                String conv = line.TrimEnd(new char[] { '\r', '\n' });

                if (patterns.Length >= 1 && conv.StartsWith(patterns[0]))
                {
                    conv = conv.Substring(patterns[0].Length);
                }
                if (patterns.Length >= 2 && conv.EndsWith(patterns[1]))
                {
                    conv = conv.Substring(0, conv.Length - (patterns[1].Length));
                }

                buf.AppendLine(conv);
            }

            TxtSource.Text = buf.ToString();
        }
    }
}