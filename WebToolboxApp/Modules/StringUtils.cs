using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace WebToolboxApp.Moudles
{
    /// <summary>
    /// 文字列関連ユーテリティ
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// 空白区切り正規表現
        /// </summary>
        private static Regex spacerRegex = new Regex("\\s+");

        /// <summary>
        /// 改行コードごとに一行の文字列とし行のコレクションとして文字列配列を返す.
        /// </summary>
        /// <param name="rawText">入力文字列</param>
        /// <returns>改行ごとに一行の文字列とした文字列配列</returns>
        public static IEnumerable<string> ConvertRows(string rawText)
        {
            rawText = rawText ?? "";
            rawText = rawText.Replace("\r\n", "\n");
            rawText = rawText.Replace("\r", "\n");
            return rawText.Split('\n');
        }

        /// <summary>
        /// タブまたは空白を区切りとしてカラムを分割して返す
        /// </summary>
        /// <param name="line">分割される一行</param>
        /// <param name="tabOnly">タブのみを区切文字とする場合はtrue、デフォルトはfalse</param>
        /// <returns>カラムの配列</returns>
        public static string[] Split(string line, bool tabOnly = false)
        {
            line = line ?? "";
            if (tabOnly)
            {
                return line.Split('\t');
            }
            return spacerRegex.Replace(line.Trim(), "\t").Split('\t');
        }

    }
}