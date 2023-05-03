using System;
using System.Linq;
using GnuCashSharp;
using RT.Lingo;
using RT.Servers;
using RT.TagSoup;

namespace AccountsWeb
{
    public class PageLastBalsnap : WebPage
    {
        private ReportAccounts _reportOutOfDate, _reportUpToDate, _reportZeroBalance, _reportNever;
        private GncAccount _account;

        public PageLastBalsnap(HttpRequest request, WebInterface iface)
            : base(request, iface)
        {
        }

        public override string GetTitle()
        {
            return Tr.PgLastBalsnap.Title;
        }

        public override object GetContent()
        {
            _account = GetAccount("Acct");

            _reportOutOfDate = new ReportAccounts(_account, Request, true, true);
            _reportUpToDate = new ReportAccounts(_account, Request, true, true);
            _reportZeroBalance = new ReportAccounts(_account, Request, true, true);
            _reportNever = new ReportAccounts(_account, Request, true, true);
            processAccount(_account, 0);

            return new[]
            {
                new H2("Out of date"),
                _reportOutOfDate.GetHtml(),
                new H2("Up to date"),
                _reportUpToDate.GetHtml(),
                new H2("Zero balance"),
                _reportZeroBalance.GetHtml(),
                new H2("Never"),
                _reportNever.GetHtml(),
            };
        }

        private void processAccount(GncAccount acct, int depth)
        {
            var balsnaps = acct.EnumSplits(false).Where(spl => spl.IsBalsnap).ToArray();

            if (!balsnaps.Any())
                _reportNever.SetValue(acct, Tr.PgLastBalsnap.ColLast, Tr.PgLastBalsnap.LastNever, "lastbalsnap_never");
            else
            {
                var lastsnap = balsnaps.OrderBy(spl => spl.Transaction.DatePosted).Last();
                var lastsplit = acct.EnumSplits(false).Last();
                if (lastsnap.Balsnap == 0 && object.ReferenceEquals(lastsplit, lastsnap) && lastsnap.AccountBalanceAfter == 0)
                    _reportZeroBalance.SetValue(acct, Tr.PgLastBalsnap.ColLast, Tr.PgLastBalsnap.LastZero, "lastbalsnap_zero");
                else
                {
                    int days = (int)(DateTime.Today - lastsnap.Transaction.DatePosted).TotalDays;
                    var rp = days < 35 ? _reportUpToDate : _reportOutOfDate;
                    rp.SetValue(acct, Tr.PgLastBalsnap.ColLast, Tr.PgLastBalsnap.LastNDaysAgo.Fmt(Tr.Language.GetNumberSystem(), days), makeCss(days));
                }
            }

            foreach (var acctChild in acct.EnumChildren())
                processAccount(acctChild, depth + 1);
        }

        private string makeCss(int days)
        {
            if (days < 35)
                return "lastbalsnap_good";
            else if (days < 90)
                return "lastbalsnap_medium";
            else
                return "lastbalsnap_bad";
        }
    }
}