using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;

namespace WebToolboxApp.Login
{
    /// <summary>
    /// ログイン関連用のユーテリティ
    /// </summary>
    public static class LoginUtils
    {
        /// <summary>
        /// HEX文字列からバイト配列に変換する
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] StringToByteArray(string hex)
        {
            return SoapHexBinary.Parse(hex).Value;
        }

        public static string ByteToHexBitFiddle(byte[] bytes)
        {
            return new SoapHexBinary(bytes).ToString();
        }
    }
}