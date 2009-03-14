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

        public override object GetBody()
        {
            return new object[] {
                Program.CurFile.Session.EnumWarnings().Count() == 0
                    ? (Tag) new P("There were no warnings.") { class_ = "info_msg" }
                    : (Tag) new UL(Program.CurFile.Session.EnumWarnings().Select(str => new LI(str)))
            };
        }
    }
}
