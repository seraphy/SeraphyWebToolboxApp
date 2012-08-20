using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Text;
using WebToolboxApp.Moudles;

namespace WebToolboxApp
{
    public class ColumnDef
    {
        public string Name { set; get; }

        public string DBType { set; get; }

        public bool IsNumeric { set; get; }

        public override string ToString()
        {
            var buf = new StringBuilder();
            buf.Append("(Name=").Append(Name);
            buf.Append(", DBType=").Append(DBType);
            buf.Append(", IsNumeric=").Append(IsNumeric);
            buf.Append(")");
            return buf.ToString();
        }
    }

    public partial class ConvInsertDML : System.Web.UI.Page
    {
        private Regex spacerRegex = new Regex("\\s+");

        private readonly string[] numericTypes =
            {
                "numeric", "number", "decimal", "currency",
                "integer", "int", "long", "bool", "boolean", "float", "double", "real"
            };

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected string RegulateValue(string value, bool isNumeric)
        {
            value = value ?? "";
            if (isNumeric)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    // 数値でnullの場合
                    return "null";
                }
                // 数値
                return value;
            }
            // 文字列
            value = value.Replace("'", "''");
            return "'" + value + "'";
        }

        protected void BtnGenerate_Click(object sender, EventArgs e)
        {
            var sql = new StringBuilder();
            IList<ColumnDef> columnDefs = GetColumnDefs(TxtColumns.Text);
            ParseRows(TxtDataRows.Text, (columnDatas) =>
                {
                    var header = new StringBuilder();
                    var values = new StringBuilder();
                    int mx = Math.Min(columnDefs.Count, columnDatas.Length);
                    for (int colIdx = 0; colIdx < mx; colIdx++)
                    {
                        var columnDef = columnDefs[colIdx];

                        if (header.Length > 0)
                        {
                            header.Append(", ");
                        }
                        header.Append(columnDef.Name);
                        string value = RegulateValue(
                            columnDatas[colIdx],
                            columnDef.IsNumeric
                            );
                        if (values.Length > 0)
                        {
                            values.Append(", ");
                        }
                        values.Append(value);
                    }

                    if (ContinuationValues.Checked)
                    {
                        if (sql.Length == 0)
                        {
                            sql.Append("insert into ");
                            sql.Append(TxtTableName.Text);
                            sql.Append(" (");
                            sql.Append(header);
                            sql.Append(") values\r\n");
                            sql.Append("(");
                            sql.Append(values);
                            sql.Append(")");
                        }
                        else
                        {
                            sql.Append(",\r\n(");
                            sql.Append(values);
                            sql.Append(")");
                        }
                    }
                    else
                    {
                        sql.Append("insert into ");
                        sql.Append(TxtTableName.Text);
                        sql.Append(" (");
                        sql.Append(header);
                        sql.Append(") values (");
                        sql.Append(values);
                        sql.Append(");\r\n");
                    }
                });

            if (ContinuationValues.Checked)
            {
                sql.Append(";\r\n");
            }

            TxtSQL.Text = sql.ToString();
            TxtSQL.Visible = true;
        }

        private void ParseRows(string txtRows, Action<string[]> callback)
        {
            IEnumerable<string> rowLines = StringUtils.ConvertRows(txtRows);
            foreach (string rowLine in rowLines)
            {
                if (!string.IsNullOrWhiteSpace(rowLine))
                {
                    callback(StringUtils.Split(rowLine, CheckTabOnly.Checked));
                }
            }
        }

        private IList<ColumnDef> GetColumnDefs(string txtColumn)
        {
            IEnumerable<string> columnLines = StringUtils.ConvertRows(txtColumn);
            var columnDefs = new List<ColumnDef>();
            foreach (string columnLine in columnLines)
            {
                if (!string.IsNullOrWhiteSpace(columnLine))
                {
                    var tokens = StringUtils.Split(columnLine);
                    if (tokens.Length > 0)
                    {
                        string name = tokens[0];
                        string dbType = (tokens.Length > 1) ? tokens[1] : "";
                        string dbTypeLc = dbType.ToLower();
                        bool isNumeric = numericTypes.Any((token) => token == dbTypeLc);
                        var columnDef = new ColumnDef()
                        {
                            Name = name,
                            DBType = dbType,
                            IsNumeric = isNumeric
                        };
                        columnDefs.Add(columnDef);
                    }
                }
            }
            System.Diagnostics.Trace.WriteLine("columnDefs=[" +
                string.Join(", ", columnDefs.Select(o => o.ToString()).ToArray()) +
                "]");
            return columnDefs;
        }

        protected void BtnClear_Click(object sender, EventArgs e)
        {
            TxtSQL.Text = "";
            TxtSQL.Visible = false;
            TxtTableName.Text = "";
            TxtColumns.Text = "";
            TxtDataRows.Text = "";
        }
    }
}