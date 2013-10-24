using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebToolboxApp.Login
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        private const string PRIVATE_KEY  = "ChangePassword.KeyPair";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                const int keysize = 1024; // 1024bit - 88bit = 117byte (最大平文サイズ)
                // 暗号化キーを安全に二点間で交換するためのRSA暗号化キーを生成する.
                using (var rsa = new RSACryptoServiceProvider(keysize))
                {
                    try
                    {
                        // 秘密キーをセッションに保存する.
                        string keyPair = rsa.ToXmlString(true);
                        Session[PRIVATE_KEY] = keyPair;

                        // 公開キーをHiddenに設定する.
                        RSAParameters publicParam = rsa.ExportParameters(false);
                        byte[] publicModules = publicParam.Modulus;
                        byte[] publicExponent = publicParam.Exponent;

                        TxtPublicModules.Value = LoginUtils.ByteToHexBitFiddle(publicModules);
                        TxtPublicExponent.Value = LoginUtils.ByteToHexBitFiddle(publicExponent);
                    }
                    finally
                    {
                        // 生成したキーは保存しない.
                        rsa.PersistKeyInCsp = false;
                        rsa.Clear();
                    }
                }
            }
        }

        protected void BtnChangePassword_Click(object sender, EventArgs e)
        {
            string keyPair = Session[PRIVATE_KEY] as string;
            if (keyPair == null)
            {
                throw new ApplicationException("セッションに保存したプライベートキーがみつかりません");
            }

            // RSA暗号化された暗号文の取得
            byte[] encryptedNewPass = LoginUtils.StringToByteArray(ENCRYPTED_NEW_PASS.Value);

            // RSAを復号する
            byte[] decryptedNewPass;
            using (var rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    rsa.FromXmlString(keyPair);

                    decryptedNewPass = rsa.Decrypt(encryptedNewPass, false);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                    rsa.Clear();
                }
            }

            string newPassword = new string(System.Text.Encoding.UTF8.GetChars(decryptedNewPass));

            MembershipUser user = Membership.GetUser();
            if (user == null)
            {
                Message.Text = "ログインしていません";
                Message.ForeColor = Color.Red;
                return;
            }

            string curPassword = user.GetPassword();
            if (!user.ChangePassword(curPassword, newPassword))
            {
                Message.Text = "パスワードを変更できません";
                Message.ForeColor = Color.Red;
                return;
            }

            Message.Text = "パスワードを変更しました";
            Message.ForeColor = Color.Blue;
            return;
        }
    }
}