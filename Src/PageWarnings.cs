using System.Linq;
using RT.Servers;
using RT.TagSoup;
using RT.TagSoup.HtmlTags;

namespace AccountsWeb
{
    public class PageWarnings: WebPage
    {
        public PageWarnings(HttpRequest request, WebInterface iface)
            : base(request, iface)
        {
        }

        public override string GetTitle()
        {
            return "Warnings";
        }

        public override object GetContent()
        {
            return new object[] {
                Program.CurFile.Session.EnumWarnings().Count() == 0
                    ? (Tag) new P("There were no warnings.") { class_ = "info_msg" }
                    : (Tag) new UL(Program.CurFile.Session.EnumWarnings().Select(str => new LI(str)))
            };
        }
    }
}
