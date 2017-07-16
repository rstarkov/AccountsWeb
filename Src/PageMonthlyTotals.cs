using System.Linq;
using GnuCashSharp;
using RT.Servers;
using RT.Util.ExtensionMethods;

namespace AccountsWeb
{
    public class PageMonthlyTotals : PageMonthly
    {
        public PageMonthlyTotals(HttpRequest request, WebInterface iface)
            : base(request, iface)
        {
        }

        public override string GetTitle()
        {
            return Tr.PgMonthlyTotals.Title;
        }

        protected override void ProcessAccount(GncAccount acct, int depth)
        {
            var intervalTotal = new GncMultiAmount();
            int intervalDays = 0, intervalCount = 0;
            var earliest = acct.Book.EarliestDate;
            var latest = acct.Book.LatestDate;
            foreach (var interval in EnumIntervals())
            {
                var tot = ConvertTo == null ? acct.GetTotalWithSubaccounts(interval) : acct.GetTotalConverted(interval, true, ConvertTo);
                if (Negate)
                    tot.NegateInplace();
                // Count only the full months for the purpose of averaging
                if (interval.Start >= earliest && interval.End <= latest)
                {
                    intervalTotal.AddInplace(tot);
                    intervalCount++;
                    intervalDays += (int) (interval.End - interval.Start).TotalDays + 1;
                }

                SetReportAmount(Report, acct, interval, tot, ConvertTo != null, "/Trns?Acct={0}&Fr={1}&To={2}{3}".Fmt(
                            acct.Path(":").UrlEscape(),
                            interval.Start.Date.ToIsoStringOptimal(),
                            interval.End.Date.ToIsoStringOptimal(),
                            acct.EnumChildren().Any() ? "&SubAccts=true" : ""));
            }

            if (intervalCount > 1)
            {
                if (!Report.ContainsCol("average"))
                    Report.AddCol("average", Tr.PgMonthlyTotals.ColAverage, "aw-col-average");
                intervalTotal.MultiplyInplace(30.43m / intervalDays); // daily average * days in month
                SetReportAmount(Report, acct, "average", intervalTotal, ConvertTo != null);
            }
        }
    }
}
