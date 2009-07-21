using RT.Servers;
using RT.TagSoup.HtmlTags;

namespace AccountsWeb
{
    public class PageMain: WebPage
    {
        public PageMain(HttpRequest request, WebInterface iface)
            : base(request, iface)
        {
        }

        public override string GetTitle()
        {
            return "GnuCash AccountsWeb";
        }

        public override object GetContent()
        {
            return new P("Welcome to AccountsWeb for GnuCash! Please select one of the links on the left.");
        }
    }
}
