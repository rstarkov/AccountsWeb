using System.Collections.Generic;
using GnuCashSharp;
using RT.Servers;
using RT.Spinneret;
using RT.TagSoup;
using RT.Util.ExtensionMethods;

namespace AccountsWeb
{
    public abstract class PageAccountsValue : WebPage
    {
        private string _amtFmt;
        protected ReportAccounts Report;
        protected int MaxDepth;
        protected GncAccount Account;

        public PageAccountsValue(HttpRequest request, WebInterface iface)
            : base(request, iface)
        {
        }

        public abstract string GetColumnCaption();

        public override object GetContent()
        {
            _amtFmt = Request.GetValidated("AmtFmt", "#,0");
            MaxDepth = Request.GetValidated<int>("MaxDepth", -1, x => x >= 0, Tr.PgMonthly.Validation_NonNegative);
            Account = GetAccount("Acct");
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

            var html = new DIV(
                new P(Tr.PgMonthly.CurAccount, GetAccountBreadcrumbs("Acct", Account)),
                new P(maxdepthUi),
                Report.GetHtml(),
                new P(Tr.PgMonthly.MessageExRatesUsed.FmtEnumerable(Program.CurFile.BaseCurrency, new A(Tr.PgMonthly.MessageExRatesUsedLink) { href = "/ExRates" }))
            );

            return html;
        }

        private void doAccount(GncAccount account, int depth)
        {
            var tot = GetAccountValue(account, depth);
            if (tot == 0)
                Report.SetValue(account, GetColumnCaption(), "-", ReportTable.CssClassNumber(tot));
            else
            {
                object content = ("{0:" + _amtFmt + "}").Fmt(tot);
                var url = GetAccountValueUrl(account);
                if (url != null)
                    content = new A(content) { href = url };
                Report.SetValue(account, GetColumnCaption(),
                    content,
                    ReportTable.CssClassNumber(tot));
            }

            if (depth < MaxDepth || MaxDepth == -1)
                foreach (var acctChild in account.EnumChildren())
                    doAccount(acctChild, depth + 1);
        }

        protected virtual void InitRequestOptions() { }
        protected abstract decimal GetAccountValue(GncAccount account, int depth);
        protected virtual string GetAccountValueUrl(GncAccount account) { return null; }
    }

}
