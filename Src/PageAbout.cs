using RT.Servers;
using RT.TagSoup.HtmlTags;
using RT.Util.ExtensionMethods;

namespace AccountsWeb
{
    public class PageAbout: WebPage
    {
        public PageAbout(HttpRequest request, WebInterface iface)
            : base(request, iface)
        {
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
                new P("Copyright (C) 2009 Roman Starkov")
            };
        }

        public override bool IgnoreGlobalMessage
        {
            get { return true; }
        }
    }
}
