using System;
using GnuCashSharp;
using RT.Servers;
using RT.Spinneret;
using RT.Util;
using RT.Util.ExtensionMethods;

namespace AccountsWeb
{
    public class PageTotalsBetween : PageAccountsValue
    {
        DateTime _dateFr, _dateTo;

        public PageTotalsBetween(HttpRequest request, WebInterface iface)
            : base(request, iface)
        {
            EqatecAnalytics.Monitor.TrackFeature("PageTotalsBetween.Load");
        }

        public override string GetTitle()
        {
            return Tr.PgTotalsBetween.Title;
        }

        protected override void InitRequestOptions()
        {
            _dateFr = Request.GetValidated("Fr", Program.CurFile.Book.EarliestDate).AssumeUtc();
            _dateTo = Request.GetValidated("To", DateTime.Today, dt => dt >= _dateFr, Tr.PgTrns.Validation_NotBeforeFr).AssumeUtc();
        }

        public override string GetColumnCaption()
        {
            return Tr.PgTotalsBetween.ColCaptionFmt.Fmt(_dateFr.ToIsoStringOptimal(), _dateTo.ToIsoStringOptimal());
        }

        protected override decimal GetAccountValue(GncAccount account, int depth)
        {
            return account.GetTotal(new DateInterval(_dateFr, _dateTo), true, account.Book.BaseCurrency);
        }
    }
}
