using GnuCashSharp;
using RT.Servers;
using RT.Spinneret;
using RT.TagSoup;
using RT.Util.ExtensionMethods;

namespace AccountsWeb;

public abstract class PageAccountsValue : WebPage
{
    protected ReportAccounts Report;
    protected int MaxDepth;
    protected GncAccount Account;
    protected GncCommodity ConvertTo;

    public PageAccountsValue(HttpRequest request, WebInterface iface)
        : base(request, iface)
    {
    }

    public abstract string GetColumnCaption();

    public override object GetContent()
    {
        MaxDepth = Request.GetValidated<int>("MaxDepth", -1, x => x >= 0, Tr.PgMonthly.Validation_NonNegative);
        Account = GetAccount("Acct");
        {
            var ccys = Program.CurFile.Book.EnumCommodities();
            var ccy = Request.GetValidated<string>("Ccy", null, x => x == null || ccys.Any(c => c.Identifier == x), Tr.PgMonthly.Validation_OneOfCommodities.Fmt(ccys.Select(c => c.Identifier).Order().JoinString(", ")));
            ConvertTo = ccy == null ? null : ccys.Single(c => c.Identifier == ccy);
        }
        InitRequestOptions();

        Report = new ReportAccounts(Account, Request, true, false);

        Report.AddCol(GetColumnCaption(), GetColumnCaption());

        doAccount(Account, 0);

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
                    maxdepthUi.Add(new A(label) { href = Request.Url.WithQuery("MaxDepth", i.ToString()).ToHref() });
                maxdepthUi.Add(" · ");
            }
            if (MaxDepth == -1)
                maxdepthUi.Add(new SPAN(Tr.PgMonthly.SubAcctsAll) { class_ = "aw-current" });
            else
                maxdepthUi.Add(new A(Tr.PgMonthly.SubAcctsAll) { href = Request.Url.WithoutQuery("MaxDepth").ToHref() });
        }

        // ConvertTo currency UI
        var convertToUI = new List<object>();
        {
            var ccys = Program.CurFile.Book.EnumCommodities().OrderBy(c => c.Identifier);
            convertToUI.Add(Program.Tr.PgMonthly.ConvertTo);
            foreach (var ccy in ((GncCommodity) null).Concat(ccys))
            {
                if (ccy != null)
                    convertToUI.Add(" · ");
                if (ConvertTo == ccy)
                    convertToUI.Add(new SPAN(ccy?.Identifier ?? Program.Tr.PgMonthly.ConvertToNone) { class_ = "aw-current" });
                else if (ccy == null)
                    convertToUI.Add(new A(Program.Tr.PgMonthly.ConvertToNone) { href = Request.Url.WithoutQuery("Ccy").ToHref() });
                else
                    convertToUI.Add(new A(ccy) { href = Request.Url.WithQuery("Ccy", ccy.Identifier).ToHref() });
            }
        }

        var html = new DIV(
            new P(Tr.PgMonthly.CurAccount, GetAccountBreadcrumbs("Acct", Account)),
            new P(maxdepthUi),
            new P(convertToUI),
            Report.GetHtml()
        );

        return html;
    }

    private void doAccount(GncAccount account, int depth)
    {
        var value = GetAccountValue(account, depth);

        SetReportAmount(Report, account, GetColumnCaption(), value.Amount, ConvertTo != null, value.Url);

        if (depth < MaxDepth || MaxDepth == -1)
            foreach (var acctChild in account.EnumChildren())
                doAccount(acctChild, depth + 1);
    }

    protected virtual void InitRequestOptions() { }
    protected abstract AccountValueInfo GetAccountValue(GncAccount account, int depth);

    protected class AccountValueInfo
    {
        public GncMultiAmount Amount;
        public string Url;
    }
}
