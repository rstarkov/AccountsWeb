using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Util.ExtensionMethods;
using RT.TagSoup;
using RT.TagSoup.HtmlTags;

namespace AccountsWeb
{
    public class ReportTable
    {
        private List<Row> _rows = new List<Row>();
        private List<Col> _cols = new List<Col>();

        public List<Row> Rows
        {
            get { return _rows; }
        }

        public List<Col> Cols
        {
            get { return _cols; }
        }

        public Col AddCol(string title)
        {
            var col = new Col(title);
            _cols.Add(col);
            return col;
        }

        public Col AddCol(string title, string cssclass)
        {
            var col = new Col(title, cssclass);
            _cols.Add(col);
            return col;
        }

        public Row AddRow()
        {
            var row = new Row();
            _rows.Add(row);
            return row;
        }

        public Row AddRow(string cssclass)
        {
            var row = new Row(cssclass);
            _rows.Add(row);
            return row;
        }

        public Row AddRow(string cssclass, params object[] values)
        {
            var row = new Row(cssclass);
            _rows.Add(row);
            int i = 0;
            foreach (var col in _cols)
            {
                if (i >= values.Length)
                    break;
                else
                    row[col] = new Val(values[i]);
                i++;
            }
            return row;
        }

        public Tag GetHtml()
        {
            if (_rows.Count == 0)
                return new P("There are no items to show.") { class_ = "info_msg" };

            HtmlPrinter prn = new HtmlPrinter(new TABLE() { class_ = "report_table" });

            int rownum = 0;
            int nextHeader = 0;
            int lastDepth = int.MaxValue;
            foreach (var row in _rows)
            {
                if (nextHeader <= rownum && (row.Depth < lastDepth || row.Depth < 0))
                {
                    prn.OpenTag(new TR() { class_ = "row_header" });
                    foreach (var col in Cols)
                        prn.AddTag(new TD(col.Title) { class_ = MakeCssClass(col.CssClass) });
                    prn.CloseTag();
                    nextHeader = rownum + 30;
                }

                string rowcss = MakeCssClass(
                    row.CssClass,
                    rownum % 2 == 0 ? "row_even" : "row_odd",
                    row.Depth >= 0 ? " row_depth_" + row.Depth : null);
                prn.OpenTag(new TR() { class_ = rowcss});
                foreach (var col in _cols)
                {
                    var val = row[col];
                    if (val == null)
                        prn.AddTag(new TD() { class_ = MakeCssClass(col.CssClass) });
                    else
                        prn.AddTag(new TD(val.Content) { class_ = MakeCssClass(col.CssClass, val.CssClass) });
                }
                prn.CloseTag();

                lastDepth = row.Depth;
                rownum++;
            }

            return prn.GetHtml();
        }

        public string MakeCssClass(params string[] classes)
        {
            if (classes.Length == 0)
                return null;
            StringBuilder sb = new StringBuilder();
            foreach (var cls in classes)
                if (cls != null)
                {
                    sb.Append(cls);
                    sb.Append(' ');
                }
            if (sb.Length == 0)
                return null;
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public class Col
        {
            public Col(string title)
            {
                Title = title;
            }

            public Col(string title, string cssclass)
                : this(title)
            {
                CssClass = cssclass;
            }

            public string Title { get; set; }
            public string CssClass { get; set; }
        }

        public class Row
        {
            private Dictionary<Col, Val> _cells = new Dictionary<Col, Val>();

            public Row()
            {
                Depth = -1;
            }

            public Row(string cssclass)
                : this()
            {
                CssClass = cssclass;
            }

            public Val this[Col column]
            {
                get { return _cells.Get(column, null); }
                set { _cells[column] = value; }
            }

            public int Depth { get; set; }
            public string CssClass { get; set; }

            public Val SetValue(Col column, object content)
            {
                var value = new Val(content, null);
                _cells[column] = value;
                return value;
            }

            public Val SetValue(Col column, object content, string cssclass)
            {
                var value = new Val(content, cssclass);
                _cells[column] = value;
                return value;
            }
        }

        public class Val
        {
            public Val(object content)
            {
                Content = content;
                CssClass = null;
            }

            public Val(object content, string cssclass)
            {
                Content = content;
                CssClass = cssclass;
            }

            public object Content { get; set; }
            public string CssClass { get; set; }
        }

        public static string CssClassNumber(decimal number)
        {
            return number == 0m ? "table_num_zero" : number > 0m ? "table_num_pos" : "table_num_neg";
        }
    }
}
