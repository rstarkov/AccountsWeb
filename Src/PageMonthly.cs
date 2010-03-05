using System;
using GnuCashSharp;
using RT.Servers;
using RT.Spinneret;
using RT.TagSoup.HtmlTags;

namespace AccountsWeb
{
    public abstract class PageMonthly: WebPage
    {
        protected DateInterval Interval;
        protected ReportAccounts Report;
        protected int MaxDepth;
        protected bool Negate;
        protected GncAccount Account;

        public PageMonthly(HttpRequest request, WebInterface iface)
            : base(request, iface)
        {
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
            var fm = Request.GetValidated<int>("FrMo", frDefault.Month, x => x >= 1 && x <= 12, Tr.PgMonthly.Validation_Between1and12);
            var ty = Request.GetValidated<int>("ToYr", toDefault.Year, x => x >= fy, Tr.PgMonthly.Validation_NotSmallerYear.Fmt(fy));
            var tm = Request.GetValidated<int>("ToMo", toDefault.Month, x => x >= 1 && x <= 12 && (fy < ty || x >= fm), Tr.PgMonthly.Validation_Between1and12_NotSmallerMonth.Fmt(fm));
            MaxDepth = Request.GetValidated<int>("MaxDepth", -1, x => x >= 0, "non-negative");
            Negate = Request.GetValidated<bool>("Neg", false);
            Interval = new DateInterval(fy, fm, 1, ty, tm, DateTime.DaysInMonth(ty, tm));

            Account = GetAccount("Acct");

            Report = new ReportAccounts(Account, Request, true, false);
            foreach (var interval in Interval.EnumMonths())
                Report.AddCol(interval, interval.Start.ToString("MMM\nyy"));
            ProcessAccount(Account, 0);

            // MaxDepth UI
            var maxdepthUi = new P();
            {
                maxdepthUi.Add(Tr.PgMonthly.SubAcctsDepth);
                for (int i = 0; i <= 5; i++)
                {
                    var label = i == 0 ? Tr.PgMonthly.SubAcctsNone.Translation : i.ToString();
                    if (MaxDepth == i)
                        maxdepthUi.Add(new SPAN(label) { class_ = "aw-current" });
                    else
                        maxdepthUi.Add(new A(label) { href = Request.SameUrlExceptSet("MaxDepth", i.ToString()) });
                    maxdepthUi.Add(" · ");
                }
                if (MaxDepth == -1)
                    maxdepthUi.Add(new SPAN(Tr.PgMonthly.SubAcctsAll) { class_ = "aw-current" });
                else
                    maxdepthUi.Add(new A(Tr.PgMonthly.SubAcctsAll) { href = Request.SameUrlExceptRemove("MaxDepth") });
            }

            var html = new DIV(
                new P(Tr.PgMonthly.CurAccount, GetAccountBreadcrumbs("Acct", Account)),
                maxdepthUi,
                Report.GetHtml(),
                new P(Tr.PgMonthly.MessageExRatesUsed.FmtEnumerable(Program.CurFile.BaseCurrency, new A(Tr.PgMonthly.MessageExRatesUsedLink) { href = "/ExRates" }))
            );

            return html;
        }

        protected abstract void ProcessAccount(GncAccount account, int depth);
    }
}
