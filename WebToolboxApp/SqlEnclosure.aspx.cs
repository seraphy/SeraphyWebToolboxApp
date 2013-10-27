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
                // プロファイルから前回設定値の読み込み (新規の場合はデフォルト値)
                var profile = Context.Profile;
                var profileGroup = profile.GetProfileGroup("SqlEnclosure");
                var pattern = (string) profileGroup.GetPropertyValue("Pattern");
                var replaceChars = (string)profileGroup.GetPropertyValue("ReplaceChars");

                TxtReplaceChars.Text = replaceChars;
                TxtPattern.Text = pattern;
            }
        }

        protected void BtnClear_Click(object sender, EventArgs e)
        {
            TxtSource.Text = "";
        }

        /// <summary>
        /// プロファイルへの設定値の保存
        /// </summary>
        /// <param name="replacePattern"></param>
        /// <param name="replacechars"></param>
        private void saveProfile(string replacePattern, string replacechars)
        {
            // プロファイルから前回設定値の保存
            var profile = Context.Profile;
            var profileGroup = profile.GetProfileGroup("SqlEnclosure");
            profileGroup.SetPropertyValue("Pattern", replacePattern);
            profileGroup.SetPropertyValue("ReplaceChars", replacechars);
        }

        protected void BtnConvert_Click(object sender, EventArgs e)
        {
            string replacePattern = TxtPattern.Text;
            string replacechars = TxtReplaceChars.Text;

            // プロファイルから前回設定値の保存
            saveProfile(replacePattern, replacechars);
 
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

            // プロファイルから前回設定値の保存
            saveProfile(replacePattern, replacechars);

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