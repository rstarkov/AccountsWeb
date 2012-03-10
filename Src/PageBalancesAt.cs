using System;
using GnuCashSharp;
using RT.Servers;
using RT.Spinneret;
using RT.Util.ExtensionMethods;
using RT.Util;

namespace AccountsWeb
{
    public class PageBalancesAt : PageAccountsValue
    {
        DateTime _date;

        public PageBalancesAt(UrlPathRequest request, WebInterface iface)
            : base(request, iface)
        {
            EqatecAnalytics.Monitor.TrackFeature("PageBalancesAt.Load");
        }

        public override string GetTitle()
        {
            return Tr.PgBalancesAt.Title;
        }

        protected override void InitRequestOptions()
        {
            _date = Request.GetValidated("At", DateTime.Today).AssumeUtc();
        }

        public override string GetColumnCaption()
        {
            return Tr.PgBalancesAt.ColCaptionFmt.Fmt(_date.ToIsoStringOptimal());
        }

        protected override decimal GetAccountValue(GncAccount account, int depth)
        {
            return account.GetBalance(_date, true, account.Book.BaseCurrency);
        }
    }
}
