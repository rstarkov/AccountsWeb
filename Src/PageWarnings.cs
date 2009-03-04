using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Servers;
using RT.TagSoup;
using RT.TagSoup.HtmlTags;

namespace AccountsWeb
{
    public class PageWarnings: Page
    {
        public PageWarnings(HttpRequest request)
            : base(request)
        {
        }

        public override string GetBaseUrl()
        {
            return "/Warnings";
        }

        public override string GetTitle()
        {
            return "Warnings";
        }

        public override IEnumerable<string> GetCss()
        {
            yield break;
        }

        public override IEnumerable<Tag> GetBody()
        {
            yield return new DIV(
                Program.CurFile.Session.EnumWarnings().Count() == 0
                    ? (HtmlTag)new P("None")
                    : (HtmlTag)new UL(Program.CurFile.Session.EnumWarnings().Select(str => new LI(str)))
            );
        }
    }
}
