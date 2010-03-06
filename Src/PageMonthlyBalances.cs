using System;
using System.Linq;
using GnuCashSharp;
using RT.Servers;
using RT.Spinneret;
using RT.TagSoup.HtmlTags;
using RT.Util.ExtensionMethods;

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
                decimal bal = acct.GetBalance(interval.End, true, acct.Book.GetCommodity(acct.Book.BaseCurrencyId));
                if (Negate)
                    bal = -bal;

                if (bal == 0)
                    Report.SetValue(acct, interval, "-", ReportTable.CssClassNumber(bal));
                else
                {
                    if (bal > 0m && bal < 1m) bal = 1m;
                    if (bal < 0m && bal > -1m) bal = -1m;
                    Report.SetValue(acct, interval,
                        "{0:#,#}".Fmt(bal),
                        ReportTable.CssClassNumber(bal));
                }
            }

            if (depth < MaxDepth || MaxDepth == -1)
                foreach (var acctChild in acct.EnumChildren())
                    ProcessAccount(acctChild, depth + 1);
        }
    }
}
