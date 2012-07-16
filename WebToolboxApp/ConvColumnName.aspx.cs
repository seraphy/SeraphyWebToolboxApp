using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace WebToolboxApp
{
    public partial class ConvColumnName : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void BtnClear_Click(object sender, EventArgs e)
        {
            TxtConvColumns.Text = "";
            TxtDBColumns.Text = "";
            TxtConvColumns.Visible = false;
        }

        protected void BtnConvert_Click(object sender, EventArgs e)
        {
            TxtConvColumns.Text = convertName(TxtDBColumns.Text, InitialMode.Checked);
            TxtConvColumns.Visible = true;
        }

        private string convertName(string dbName, bool initialMode = false)
        {
            var buf = new StringBuilder();
            if (!string.IsNullOrEmpty(dbName))
            {
                foreach (string line in StringUtils.ConvertRows(dbName))
                {
                    bool mode = initialMode;
                    foreach (char c in line.ToLower())
                    {
                        if (c == '_')
                        {
                            mode = false;
                        }
                        else if (Char.IsWhiteSpace(c))
                        {
                            mode = initialMode;
                            buf.Append(c);
                        }
                        else
                        {
                            if (!mode)
                            {
                                buf.Append(c.ToString().ToUpper());
                                mode = true;
                            }
                            else
                            {
                                buf.Append(c);
                            }
                        }
                    }
                    buf.Append("\r\n");
                }
            }
            return buf.ToString();
        }
    }
}