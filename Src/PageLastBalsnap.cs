using System;
using System.Linq;
using GnuCashSharp;
using RT.Servers;
using RT.TagSoup.HtmlTags;

namespace AccountsWeb
{
    public class PageLastBalsnap : WebPage
    {
        private ReportAccounts _report;
        private GncAccount _account;

        public PageLastBalsnap(HttpRequest request, WebInterface iface)
            : base(request, iface)
        {
        }

        public override string GetTitle()
        {
            return "Last Balance Snapshot";
        }

        public override object GetContent()
        {
            _account = GetAccount("Acct");

            _report = new ReportAccounts(_account, Request, true, true);
            processAccount(_account, 0);

            return new DIV(_report.GetHtml());
        }

        private void processAccount(GncAccount acct, int depth)
        {
            var balsnaps = acct.EnumSplits(false).Where(spl => spl.IsBalsnap).ToArray();

            if (!balsnaps.Any())
                _report.SetValue(acct, "Last", "never", "lastbalsnap_never");
            else
            {
                var lastsnap = balsnaps.OrderBy(spl => spl.Transaction.DatePosted).Last();
                var lastsplit = acct.EnumSplits(false).Last();
                if (lastsnap.Balsnap == 0 && object.ReferenceEquals(lastsplit, lastsnap) && lastsnap.AccountBalanceAfter == 0)
                    _report.SetValue(acct, "Last", "balance = 0", "lastbalsnap_zero");
                else
                {
                    int days = (int) (DateTime.Today - lastsnap.Transaction.DatePosted).TotalDays;
                    _report.SetValue(acct, "Last", days + " days ago", makeCss(days));
                }
            }

            foreach (var acctChild in acct.EnumChildren())
                processAccount(acctChild, depth + 1);
        }

        private string makeCss(int days)
        {
            if (days < 7)
                return "lastbalsnap_good";
            else if (days < 31)
                return "lastbalsnap_medium";
            else
                return "lastbalsnap_bad";
        }
    }
}
