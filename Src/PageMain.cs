using RT.Servers;
using RT.TagSoup;
using RT.Util;

namespace AccountsWeb
{
    public class PageMain : WebPage
    {
        public PageMain(UrlPathRequest request, WebInterface iface)
            : base(request, iface)
        {
            EqatecAnalytics.Monitor.TrackFeature("PageMain.Load");
        }

        public override string GetTitle()
        {
            return "GnuCash AccountsWeb";
        }

        public override object GetContent()
        {
            return new P(Tr.PgMain.WelcomeMessage);
        }
    }
}
