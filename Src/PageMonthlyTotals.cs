using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Servers;
using RT.TagSoup;
using RT.TagSoup.HtmlTags;
using GnuCashSharp;
using RT.Util.ExtensionMethods;

namespace AccountsWeb
{
    public class PageMonthlyTotals: Page
    {
        private DateInterval _interval;
        private ReportAccounts _report;
        private int _maxDepth;
        private bool _negate;
        private GncAccount _account;
        private Dictionary<DateInterval, ReportAccounts.Col> _colMap;
        private Dictionary<GncAccount, ReportAccounts.Acct> _acctMap;

        public PageMonthlyTotals(HttpRequest request)
            : base(request)
        {
        }

        public override string GetBaseUrl()
        {
            return "/MonthlyTotals";
        }

        public override string GetTitle()
        {
            return "Monthly Totals";
        }

        public override IEnumerable<string> GetCss()
        {
            yield return "ReportAccounts.css";
            yield return "MonthlyTotals.css";
        }

        public override IEnumerable<Tag> GetBody()
        {
            // Default to the last 12 months
            var toDefault = DateTime.Now;
            var frDefault = toDefault - new TimeSpan(360, 0, 0, 0);
            if (frDefault < Program.CurFile.Book.EarliestDate)
                frDefault = Program.CurFile.Book.EarliestDate;

            var fy = GetValidated<int>("FrYr", frDefault.Year);
            var fm = GetValidated<int>("FrMo", frDefault.Month, x => x >= 1 && x <= 12, "between 1 and 12");
            var ty = GetValidated<int>("ToYr", toDefault.Year, x => x >= fy, "no smaller than the starting year, {0}".Fmt(fy));
            var tm = GetValidated<int>("ToMo", toDefault.Month, x => x >= 1 && x <= 12 && (fy < ty || x >= fm), "between 1 and 12, and no smaller than the starting month, {0}".Fmt(fm));
            _maxDepth = GetValidated<int>("MaxDepth", -1, x => x >= 0, "non-negative");
            _negate = GetValidated<bool>("Neg", false);
            _interval = new DateInterval(fy, fm, 1, ty, tm, DateTime.DaysInMonth(ty, tm));

            var acctpath = Request.RestUrlWithoutQuery.UrlUnescape().Replace("/", ":");
            if (acctpath.StartsWith(":"))
                acctpath = acctpath.Substring(1);
            _account = Program.CurFile.Book.GetAccountByPath(acctpath);

            generateReport();
            var report_content =_report.Accounts.Count == 0
                ? new P("There are no items to show.") { class_ = "emptyresult" }
                : _report.GetContent();

            var html = new DIV(
                generateBreadCrumbs(Request, _account),
                report_content,
                new P("All values above are in {0}, converted where necessary using ".Fmt(Program.CurFile.BaseCurrency), new A("exchange rates") { href = "/ExRates" }, ".")
            );

            yield return html;
        }

        private Tag generateBreadCrumbs(HttpRequest request, GncAccount acct)
        {
            Tag breadcrumbs;
            var path = acct == null ? null : acct.PathAsList();

            var acct_link = (acct == Program.CurFile.Book.AccountRoot)
                    ? (object)""
                    : new SPAN(" (", new A("account's page") { href = "/Account/" + acct.Path("/") }, ")");

            if (acct == null || path.Count == 0)
                breadcrumbs = new P() { class_ = "breadcrumbs" }._("> ", new SPAN("Root") { class_ = "breadcrumbs" }, acct_link);
            else
                breadcrumbs = new P() { class_ = "breadcrumbs" }._("> ",
                    new A("Root") { class_ = "breadcrumbs", href = request.SameUrlExceptSetRest("/") }, " > ",
                    path.Take(path.Count-1).SelectMany(item => new object[] {
                        new A(item.Name) { class_ = "breadcrumbs", href = request.SameUrlExceptSetRest("/" + item.Path("/")) },
                        " : "
                    }),
                    new SPAN(path.Last().Name) { class_ = "breadcrumbs" },
                    acct_link
                );
            return breadcrumbs;
        }

        private ReportAccounts generateReport()
        {
            _report = new ReportAccounts();
            _colMap = new Dictionary<DateInterval, ReportAccounts.Col>();
            _acctMap = new Dictionary<GncAccount, ReportAccounts.Acct>();

            List<ReportAccounts.Col> cols = new List<ReportAccounts.Col>();
            foreach (var interval in _interval.EnumMonths())
            {
                var rCol = new ReportAccounts.Col(interval.Start.ToString("MMM\nyy"));
                _colMap.Add(interval, rCol);
                _report.Cols.Add(rCol);
            }

            foreach (var acctChild in _account.EnumChildren())
                processAccount(acctChild, null, 0);

            return _report;
        }

        private void processAccount(GncAccount acct, ReportAccounts.Acct parent, int depth)
        {
            var rAcct = acct.EnumChildren().Count() > 0
                ? new ReportAccounts.Acct(acct.Name, parent, Request.SameUrlExceptSetRest("/" + acct.Path("/")))
                : new ReportAccounts.Acct(acct.Name, parent);
            _acctMap.Add(acct, rAcct);
            _report.Accounts.Add(rAcct);

            foreach (var interval in _interval.EnumMonths())
            {
                decimal tot = acct.GetTotal(interval, true, acct.Book.GetCommodity(acct.Book.BaseCurrencyId));
                if (_negate)
                    tot = -tot;
                var value = tot == 0 ? "-" : "{0:# ###}".Fmt(tot);
                _report[rAcct, _colMap[interval]] = new ReportAccounts.Val(value);
            }

            if (depth < _maxDepth || _maxDepth == -1)
            {
                foreach (var acctChild in acct.EnumChildren())
                    processAccount(acctChild, rAcct, depth + 1);
            }
        }
    }
}
