using System;
using System.Collections.Generic;
using System.Linq;
using GnuCashSharp;
using RT.Servers;
using RT.Spinneret;
using RT.TagSoup;
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
                yield return new A(account.Book.AccountRoot.Name) { class_ = "aw-breadcrumbs", href = Request.Url.WithQuery(acctArgName, "").ToHref() };
                yield return " : ";
                foreach (var item in path.Take(path.Count - 1))
                {
                    yield return new A(item.Name) { class_ = "aw-breadcrumbs", href = Request.Url.WithQuery(acctArgName, item.Path(":")).ToHref() };
                    yield return " : ";
                }
                yield return new SPAN(path.Last().Name) { class_ = "aw-breadcrumbs aw-current" };
            }
        }

        public static object FormatCcys(GncMultiAmount amount, bool isConverted, bool whole)
        {
            string stringify(decimal amt, bool whole2)
            {
                if (!whole2)
                    return $"{amt:#,0.00}";

                if (amt > 0 && amt < 1m)
                    amt = 1m;
                else if (amt < 0 && amt > -1m)
                    amt = -1m;
                return $"{amt:#,0}";
            }

            if (isConverted)
                return stringify(amount.Single().Quantity, amount.Single().Commodity.Identifier == "UAH" ? true : whole);

            var result = new List<object>();
            foreach (var amt in amount.Where(a => a.Quantity != 0).OrderByDescending(a => a.Commodity.Identifier)) // because they are typically right-aligned, so the main currency should be the rightmost one
            {
                string str;
                switch (amt.Commodity.Identifier)
                {
                    case "GBP": str = "£" + stringify(amt.Quantity, whole); break;
                    case "EUR": str = "€" + stringify(amt.Quantity, whole); break;
                    case "USD": str = "$" + stringify(amt.Quantity, whole); break;
                    case "UAH": str = stringify(amt.Quantity, true) + " грн"; break;
                    default: str = amt.Commodity.Identifier + " " + stringify(amt.Quantity, whole); break;
                }
                result.Add(new SPAN(str) { class_ = "ccy_" + amt.Commodity.Identifier.Replace(":", "_") });
            }
            return result.InsertBetween(" ");
        }

        protected static void SetReportAmount(ReportAccounts report, GncAccount acct, object colref, GncMultiAmount amount, bool isConverted, string url = null)
        {
            if (amount.Count == 0)
                report.SetValue(acct, colref, "-", ReportTable.CssClassNumber(0));
            else
            {
                var val = FormatCcys(amount, isConverted, whole: true);
                report.SetValue(acct, colref, url == null ? val : new A(val) { href = url },
                    isConverted ? ReportTable.CssClassNumber(amount.Single().Quantity) : "");
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
                    return new DIV(content, new P(Program.Tr.GlobalMessage.AlsoException),
                        e.SelectChain(ex => ex.InnerException)
                        .Select(ex => new DIV { class_ = "exception" }._(
                            new H3(ex.GetType().FullName),
                            new P(ex.Message),
                            new PRE(ex.StackTrace)
                        )).InsertBetween(new HR()));
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
            yield return new A(Program.Tr.AddLink) { href = "/AddLink?Href=" + page.Request.Url.ToHref().UrlEscape() };
            foreach (var a in baseGetFloatingLinks(page))
                yield return a;
        }

        private IEnumerable<A> baseGetFloatingLinks(SpinneretPage page)
        {
            return base.GetFloatingLinks(page);
        }

        protected override object GetNavPanelBottom(SpinneretPage page)
        {
            var sections = new List<object>();

            var values = Program.CurFile.UserScripts.Values.Where(v => v.Location.HasFlag(ScriptDisplayLocation.Sidebar)).ToList();
            if (values.Count > 0)
                sections.Add(new object[] {
                    new A(Program.Tr.PgCalculations.EditLink) { href="/CalculationsEdit", style="float: right; font-size: 10pt; margin-top: 1px; margin-right: 4px;" },
                    new H2(Program.Tr.PgCalculations.Title),
                    new UL(values.Select(v => new LI(v.Name, ": ", v.TagSoupValue)))
                });

            if (Program.CurFile.Session != null && Program.CurFile.Session.EnumWarnings().Any())
                sections.Add(new object[] {
                    new H2(new IMG() { src = "/Static/warning_10.png", width = 14, style = "position: relative; top: 1px;" }, " ", Program.Tr.WarningsLink),
                    new UL(new LI(new A(Program.Tr.WarningsLink) { href = "/Warnings" }))
                });

            return sections;
        }
    }

    public class WebInterface : SpinneretInterface
    {
        public WebInterface()
        {
            Layout = new WebLayout(this);
        }

        public override void RegisterHandlers(FileSystemOptions fsOptions)
        {
            base.RegisterHandlers(fsOptions);
            RegisterPage("/", req => new PageMain(req, this));
            RegisterPage("/AddLink", req => new PageAddLink(req, this));
            RegisterPage("/MonthlyTotals", Program.Tr.NavigationHeader, Program.Tr.PgMonthlyTotals.NavLink, req => new PageMonthlyTotals(req, this));
            RegisterPage("/MonthlyBalances", Program.Tr.NavigationHeader, Program.Tr.PgMonthlyBalances.NavLink, req => new PageMonthlyBalances(req, this));
            RegisterPage("/LastBalsnap", Program.Tr.NavigationHeader, Program.Tr.PgLastBalsnap.NavLink, req => new PageLastBalsnap(req, this));
            RegisterPage("/ExRates", Program.Tr.NavigationHeader, Program.Tr.PgExRates.NavLink, req => new PageExRates(req, this));
            RegisterPage("/Trns", Program.Tr.NavigationHeader, Program.Tr.PgTrns.NavLink, req => new PageTrns(req, this));
            RegisterPage("/Reconcile", Program.Tr.NavigationHeader, Program.Tr.PgReconcile.NavLink, req => new PageReconcile(req, this));
            RegisterPage("/BalancesAt", Program.Tr.NavigationHeader, Program.Tr.PgBalancesAt.NavLink, req => new PageBalancesAt(req, this));
            RegisterPage("/TotalsBetween", Program.Tr.NavigationHeader, Program.Tr.PgTotalsBetween.NavLink, req => new PageTotalsBetween(req, this));
            RegisterPage("/Calculations", Program.Tr.NavigationHeader, Program.Tr.PgCalculations.Title, req => new PageCalculations(req, this));
            RegisterPage("/CalculationsEdit", req => new PageCalculationsEdit(req, this));
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
