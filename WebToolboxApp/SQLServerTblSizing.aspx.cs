using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebToolboxApp
{
    public partial class SQLServerTblSizing : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                TxtNumOfRows.Text = "100000";
                TxtFillFactor.Text = "100";
            }
        }

        protected void BtnClear_Click(object sender, EventArgs e)
        {
            TxtResult.Text = "";
            TxtSource.Text = "";
        }

        private enum Mode {
            HEADER,
            KEYS,
            COLUMNS
        };

        private enum ColumnType
        {
            FIXED,
            VARIABLE
        };

        private class PatPair
        {
            public delegate int ParseDelg(Match mat, out ColumnType typ);
            public Regex Reg { set; get; }
            public ParseDelg Func {set; get;}
        }

        private PatPair[] defs = new PatPair[] {
                new PatPair() { Reg = new Regex("NUMBER\\((.+?)\\)", RegexOptions.IgnoreCase), Func = parseNumberColumn },
                new PatPair() { Reg = new Regex("DECIMAL\\((.+?)\\)", RegexOptions.IgnoreCase), Func = parseNumberColumn },
                new PatPair() { Reg = new Regex("NUMERIC\\((.+?)\\)", RegexOptions.IgnoreCase), Func = parseNumberColumn },

                new PatPair() { Reg = new Regex("CHAR\\((.+?)\\)", RegexOptions.IgnoreCase), Func = parseCharColumn },

                new PatPair() { Reg = new Regex("VARCHAR\\((.+?)\\)", RegexOptions.IgnoreCase), Func = parseVarCharColumn },
                new PatPair() { Reg = new Regex("VARCHAR2\\((.+?)\\)", RegexOptions.IgnoreCase), Func = parseVarCharColumn },

                new PatPair() { Reg = new Regex("DATE", RegexOptions.IgnoreCase), Func = parseDateTimeColumn },
                new PatPair() { Reg = new Regex("TIME", RegexOptions.IgnoreCase), Func = parseDateTimeColumn },
                new PatPair() { Reg = new Regex("DATETIME", RegexOptions.IgnoreCase), Func = parseDateTimeColumn },
            };

        private int parseColumnType(string line, out ColumnType typ)
        {
            line = line.Trim().ToUpper();

            foreach (PatPair patPair in defs)
            {
                Regex reg = patPair.Reg;
                Match mat = reg.Match(line);
                if (mat.Success)
                {
                    return patPair.Func(mat, out typ);
                }
            }

            // unknown
            typ = ColumnType.FIXED;
            return 0;
        }

        private static int parseNumberColumn(Match mat, out ColumnType typ)
        {
            typ = ColumnType.FIXED;

            Group capt = mat.Groups[1];
            string captStr = capt.Value.Trim();

            string precStr = captStr;
            int pt = captStr.IndexOf(",");
            if (pt >= 0)
            {
                precStr = captStr.Substring(0, pt);
            }

            int len = 0;
            int.TryParse(precStr, out len);

            int byteLen = 0;
            if (len <= 0)
            { 
                byteLen = 0;
            }
            else if (len <= 9)
            {
                byteLen = 5;
            }
            else if (len <= 19)
            {
                byteLen = 9;
            }
            else if (len <= 28)
            {
                byteLen = 13;
            }
            else if (len <= 38)
            {
                byteLen = 17;
            }

            return byteLen;
        }

        private static int parseCharColumn(Match mat, out ColumnType typ)
        {
            typ = ColumnType.FIXED;
            Group capt = mat.Groups[1];
            string captStr = capt.Value.Trim();
            int len = 0;
            if (int.TryParse(captStr, out len))
            {
                return len;
            }
            return 0;
        }

        private static int parseVarCharColumn(Match mat, out ColumnType typ)
        {
            typ = ColumnType.VARIABLE;
            Group capt = mat.Groups[1];
            string captStr = capt.Value.Trim();
            int len = 0;
            if (int.TryParse(captStr, out len))
            {
                return len;
            }
            return 0;
        }

        private static int parseDateTimeColumn(Match mat, out ColumnType typ)
        {
            typ = ColumnType.FIXED;
            return 8;
        }
        
        protected void BtnCalcurate_Click(object sender, EventArgs e)
        {
            string input = TxtSource.Text.Trim();
            string[] lines = input.Split(new string[] { "\n" }, StringSplitOptions.None);

            // 行数を取得
            int Num_Rows = 1;
            int.TryParse(TxtNumOfRows.Text, out Num_Rows);

            // 定義
            var keyDefs = new List<String>();
            long Num_Cols = 0;
            long Fixed_Data_Size = 0;
            long Num_Variable_Cols = 0;
            long Max_Var_Size = 0;

            var buf = new StringBuilder();

            // 解析
            // http://msdn.microsoft.com/ja-jp/library/ms178085.aspx
            Mode mode = Mode.HEADER;
            foreach (string line in lines)
            {
                String trimedLine = line.TrimEnd(new char[] { '\r', '\n' });
                trimedLine = trimedLine.Trim();

                if (mode == Mode.HEADER)
                {
                    // 先行する空白はスキップする.
                    if (trimedLine.Length == 0)
                    {
                        continue;
                    }
                    // ヘッダ
                    mode = Mode.KEYS;
                    buf.AppendLine("*key");
                }
                else if (mode == Mode.KEYS)
                {
                    if (trimedLine.Length == 0)
                    {
                        // キー定義の終了
                        mode = Mode.COLUMNS;
                        buf.AppendLine("*columns");
                        continue;
                    }
                    // キーの定義
                    keyDefs.Add(trimedLine);
                }

                // キーおよびカラムの定義
                if (trimedLine.Length == 0)
                {
                    continue;
                }

                buf.AppendLine(trimedLine);

                ColumnType typ;
                int len = parseColumnType(trimedLine, out typ);
                if (len == 0)
                {
                    buf.AppendLine("unknown " + trimedLine);
                }
                else
                {
                    if (typ == ColumnType.FIXED)
                    {
                        Fixed_Data_Size += len;
                    }
                    else if (typ == ColumnType.VARIABLE)
                    {
                        Num_Variable_Cols++;
                        Max_Var_Size += len;
                    }
                }
                Num_Cols++;
            }

            long Null_Bitmap = 2 + ((Num_Cols + 7) / 8);
            long Variable_Data_Size = 2 + (Num_Variable_Cols * 2) + Max_Var_Size;
            long Row_Size = Fixed_Data_Size + Variable_Data_Size + Null_Bitmap + 4;

            long Rows_Per_Page = 8096 / (Row_Size + 2);

            long Fill_Factor = 100;
            long.TryParse(TxtFillFactor.Text, out Fill_Factor);

            long Free_Rows_Per_Page = 8096 * ((100 - Fill_Factor) / 100) / (Row_Size + 2);

            long Num_Leaf_Pages = (long) Math.Floor(Num_Rows / (double)(Rows_Per_Page - Free_Rows_Per_Page));

            long Leaf_space_used = 8192 * Num_Leaf_Pages;

            buf.AppendLine();
            buf.AppendLine("*result");
            buf.AppendLine("Num_Rows=" + Num_Rows);
            buf.AppendLine("Row Size=" + Row_Size);
            buf.AppendLine("Require Size=" + Leaf_space_used);

            // キーサイズ
            long keySize = parseKeyDefs(keyDefs, Num_Leaf_Pages, buf);

            // 合計
            long total = Leaf_space_used + keySize;
            buf.AppendLine("*total");
            buf.AppendLine("total=" + String.Format("{0:#,0} bytes", total));

            TxtResult.Text = buf.ToString();
        }

        private long parseKeyDefs(IList<String> keyDefs, long Num_Leaf_Pages, StringBuilder buf)
        {
            buf.AppendLine();
            buf.AppendLine("*index size");

            long Num_Key_Cols = 0;
            long Fixed_Key_Size = 0;
            long Num_Variable_Key_Cols = 0;
            long Max_Var_Key_Size = 0;

            foreach (String key in keyDefs)
            {
                buf.AppendLine(key);

                ColumnType typ;
                int len = parseColumnType(key, out typ);
                if (len == 0)
                {
                    buf.AppendLine("unknown " + key);
                }
                else
                {
                    if (typ == ColumnType.FIXED)
                    {
                        Fixed_Key_Size += len;
                    }
                    else if (typ == ColumnType.VARIABLE)
                    {
                        Num_Variable_Key_Cols++;
                        Max_Var_Key_Size += len;
                    }
                }
                Num_Key_Cols++;
            }

            // null可の列
            // Index_Null_Bitmap = 2 + ((インデックス行の列数 + 7) / 8)
            long Index_Null_Bitmap = 0;


            long Variable_Key_Size = 2 + (Num_Variable_Key_Cols * 2) + Max_Var_Key_Size;

            long Index_Row_Size = Fixed_Key_Size + Variable_Key_Size + Index_Null_Bitmap + 1;

            long Index_Rows_Per_Page = 8096 / (Index_Row_Size + 2);

            long Non_leaf_Levels = (long)Math.Floor(1 + Math.Log(Index_Rows_Per_Page, (Num_Leaf_Pages / Index_Rows_Per_Page)));

            long Num_Index_Pages = 0;
            for (int lv = 1; lv <= Non_leaf_Levels; lv++)
            {
                Num_Index_Pages += (long)(Num_Leaf_Pages / (Math.Pow(Index_Rows_Per_Page, lv)));
            }

            long Index_Space_Used = 8192 * Num_Index_Pages;

            buf.AppendLine("Index_Row_Size=" + Index_Row_Size);
            buf.AppendLine("Index_Space_Used=" + Index_Space_Used);

            return Index_Space_Used;
        }
    }
}