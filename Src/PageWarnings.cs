using RT.Servers;
using RT.TagSoup;

namespace AccountsWeb;

public class PageWarnings : WebPage
{
    public PageWarnings(HttpRequest request, WebInterface iface)
        : base(request, iface)
    {
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
