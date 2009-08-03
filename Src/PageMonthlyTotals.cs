using System;
using System.Linq;
using GnuCashSharp;
using RT.Servers;
using RT.Spinneret;
using RT.TagSoup.HtmlTags;
using RT.Util.ExtensionMethods;

namespace AccountsWeb
{
    public class PageMonthlyTotals: WebPage
    {
        private DateInterval _interval;
        private ReportAccounts _report;
        private int _maxDepth;
        private bool _negate;
        private GncAccount _account;

        public PageMonthlyTotals(HttpRequest request, WebInterface iface)
            : base(request, iface)
        {
        }

        public override string GetTitle()
        {
            return Tr.PgMonthlyTotals.Title;
        }

        public override object GetContent()
        {
            // Default to the last 12 months
            var earliest = Program.CurFile.Book.EarliestDate;
            var latest = Program.CurFile.Book.LatestDate;
            var toDefault = DateTime.Now;
            var frDefault = toDefault - new TimeSpan(360, 0, 0, 0);
            if (frDefault < earliest)
                frDefault = earliest;

            var fy = Request.GetValidated<int>("FrYr", frDefault.Year);
            var fm = Request.GetValidated<int>("FrMo", frDefault.Month, x => x >= 1 && x <= 12, Tr.PgMonthlyTotals.Validation_Between1and12);
            var ty = Request.GetValidated<int>("ToYr", toDefault.Year, x => x >= fy, Tr.PgMonthlyTotals.Validation_NotSmallerYear.Fmt(fy));
            var tm = Request.GetValidated<int>("ToMo", toDefault.Month, x => x >= 1 && x <= 12 && (fy < ty || x >= fm), Tr.PgMonthlyTotals.Validation_Between1and12_NotSmallerMonth.Fmt(fm));
            _maxDepth = Request.GetValidated<int>("MaxDepth", -1, x => x >= 0, "non-negative");
            _negate = Request.GetValidated<bool>("Neg", false);
            _interval = new DateInterval(fy, fm, 1, ty, tm, DateTime.DaysInMonth(ty, tm));

            _account = GetAccount("Acct");

            _report = new ReportAccounts(_account, Request, true, false);
            foreach (var interval in _interval.EnumMonths())
                _report.AddCol(interval, interval.Start.ToString("MMM\nyy"));
            processAccount(_account, 0);

            // MaxDepth UI
            var maxdepthUi = new P();
            {
                maxdepthUi.Add(Tr.PgMonthlyTotals.SubAcctsDepth);
                for (int i = 0; i <= 5; i++)
                {
                    var label = i == 0 ? Tr.PgMonthlyTotals.SubAcctsNone.Translation : i.ToString();
                    if (_maxDepth == i)
                        maxdepthUi.Add(new SPAN(label) { class_ = "aw-current" });
                    else
                        maxdepthUi.Add(new A(label) { href = Request.SameUrlExceptSet("MaxDepth", i.ToString()) });
                    maxdepthUi.Add(" · ");
                }
                if (_maxDepth == -1)
                    maxdepthUi.Add(new SPAN(Tr.PgMonthlyTotals.SubAcctsAll) { class_ = "aw-current" });
                else
                    maxdepthUi.Add(new A(Tr.PgMonthlyTotals.SubAcctsAll) { href = Request.SameUrlExceptRemove("MaxDepth") });
            }

            var html = new DIV(
                new P(Tr.PgMonthlyTotals.CurAccount, GetAccountBreadcrumbs("Acct", _account)),
                maxdepthUi,
                _report.GetHtml(),
                new P(Tr.PgMonthlyTotals.MessageExRatesUsed.FmtEnumerable(Program.CurFile.BaseCurrency, new A(Tr.PgMonthlyTotals.MessageExRatesUsedLink) { href = "/ExRates" }))
            );

            return html;
        }

        private void processAccount(GncAccount acct, int depth)
        {
            decimal intervalTotal = 0;
            int intervalCount = 0;
            var earliest = acct.Book.EarliestDate;
            var latest = acct.Book.LatestDate;
            foreach (var interval in _interval.EnumMonths())
            {
                decimal tot = acct.GetTotal(interval, true, acct.Book.GetCommodity(acct.Book.BaseCurrencyId));
                if (_negate)
                    tot = -tot;
                // Count only the full months for the purpose of averaging
                if (interval.Start >= earliest && interval.End <= latest)
                {
                    intervalTotal += tot;
                    intervalCount++;
                }

                if (tot == 0)
                    _report.SetValue(acct, interval, "-", ReportTable.CssClassNumber(tot));
                else
                {
                    if (tot > 0m && tot < 1m) tot = 1m;
                    if (tot < 0m && tot > -1m) tot = -1m;
                    _report.SetValue(acct, interval,
                        new A("{0:#,#}".Fmt(tot)) { href = "/Trns?Acct={0}&Fr={1}&To={2}{3}".Fmt(
                            acct.Path(":").UrlEscape(),
                            interval.Start.Date.ToIsoStringOptimal(),
                            interval.End.Date.ToIsoStringOptimal(),
                            acct.EnumChildren().Any() ? "&SubAccts=true" : "") },
                        ReportTable.CssClassNumber(tot));
                }
            }

            if (intervalCount > 1)
            {
                if (!_report.ContainsCol("average"))
                    _report.AddCol("average", Tr.PgMonthlyTotals.ColAverage, "aw-col-average");
                intervalTotal = intervalTotal / intervalCount;
                if (intervalTotal > 0m && intervalTotal < 1m) intervalTotal = 1m;
                if (intervalTotal < 0m && intervalTotal > -1m) intervalTotal = -1m;
                _report.SetValue(acct, "average",
                    "{0:#,#}".Fmt(intervalTotal),
                    ReportTable.CssClassNumber(intervalTotal));
            }

            if (depth < _maxDepth || _maxDepth == -1)
                foreach (var acctChild in acct.EnumChildren())
                    processAccount(acctChild, depth + 1);
        }
    }
}
