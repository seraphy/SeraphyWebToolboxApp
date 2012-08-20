using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Web.Security;

namespace WebToolboxApp.Login
{
    /// <summary>
    /// ログインページ
    /// </summary>
    public partial class Login : System.Web.UI.Page
    {
        private const string PASSWORD_SALT = "PASSWORD_SALT";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
                var salt = new byte[18];
                rngCsp.GetBytes(salt);
                string strSalt = Convert.ToBase64String(salt);
                SALT.Value = strSalt;

                Session[PASSWORD_SALT] = strSalt;
            }
            else
            {
                string strHash = HashedPassword.Value;
                byte[] hash = Convert.FromBase64String(strHash);

                string strSalt = (Session[PASSWORD_SALT] as string) ?? "";
                if (strSalt != SALT.Value)
                {
                    Response.StatusCode = 403; // Forbidden
                    Response.End();
                    return;
                }

                var userName = UserName.Text;
                if ("ADMIN" == userName)
                {
                    SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                    string password = "hello";
                    byte[] passwordWithSalt = Encoding.UTF8.GetBytes(strSalt + "@" + password);
                    byte[] result = sha1.ComputeHash(passwordWithSalt);

                    bool matched = result.SequenceEqual(hash);
                    if (matched)
                    {
                        // 認証OK
                        FormsAuthentication.SetAuthCookie(userName, false);
                        Response.Redirect("~/Admin/Default.aspx", true);
                        return;
                    }
                }
            }
        }
    }
}