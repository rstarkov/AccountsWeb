using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.TagSoup;
using RT.Servers;

namespace AccountsWeb
{
    public class PageTrns: Page
    {
        public PageTrns(HttpRequest request)
            : base(request)
        {
        }

        public override string GetBaseUrl()
        {
            return "/Trns";
        }

        public override string GetTitle()
        {
            return "Transactions";
        }

        public override IEnumerable<string> GetCss()
        {
            yield return "Transactions.css";
        }

        public override IEnumerable<Tag> GetBody()
        {
            throw new NotImplementedException();
        }
    }
}
