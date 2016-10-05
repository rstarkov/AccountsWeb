using System;
using System.Collections.Generic;
using System.Linq;
using GnuCashSharp;
using RT.Servers;
using RT.Spinneret;
using RT.TagSoup;
using RT.Util;

namespace AccountsWeb
{
    public class PageTrns : WebPage
    {
        GncAccount _account;
        DateInterval _interval;
        bool _subaccts;

        public PageTrns(HttpRequest request, WebInterface iface)
            : base(request, iface)
        {
        }

        public override string GetTitle()
        {
            return Tr.PgTrns.Title;
        }

        public override object GetContent()
        {
            var minDate = DateTime.MinValue;
            var maxDate = DateTime.MaxValue - TimeSpan.FromDays(1);
            var frDate = Request.GetValidated("Fr", minDate);
            var toDate = Request.GetValidated("To", maxDate, dt => dt >= frDate, Tr.PgTrns.Validation_NotBeforeFr);
            var amtFmt = Request.GetValidated("AmtFmt", "#,0");

            _interval = new DateInterval(frDate.Date, toDate.Date + TimeSpan.FromDays(1) - TimeSpan.FromTicks(1));
            _account = GetAccount("Acct");
            var subAccts = _subaccts = Request.GetValidated("SubAccts", false);
            var showBalance = Request.GetValidated("ShowBal", false, val => !(val && _subaccts), Tr.PgTrns.Validation_ShowBalVsSubAccts);

            _subaccts &= _account.EnumChildren().Any();

            ReportTable table = new ReportTable();
            var colDate = table.AddCol(Tr.PgTrns.ColDate);
            var colDesc = table.AddCol(Tr.PgTrns.ColDescription);
            var colQty = table.AddCol(Tr.PgTrns.ColQuantity);
            var colCcy = table.AddCol(Tr.PgTrns.ColCurrency);
            var colBal = table.AddCol(Tr.PgTrns.ColBalance);
            var colAcct = table.AddCol(Tr.PgTrns.ColAccount);
            var colInBase = table.AddCol(Tr.PgTrns.ColInBaseCcy.Fmt(Program.CurFile.Book.BaseCurrencyId));

            if (!_subaccts)
                table.Cols.Remove(colAcct);
            if (showBalance)
                table.Cols.Remove(colInBase);
            else
                table.Cols.Remove(colBal);

            var splits = _account.EnumSplits(_subaccts);
            splits = splits.Where(split => split.Transaction.DatePosted >= frDate && split.Transaction.DatePosted <= toDate);
            splits = splits.OrderBy(split => split.Transaction);

            foreach (var split in splits)
            {
                var trn = split.Transaction;
                var row = table.AddRow();

                row.SetValue(colDate, trn.DatePosted.ToShortDateString());
                row.SetValue(colDesc, (split.Memo == null || split.Memo == "" || split.Memo == trn.Description)
                    ? trn.Description
                    : (trn.Description + " [" + split.Memo + "]"));
                row.SetValue(colQty, split.Quantity.ToString(amtFmt), ReportTable.CssClassNumber(split.Quantity));
                row.SetValue(colCcy, split.Account.Commodity.Identifier, "ccy_name ccy_name_" + split.Account.Commodity.Identifier.Replace(":", "_"));
                if (!showBalance)
                    row.SetValue(colInBase, split.ConvertAmount(Program.CurFile.Book.BaseCurrency).Quantity.ToString(amtFmt), ReportTable.CssClassNumber(split.Quantity));

                if (showBalance)
                    row.SetValue(colBal, split.AccountBalanceAfter.ToString(amtFmt), ReportTable.CssClassNumber(split.AccountBalanceAfter));

                if (_subaccts)
                    row.SetValue(colAcct, generateAcctPath(split.Account));

                if (split.IsBalsnap)
                    try { row.CssClass = split.AccountBalanceAfter == split.Balsnap ? "balsnap_ok" : "balsnap_wrong"; }
                    catch (GncBalsnapParseException) { row.CssClass = "balsnap_error"; }
            }

            return Ut.NewArray(
                new P(Tr.PgMonthly.CurAccount, GetAccountBreadcrumbs("Acct", _account)),
                new P(Tr.PgTrns.ModeCaption,
                    showBalance && !subAccts ? (object) new SPAN(Tr.PgTrns.ModeWithBalance) { class_ = "aw-current" } : new A(Tr.PgTrns.ModeWithBalance) { href = Request.Url.WithQuery("ShowBal", "true").WithoutQuery("SubAccts").ToHref() },
                    " · ",
                    !showBalance && subAccts ? (object) new SPAN(Tr.PgTrns.ModeWithSubaccts) { class_ = "aw-current" } : new A(Tr.PgTrns.ModeWithSubaccts) { href = Request.Url.WithoutQuery("ShowBal").WithQuery("SubAccts", "true").ToHref() }
                ),
                new P(Tr.PgTrns.RoundingCaption,
                    amtFmt == "#,0" ? (object) new SPAN(Tr.PgTrns.RoundingWhole) { class_ = "aw-current" } : new A(Tr.PgTrns.RoundingWhole) { href = Request.Url.WithoutQuery("AmtFmt").ToHref() },
                    " · ",
                    amtFmt == "#,0.00" ? (object) new SPAN(Tr.PgTrns.RoundingDecimals) { class_ = "aw-current" } : new A(Tr.PgTrns.RoundingDecimals) { href = Request.Url.WithQuery("AmtFmt", "#,0.00").ToHref() }
                ),
                new P(Tr.PgTrns.DateCaption,
                    (frDate == minDate && toDate == maxDate) ? (object) Tr.PgTrns.ShowingAll :
                    (frDate == minDate) ? Tr.PgTrns.ShowingOnOrBefore.FmtEnumerable(
                        (object) Ut.NewArray<object>(new SPAN(toDate.ToShortDateString()) { class_ = "filter_hilite" }, " (", new A(Tr.PgTrns.DateRemove) { href = Request.Url.WithoutQuery("To").ToHref() }, ")")
                    ) :
                    (toDate == maxDate) ? Tr.PgTrns.ShowingOnOrAfter.FmtEnumerable(
                        (object) Ut.NewArray<object>(new SPAN(frDate.ToShortDateString()) { class_ = "filter_hilite" }, " (", new A(Tr.PgTrns.DateRemove) { href = Request.Url.WithoutQuery("Fr").ToHref() }, ")")
                    ) :
                    Tr.PgTrns.ShowingBetween.FmtEnumerable(
                        Ut.NewArray<object>(new SPAN(frDate.ToShortDateString()) { class_ = "filter_hilite" }, " (", new A(Tr.PgTrns.DateRemove) { href = Request.Url.WithoutQuery("Fr").ToHref() }, ")"),
                        Ut.NewArray<object>(new SPAN(toDate.ToShortDateString()) { class_ = "filter_hilite" }, " (", new A(Tr.PgTrns.DateRemove) { href = Request.Url.WithoutQuery("To").ToHref() }, ")")
                    )
                ),
                table.GetHtml()
            );
        }

        private IEnumerable<object> generateAcctPath(GncAccount acct)
        {
            var path = acct.PathAsList(_account);
            if (path.Count == 0)
            {
                yield return new I(Tr.PgTrns.ThisAccount);
            }
            else
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    yield return new A(path[i].Name) { class_ = "nocolor", href = Request.Url.WithQuery("Acct", path[i].Path(":")).ToHref() };
                    yield return " : ";
                }
                if (path.Count > 0)
                {
                    var pathlast = path[path.Count - 1];
                    yield return new A(pathlast.Name) { class_ = "nocolor", href = Request.Url.WithQuery("Acct", pathlast.Path(":")).ToHref() };
                }
            }
        }
    }
}
