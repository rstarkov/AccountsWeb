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

        internal Translation Tr
        {
            get
            {
                return Program.Tr;
            }
        }

        public virtual bool IgnoreGlobalMessage
        {
            get { return false; }
        }

        public GncAccount GetAccount(string acctArgName)
        {
            var acct = Request.GetValidated(acctArgName, "");
            try { return Program.CurFile.Book.GetAccountByPath(acct); }
            catch (RTException) { throw new ValidationException(acctArgName, acct, Tr.Spinneret_Validation_AcctMustExist); }
        }

        public IEnumerable<object> GetAccountBreadcrumbs(string acctArgName, GncAccount account)
        {
            var path = account == null ? null : account.PathAsList();

            if (account == null || path.Count == 0)
                yield return new SPAN(account.Book.AccountRoot.Name) { class_ = "aw-breadcrumbs aw-current" };
            else
            {
                yield return new A(account.Book.AccountRoot.Name) { class_ = "aw-breadcrumbs", href = Request.SameUrlExceptSet(acctArgName, "") };
                yield return " : ";
                foreach (var item in path.Take(path.Count - 1))
                {
                    yield return new A(item.Name) { class_ = "aw-breadcrumbs", href = Request.SameUrlExceptSet(acctArgName, item.Path(":")) };
                    yield return " : ";
                }
                yield return new SPAN(path.Last().Name) { class_ = "aw-breadcrumbs aw-current" };
            }
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
                        new P(Program.Tr.GlobalMessage.Explanation.Fmt("AccountsWeb")),
                        Program.CurFile.GlobalErrorMessage
                    );
                try
                {
                    (page as WebPage).FullScreen = false;
                    return MakePage(page, Program.Tr.GlobalMessage.Title, content);
                }
                catch (Exception e)
                {
                    return new DIV(content, new P(Program.Tr.GlobalMessage.AlsoException), new RAWHTML(HttpServer.ExceptionAsString(e, true)));
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
            yield return new A(Program.Tr.AddLink) { href = "/AddLink?Href=" + page.Request.Url.UrlEscape() };
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
                    new H2(new IMG() { src = "/Static/warning_10.png" }, " ", Program.Tr.WarningsLink),
                    new UL(new LI(new A(Program.Tr.WarningsLink) { href = "/Warnings" }))
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
            RegisterPage("/MonthlyTotals", Program.Tr.NavigationHeader, Program.Tr.PgMonthlyTotals.NavLink, req => new PageMonthlyTotals(req, this));
            RegisterPage("/LastBalsnap", Program.Tr.NavigationHeader, Program.Tr.PgLastBalsnap.NavLink, req => new PageLastBalsnap(req, this));
            RegisterPage("/ExRates", Program.Tr.NavigationHeader, Program.Tr.PgExRates.NavLink, req => new PageExRates(req, this));
            RegisterPage("/Trns", Program.Tr.NavigationHeader, Program.Tr.PgTrns.NavLink, req => new PageTrns(req, this));
            RegisterPage("/About", Program.Tr.NavigationHeader, Program.Tr.PgAbout.NavLink, req => new PageAbout(req, this));
            RegisterPage("/Warnings", req => new PageWarnings(req, this));
        }

        public override IEnumerable<NavLink> NavLinksUser
        {
            get
            {
                if (Program.CurFile == null || Program.CurFile.UserLinks == null)
                    yield break;
                foreach (var link in Program.CurFile.UserLinks)
                    yield return new NavLink(Program.Tr.LinksHeader, link.Name, link.Href);
            }
        }
    }
}
