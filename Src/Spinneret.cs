using System;
using System.Collections.Generic;
using System.Linq;
using GnuCashSharp;
using RT.Servers;
using RT.Spinneret;
using RT.TagSoup;
using RT.TagSoup.HtmlTags;
using RT.Util;
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

        public GncAccount GetAccount(string getArgName)
        {
            var acct = Request.GetValidated(getArgName, "");
            try { return Program.CurFile.Book.GetAccountByPath(acct); }
            catch (RTException) { throw new ValidationException(getArgName, acct, "the name of an existing account"); }
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
                Tag content = new DIV(
                        new P("AccountsWeb cannot process your request due to the following error:"),
                        Program.CurFile.GlobalErrorMessage
                    );
                try
                {
                    (page as WebPage).FullScreen = false;
                    return MakePage(page, "AccountsWeb message", content);
                }
                catch (Exception e)
                {
                    return new DIV(content, new P("Additionally, an exception has occurred:"), new RAWHTML(HttpServer.ExceptionAsString(e, true)));
                }
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

        protected override IEnumerable<A> GetFloatingLinks(SpinneretPage page)
        {
            yield return new A("Add link") { href = "/AddLink?Href=" + page.Request.Url.UrlEscape() };
            foreach (var a in baseGetFloatingLinks(page))
                yield return a;
        }

        private IEnumerable<A> baseGetFloatingLinks(SpinneretPage page)
        {
            return base.GetFloatingLinks(page);
        }

        protected override object GetNavPanelBottom(SpinneretPage page)
        {
            if (Program.CurFile.Session != null && Program.CurFile.Session.EnumWarnings().Any())
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
            RegisterPage("/AddLink", req => new PageAddLink(req, this));
            RegisterPage("/MonthlyTotals", "Navigation", "Monthly totals", req => new PageMonthlyTotals(req, this));
            RegisterPage("/LastBalsnap", "Navigation", "Last balsnaps", req => new PageLastBalsnap(req, this));
            RegisterPage("/ExRates", "Navigation", "Exchange rates", req => new PageExRates(req, this));
            RegisterPage("/Trns", "Navigation", "Transactions", req => new PageTrns(req, this));
            RegisterPage("/About", "Navigation", "About", req => new PageAbout(req, this));
            RegisterPage("/Warnings", req => new PageWarnings(req, this));
        }

        public override IEnumerable<NavLink> NavLinksUser
        {
            get
            {
                if (Program.CurFile == null || Program.CurFile.UserLinks == null)
                    yield break;
                foreach (var link in Program.CurFile.UserLinks)
                    yield return new NavLink("Links", link.Name, link.Href);
            }
        }
    }
}
