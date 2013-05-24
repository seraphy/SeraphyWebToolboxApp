using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebToolboxApp.Admin
{
    public partial class GenRandom : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private byte[] GenerateRandom()
        {
            int fileSize = 1024;
            int.TryParse(TxtFileSize.Text, out fileSize);

            var buf = new byte[fileSize];

            RandomNumberGenerator rng = RNGCryptoServiceProvider.Create();
            rng.GetBytes(buf);

            return buf;
        }

        protected void BtnGenerateRaw_Click(object sender, EventArgs e)
        {
            byte[] buf = GenerateRandom();

            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-Length", buf.Length.ToString());
            Response.AppendHeader("Content-Disposition", "inline; filename=\"random.bin\"");
            Response.BinaryWrite(buf);
            Response.Flush();
            Response.End();
        }

        protected void BtnGenerate_Click(object sender, EventArgs e)
        {
            byte[] buf = GenerateRandom();

            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-Disposition", "inline; filename=\"random.txt\"");

            using (var stm = new CryptoStream(
                Response.OutputStream,
                new ToBase64Transform(),
                CryptoStreamMode.Write))
            {
                stm.Write(buf, 0, buf.Length);
                stm.Flush();
            }
            Response.End();
        }
    }
}