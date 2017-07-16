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

        public PageBalancesAt(HttpRequest request, WebInterface iface)
            : base(request, iface)
        {
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

        protected override AccountValueInfo GetAccountValue(GncAccount account, int depth)
        {
            return new AccountValueInfo
            {
                Amount = ConvertTo == null ? account.GetBalanceWithSubaccounts(_date) : account.GetBalanceConverted(_date, true, ConvertTo),
            };
        }
    }
}
