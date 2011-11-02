using System.Linq;
using GnuCashSharp;
using RT.Servers;
using RT.Spinneret;
using RT.TagSoup;
using RT.Util;
using RT.Util.ExtensionMethods;

namespace AccountsWeb
{
    public class PageMonthlyTotals : PageMonthly
    {
        public PageMonthlyTotals(HttpRequest request, WebInterface iface)
            : base(request, iface)
        {
            EqatecAnalytics.Monitor.TrackFeature("PageMonthlyTotals.Load");
        }

        public override string GetTitle()
        {
            return Tr.PgMonthlyTotals.Title;
        }

        protected override void ProcessAccount(GncAccount acct, int depth)
        {
            decimal intervalTotal = 0;
            int intervalDays = 0, intervalCount = 0;
            var earliest = acct.Book.EarliestDate;
            var latest = acct.Book.LatestDate;
            foreach (var interval in EnumIntervals())
            {
                decimal tot = acct.GetTotal(interval, true, acct.Book.GetCommodity(acct.Book.BaseCurrencyId));
                if (Negate)
                    tot = -tot;
                // Count only the full months for the purpose of averaging
                if (interval.Start >= earliest && interval.End <= latest)
                {
                    intervalTotal += tot;
                    intervalCount++;
                    intervalDays += (int) (interval.End - interval.Start).TotalDays + 1;
                }

                if (tot == 0)
                    Report.SetValue(acct, interval, "-", ReportTable.CssClassNumber(tot));
                else
                {
                    if (tot > 0m && tot < 1m) tot = 1m;
                    if (tot < 0m && tot > -1m) tot = -1m;
                    Report.SetValue(acct, interval,
                        new A("{0:#,#}".Fmt(tot))
                        {
                            href = "/Trns?Acct={0}&Fr={1}&To={2}{3}".Fmt(
                                acct.Path(":").UrlEscape(),
                                interval.Start.Date.ToIsoStringOptimal(),
                                interval.End.Date.ToIsoStringOptimal(),
                                acct.EnumChildren().Any() ? "&SubAccts=true" : "")
                        },
                        ReportTable.CssClassNumber(tot));
                }
            }

            if (intervalCount > 1)
            {
                if (!Report.ContainsCol("average"))
                    Report.AddCol("average", Tr.PgMonthlyTotals.ColAverage, "aw-col-average");
                intervalTotal = intervalTotal / intervalDays * 30.43m/* * GroupMonths*/;
                if (intervalTotal > 0m && intervalTotal < 1m) intervalTotal = 1m;
                if (intervalTotal < 0m && intervalTotal > -1m) intervalTotal = -1m;
                Report.SetValue(acct, "average",
                    "{0:#,#}".Fmt(intervalTotal),
                    ReportTable.CssClassNumber(intervalTotal));
            }
        }
    }
}
