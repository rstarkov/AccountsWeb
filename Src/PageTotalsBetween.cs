using GnuCashSharp;
using RT.Servers;
using RT.Spinneret;
using RT.Util.ExtensionMethods;

namespace AccountsWeb;

public class PageTotalsBetween : PageAccountsValue
{
    DateTime _dateFr, _dateTo;

    public PageTotalsBetween(HttpRequest request, WebInterface iface)
        : base(request, iface)
    {
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

    protected override AccountValueInfo GetAccountValue(GncAccount account, int depth)
    {
        var interval = new DateInterval(_dateFr, _dateTo);
        return new AccountValueInfo
        {
            Amount = ConvertTo == null ? account.GetTotalWithSubaccounts(interval) : account.GetTotalConverted(interval, true, ConvertTo),
            Url = "/Trns?Acct={0}&Fr={1}&To={2}{3}".Fmt(
                account.Path(":").UrlEscape(),
                _dateFr.ToIsoStringOptimal(),
                _dateTo.ToIsoStringOptimal(),
                account.EnumChildren().Any() ? "&SubAccts=true" : ""),
        };
    }
}
