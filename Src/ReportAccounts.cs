using System.Collections.Generic;
using RT.Servers;
using RT.TagSoup.HtmlTags;
using RT.Util.ExtensionMethods;
using RT.Util.Streams;

namespace AccountsWeb
{
    public class ReportAccounts
    {
        public class Acct
        {
            public string Name;
            public Acct Parent;

            public Acct(string name)
            {
                Name = name;
            }

            public Acct(string name, Acct parent)
                : this(name)
            {
                Parent = parent;
            }

            public int Depth
            {
                get
                {
                    int depth = 0;
                    var acct = this;
                    while (acct.Parent != null)
                    {
                        depth++;
                        acct = acct.Parent;
                    }
                    return depth;
                }
            }
        }

        public class Col
        {
            public string Title;
            public Col Parent;

            public Col(string title)
            {
                Title = title;
            }

            public Col(string title, Col parent)
                : this(title)
            {
                Parent = parent;
            }
        }

        public class Val
        {
            public string Text;
            public string Href;

            public Val(string text)
            {
                Text = text;
            }

            public Val(string text, string href)
                : this(text)
            {
                Href = href;
            }
        }

        public List<Acct> Accounts = new List<Acct>();
        public List<Col> Cols = new List<Col>();
        public Dictionary<Acct, Dictionary<Col, Val>> Vals = new Dictionary<Acct, Dictionary<Col, Val>>();
        public string ReportCss;
        public string ReportTitle = "Accounts report";

        public Val this[Acct acct, Col col]
        {
            get
            {
                if (!Vals.ContainsKey(acct))
                    return null;
                if (!Vals[acct].ContainsKey(col))
                    return null;
                return Vals[acct][col];
            }
            set
            {
                if (!Vals.ContainsKey(acct))
                    Vals.Add(acct, new Dictionary<Col,Val>());
                if (!Vals[acct].ContainsKey(col))
                    Vals[acct].Add(col, value);
                else
                    Vals[acct][col] = value;
            }
        }

        public HtmlTag GetContent()
        {
            HtmlPrinter prn = new HtmlPrinter(new TABLE());

            int rownum = 0;
            int nextHeader = 0;
            int lastDepth = int.MaxValue;
            foreach (var acct in Accounts)
            {
                int curDepth = acct.Depth;

                if (nextHeader <= rownum && lastDepth > curDepth)
                {
                    prn.OpenTag(new TR() { class_ = "rowHeader" });
                    prn.AddTag(new TD() { class_ = "cellTopLeft" });
                    foreach (var col in Cols)
                        prn.AddTag(new TD(col.Title));
                    prn.CloseTag();
                    nextHeader = rownum + 30;
                }

                prn.OpenTag(new TR() { class_ = (rownum % 2 == 0 ? "rowEven" : "rowOdd") + " rowDepth" + curDepth });
                prn.AddTag(new TD("\u2003\u2003".Repeat(curDepth) + acct.Name) { class_ = "cellAcct" });
                foreach (var col in Cols)
                    prn.AddTag(new TD(this[acct, col].Text) { class_ = "cellNum" });
                prn.CloseTag();

                lastDepth = curDepth;
                rownum++;
            }

            return prn.GetHtml();
        }
    }
}
