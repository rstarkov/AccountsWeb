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

        public override object GetBody()
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

            _account = GetAccountFromRestUrl();

            _report = new ReportAccounts(_account, Request, true, false);
            foreach (var interval in _interval.EnumMonths())
                _report.AddCol(interval, interval.Start.ToString("MMM\nyy"));
            processAccount(_account, 0);

            var html = new DIV(
                GenerateBreadCrumbs(Request, _account),
                _report.GetHtml(),
                new P("All values above are in {0}, converted where necessary using ".Fmt(Program.CurFile.BaseCurrency), new A("exchange rates") { href = "/ExRates" }, ".")
            );

            return html;
        }

        private void processAccount(GncAccount acct, int depth)
        {
            foreach (var interval in _interval.EnumMonths())
            {
                decimal tot = acct.GetTotal(interval, true, acct.Book.GetCommodity(acct.Book.BaseCurrencyId));
                if (_negate)
                    tot = -tot;
                if (tot == 0)
                    _report.SetValue(acct, interval, "-", ReportTable.CssClassNumber(tot));
                else
                {
                    if (tot > 0m && tot < 1m) tot = 1m;
                    if (tot < 0m && tot > -1m) tot = -1m;
                    _report.SetValue(acct, interval,
                        new A("{0:#,#}".Fmt(tot)) { href = "/Trns/{0}?Fr={1}&To={2}{3}".Fmt(
                            acct.Path("/"),
                            interval.Start.Date.ToIsoStringOptimal(),
                            interval.End.Date.ToIsoStringOptimal(),
                            acct.EnumChildren().Any() ? "&SubAccts=true" : "") },
                        ReportTable.CssClassNumber(tot));
                }
            }

            if (depth < _maxDepth || _maxDepth == -1)
                foreach (var acctChild in acct.EnumChildren())
                    processAccount(acctChild, depth + 1);
        }
    }
}
