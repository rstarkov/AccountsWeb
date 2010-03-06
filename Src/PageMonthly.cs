using System;
using System.Collections.Generic;
using System.Linq;
using GnuCashSharp;
using RT.Servers;
using RT.Spinneret;
using RT.TagSoup.HtmlTags;
using RT.Util.ExtensionMethods;

namespace AccountsWeb
{
    public abstract class PageMonthly : WebPage
    {
        private DateInterval Interval;
        protected int GroupMonths;
        protected ReportAccounts Report;
        protected int MaxDepth;
        protected bool Negate;
        protected GncAccount Account;

        public PageMonthly(HttpRequest request, WebInterface iface)
            : base(request, iface)
        {
        }

        protected IEnumerable<DateInterval> EnumIntervals()
        {
            var intervals = Interval.EnumMonths().ToArray();
            for (int i = (intervals.Length - 1) % GroupMonths + 1 - GroupMonths; i < intervals.Length; i += GroupMonths)
                yield return new DateInterval(intervals[Math.Max(0, i)].Start, intervals[i + GroupMonths - 1].End);
        }

        public override object GetContent()
        {
            GroupMonths = Request.GetValidated<int>("Group", 1, x => x >= 1, Tr.PgMonthly.Validation_1OrGreater);
            int pastYears = Request.GetValidated<int>("Years", 1, x => x >= 1, Tr.PgMonthly.Validation_1OrGreater);
            int pastGroups = (12 * pastYears) / GroupMonths + 1;

            // Default to the last N months such that we see M whole groups, where M = number of groups per year + 1
            var earliest = Program.CurFile.Book.EarliestDate;
            var toDefault = DateTime.Today == DateTime.Today.EndOfMonth() ? DateTime.Today : DateTime.Today.AddMonths(-1).EndOfMonth().AssumeUtc();
            var frDefault = toDefault.AddMonths(1 - pastGroups * GroupMonths).StartOfMonth().AssumeUtc();
            if (frDefault < earliest)
                frDefault = earliest;

            var fy = Request.GetValidated<int>("FrYr", frDefault.Year);
            var fm = Request.GetValidated<int>("FrMo", frDefault.Month, x => x >= 1 && x <= 12, Tr.PgMonthly.Validation_Between1and12);
            var ty = Request.GetValidated<int>("ToYr", toDefault.Year, x => x >= fy, Tr.PgMonthly.Validation_NotSmallerYear.Fmt(fy));
            var tm = Request.GetValidated<int>("ToMo", toDefault.Month, x => x >= 1 && x <= 12 && (fy < ty || x >= fm), Tr.PgMonthly.Validation_Between1and12_NotSmallerMonth.Fmt(fm));
            MaxDepth = Request.GetValidated<int>("MaxDepth", -1, x => x >= 0, Tr.PgMonthly.Validation_NonNegative);
            Negate = Request.GetValidated<bool>("Neg", false);
            Interval = new DateInterval(fy, fm, 1, ty, tm, DateTime.DaysInMonth(ty, tm));

            Account = GetAccount("Acct");

            Report = new ReportAccounts(Account, Request, true, false);
            foreach (var interval in EnumIntervals())
            {
                double months = interval.TotalMonths;
                string capt = (months <= 1) ? interval.Start.ToString("MMM\nyy") : (interval.Start.ToString("MMMyy\n") + Tr.PgMonthly.MonthGroupJoiner + interval.End.ToString("\nMMMyy"));
                if (months != GroupMonths)
                    capt += "\n({0:0.#} {1})".Fmt(months, Tr.PgMonthly.MoSuffix);
                Report.AddCol(interval, capt);
            }
            ProcessAccount(Account, 0);

            // MaxDepth UI
            var maxdepthUi = new List<object>();
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

            // Group months UI
            var groupMonthsUi = new List<object>();
            {
                groupMonthsUi.Add(Tr.PgMonthly.GroupMonthsCount);
                foreach (var len in new[] { 1, 2, 3, 4, 6, 12 })
                {
                    if (len != 1)
                        groupMonthsUi.Add(" · ");
                    if (GroupMonths == len)
                        groupMonthsUi.Add(new SPAN(len) { class_ = "aw-current" });
                    else if (len == 1)
                        groupMonthsUi.Add(new A(len) { href = Request.SameUrlExceptRemove("Group") });
                    else
                        groupMonthsUi.Add(new A(len) { href = Request.SameUrlExceptSet("Group", len.ToString()) });
                }
            }

            // Years to show UI
            var yearsUi = new List<object>();
            {
                yearsUi.Add(Tr.PgMonthly.PastYears);
                foreach (var yrs in Enumerable.Range(1, (int) Math.Ceiling((DateTime.Now - Program.CurFile.Book.EarliestDate).TotalDays / 365)))
                {
                    if (yrs != 1)
                        yearsUi.Add(" · ");
                    if (pastYears == yrs)
                        yearsUi.Add(new SPAN(Tr.PgMonthly.YearsMonths.Fmt(Tr.Ns, yrs, GroupMonths)) { class_ = "aw-current" });
                    else if (yrs == 1)
                        yearsUi.Add(new A(yrs) { href = Request.SameUrlExceptRemove("Years") });
                    else
                        yearsUi.Add(new A(yrs) { href = Request.SameUrlExceptSet("Years", yrs.ToString()) });
                }
            }

            // Mode
            var modeUi = new List<object>();
            if (this is PageMonthlyTotals)
                modeUi.Add(new SPAN(Tr.PgMonthly.ViewModeTotals) { class_ = "aw-current" });
            else
                modeUi.Add(new A(Tr.PgMonthly.ViewModeTotals) { href = "/MonthlyTotals" + (Request.Query == null ? "" : ("?" + Request.Query)) });
            modeUi.Add(" · ");
            if (this is PageMonthlyBalances)
                modeUi.Add(new SPAN(Tr.PgMonthly.ViewModeBalances) { class_ = "aw-current" });
            else
                modeUi.Add(new A(Tr.PgMonthly.ViewModeBalances) { href = "/MonthlyBalances" + (Request.Query == null ? "" : ("?" + Request.Query)) });

            var html = new DIV(
                new P(Tr.PgMonthly.CurAccount, GetAccountBreadcrumbs("Acct", Account)),
                new P(maxdepthUi),
                new P(groupMonthsUi, new RAWHTML("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"), yearsUi),
                new P(modeUi),
                Report.GetHtml(),
                new P(Tr.PgMonthly.MessageExRatesUsed.FmtEnumerable(Program.CurFile.BaseCurrency, new A(Tr.PgMonthly.MessageExRatesUsedLink) { href = "/ExRates" }))
            );

            return html;
        }

        protected abstract void ProcessAccount(GncAccount account, int depth);
    }
}
