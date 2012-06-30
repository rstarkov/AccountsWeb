using RT.Servers;
using RT.TagSoup;
using RT.Util;
using RT.Util.ExtensionMethods;

namespace AccountsWeb
{
    public class PageAbout: WebPage
    {
        public PageAbout(HttpRequest request, WebInterface iface)
            : base(request, iface)
        {
            EqatecAnalytics.Monitor.TrackFeature("PageAbout.Load");
        }

        public override string GetTitle()
        {
            return Tr.PgAbout.Title.Translation.Fmt("AccountsWeb");
        }

        public override object GetContent()
        {
            return new object[]
            {
                new P(Tr.PgAbout.Version.Fmt(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version)),
                new P("Copyright © 2009-2012 Roman Starkov")
            };
        }

        public override bool IgnoreGlobalMessage
        {
            get { return true; }
        }
    }
}
