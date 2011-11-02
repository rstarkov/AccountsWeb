using System.Linq;
using RT.Servers;
using RT.TagSoup;
using RT.Util;

namespace AccountsWeb
{
    public class PageWarnings: WebPage
    {
        public PageWarnings(HttpRequest request, WebInterface iface)
            : base(request, iface)
        {
            EqatecAnalytics.Monitor.TrackFeature("PageWarnings.Load");
        }

        public override string GetTitle()
        {
            return Tr.PgWarnings.Title;
        }

        public override object GetContent()
        {
            return new object[] {
                Program.CurFile.Session.EnumWarnings().Count() == 0
                    ? (Tag) new P(Tr.PgWarnings.MessageNoWarnings) { class_ = "aw-info-msg" }
                    : (Tag) new UL(Program.CurFile.Session.EnumWarnings().Select(str => new LI(str)))
            };
        }
    }
}
