using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Servers;
using RT.TagSoup;
using RT.TagSoup.HtmlTags;

namespace AccountsWeb
{
    public class PageMain: Page
    {
        public PageMain(HttpRequest request)
            : base(request)
        {
        }

        public override string GetBaseUrl()
        {
            return "/";
        }

        public override string GetTitle()
        {
            return "Main";
        }

        public override IEnumerable<string> GetCss()
        {
            yield break;
        }

        public override IEnumerable<Tag> GetBody()
        {
            yield return new UL(
                new LI(new A("Monthly totals") { href = "/MonthlyTotals" })
            );
        }
    }
}
