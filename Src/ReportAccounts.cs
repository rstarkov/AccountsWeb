﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Servers;
using RT.TagSoup.XhtmlTags;
using RT.Util.Streams;
using RT.Util.ExtensionMethods;

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

        public HttpResponse Respond(HttpRequest request)
        {
            table tbl = new table();
            tr tr = null;
            int rownum = 0;
            int nextHeader = 0;
            int lastDepth = int.MaxValue;
            foreach (var acct in Accounts)
            {
                int curDepth = acct.Depth;

                if (nextHeader <= rownum && lastDepth > curDepth)
                {
                    tr = new tr(new td() { class_ = "cellTopLeft" }) { class_ = "rowHeader" };
                    foreach (var col in Cols)
                        tr.Add(new td(col.Title));
                    tbl.Add(tr);
                    nextHeader = rownum + 30;
                }

                tr = new tr(new td("\u2003\u2003".Repeat(curDepth) + acct.Name) { class_ = "cellAcct" });
                tr.class_ = (rownum % 2 == 0 ? "rowEven" : "rowOdd") + " rowDepth" + curDepth;
                foreach (var col in Cols)
                    tr.Add(new td(this[acct, col].Text) { class_ = "cellNum" });
                tbl.Add(tr);

                lastDepth = curDepth;
                rownum++;
            }

            var html = new html(
                new head(
                    new title(ReportTitle),
                    new link() { rel = "stylesheet", type = "text/css", href = "/Static/ReportAccounts.css" },
                    ReportCss == null ? (object)"" : new link() { rel = "stylesheet", type = "text/css", href = "/Static/" + ReportCss }
                ),
                new body(tbl)
            );

            return new HttpResponse()
            {
                Status = HttpStatusCode._200_OK,
                Headers = new HttpResponseHeaders() { ContentType = "text/html; charset=utf-8" },
                Content = new DynamicContentStream(html.ToEnumerable()),
            };
        }
    }

    /// columns:
    /// - tag / month / cre,deb,tot
    /// - month / tag / cre,deb,tot
    /// 
    /// tag: total
    /// cre,deb: total
}
