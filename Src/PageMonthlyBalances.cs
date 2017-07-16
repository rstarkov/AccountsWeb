using GnuCashSharp;
using RT.Servers;

namespace AccountsWeb
{
    public class PageMonthlyBalances: PageMonthly
    {
        public PageMonthlyBalances(HttpRequest request, WebInterface iface)
            : base(request, iface)
        {
        }

        public override string GetTitle()
        {
            return Tr.PgMonthlyBalances.Title;
        }

        protected override void ProcessAccount(GncAccount acct, int depth)
        {
            var earliest = acct.Book.EarliestDate;
            foreach (var interval in EnumIntervals())
            {
                var bal = ConvertTo == null ? acct.GetBalanceWithSubaccounts(interval.End) : acct.GetBalanceConverted(interval.End, true, ConvertTo);
                if (Negate)
                    bal.NegateInplace();

                SetReportAmount(Report, acct, interval, bal, ConvertTo != null);
            }
        }
    }
}
