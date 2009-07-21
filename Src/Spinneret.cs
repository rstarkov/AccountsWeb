using System.Linq;
using GnuCashSharp;
using RT.Servers;
using RT.Spinneret;
using RT.TagSoup;
using RT.TagSoup.HtmlTags;
using RT.Util.ExtensionMethods;

namespace AccountsWeb
{
    public abstract class WebPage : SpinneretPage
    {
        public WebPage(HttpRequest request, WebInterface iface)
            : base(request, iface)
        {
            Program.CurFile.ReloadSessionIfNecessary();
        }

        public virtual bool IgnoreGlobalMessage
        {
            get { return false; }
        }

        public Tag GenerateBreadCrumbs(HttpRequest request, GncAccount acct)
        {
            Tag breadcrumbs;
            var path = acct == null ? null : acct.PathAsList();

            var acct_link = (acct == Program.CurFile.Book.AccountRoot)
                    ? (object) ""
                    : new SPAN(" (", new A("account's page") { href = "/Account/" + acct.Path("/") }, ")");

            if (acct == null || path.Count == 0)
                breadcrumbs = new P() { class_ = "breadcrumbs" }._("> ", new SPAN("Root") { class_ = "breadcrumbs" }, acct_link);
            else
                breadcrumbs = new P() { class_ = "breadcrumbs" }._("> ",
                    new A("Root") { class_ = "breadcrumbs", href = request.SameUrlExceptSetRest("/") }, " > ",
                    path.Take(path.Count - 1).SelectMany(item => new object[] {
                        new A(item.Name) { class_ = "breadcrumbs", href = request.SameUrlExceptSetRest("/" + item.Path("/")) },
                        " : "
                    }),
                    new SPAN(path.Last().Name) { class_ = "breadcrumbs" },
                    acct_link
                );
            return breadcrumbs;
        }

        public GncAccount GetAccountFromRestUrl()
        {
            var acctpath = Request.RestUrlWithoutQuery.UrlUnescape().Replace("/", ":");
            if (acctpath.StartsWith(":"))
                acctpath = acctpath.Substring(1);
            return Program.CurFile.Book.GetAccountByPath(acctpath);
        }
    }

    internal class WebLayout : SnowWhiteLayout
    {
        internal WebLayout(WebInterface iface)
            : base(iface)
        {
        }

        public override Tag GetPageHtml(SpinneretPage page)
        {
            bool globmsg = Program.CurFile.GlobalErrorMessage != null && !(page as WebPage).IgnoreGlobalMessage;
            if (!globmsg)
                return base.GetPageHtml(page);
            else
            {
                (page as WebPage).FullScreen = false;
                return MakePage(page, "AccountsWeb message", new DIV(
                    new P("AccountsWeb cannot process your request due to the following error:"),
                    Program.CurFile.GlobalErrorMessage
                ));
            }
        }

        protected override object GetHomeLinkBody()
        {
            return Program.CurFile.FileNameOnly.Replace(".accweb", "");
        }

        protected override string GetCssLink()
        {
            return "/Static/AccountsWeb-SnowWhite.css";
        }

        protected override object GetNavPanelBottom(SpinneretPage page)
        {
            if (Program.CurFile.Session.EnumWarnings().Any())
                return new object[] {
                    new H2(new IMG() { src = "/Static/warning_10.png" }, " Warnings"),
                    new UL(new LI(new A("Warnings") { href = "/Warnings" }))
                };
            else
                return null;
        }
    }

    public class WebInterface : SpinneretInterface
    {
        public WebInterface()
        {
            Layout = new WebLayout(this);
        }

        public override void RegisterHandlers()
        {
            base.RegisterHandlers();
            RegisterPage("/", req => new PageMain(req, this));
            RegisterPage("/MonthlyTotals", "Navigation", "Monthly totals", req => new PageMonthlyTotals(req, this));
            RegisterPage("/LastBalsnap", "Navigation", "Last balsnaps", req => new PageLastBalsnap(req, this));
            RegisterPage("/ExRates", "Navigation", "Exchange rates", req => new PageExRates(req, this));
            RegisterPage("/Trns", "Navigation", "Transactions", req => new PageTrns(req, this));
            RegisterPage("/About", "Navigation", "About", req => new PageAbout(req, this));
            RegisterPage("/Warnings", req => new PageWarnings(req, this));
        }
    }
}
