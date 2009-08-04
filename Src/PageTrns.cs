using System;
using System.Collections.Generic;
using System.Linq;
using GnuCashSharp;
using RT.Servers;
using RT.Spinneret;
using RT.TagSoup.HtmlTags;
using RT.Util.ExtensionMethods;
using RT.TagSoup;

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
            _subaccts = Request.GetValidated("SubAccts", false);
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
                row.SetValue(colCcy, split.Account.Commodity, "ccy_name ccy_name_" + split.Account.Commodity);
                if (!showBalance)
                    row.SetValue(colInBase, split.Amount.ConvertTo(Program.CurFile.Book.BaseCurrency).Quantity.ToString(amtFmt), ReportTable.CssClassNumber(split.Quantity));

                if (showBalance)
                    row.SetValue(colBal, split.AccountBalanceAfter.ToString(amtFmt), ReportTable.CssClassNumber(split.AccountBalanceAfter));

                if (_subaccts)
                    row.SetValue(colAcct, generateAcctPath(split.Account));

                if (split.IsBalsnap)
                    try { row.CssClass = split.AccountBalanceAfter == split.Balsnap ? "balsnap_ok" : "balsnap_wrong"; }
                    catch (GncBalsnapParseException) { row.CssClass = "balsnap_error"; }
            }

            DIV filterInfo = new DIV() { class_ = "filter_info" };
            if (frDate == minDate && toDate == maxDate)
                filterInfo.Add(new P(Tr.PgTrns.ShowingAll));
            else if (frDate == minDate)
                filterInfo.Add(new P(Tr.PgTrns.ShowingOnOrBefore.FmtEnumerable(new SPAN(toDate.ToShortDateString()) { class_ = "filter_hilite" })));
            else if (toDate == maxDate)
                filterInfo.Add(new P(Tr.PgTrns.ShowingOnOrAfter.FmtEnumerable(new SPAN(frDate.ToShortDateString()) { class_ = "filter_hilite" })));
            else
                filterInfo.Add(new P(Tr.PgTrns.ShowingBetween.FmtEnumerable(new SPAN(frDate.ToShortDateString()) { class_ = "filter_hilite" }, new SPAN(toDate.ToShortDateString()) { class_ = "filter_hilite" })));
            filterInfo.Add(new P(Tr.PgTrns.ShowingSubaccts, new SPAN(_subaccts ? Tr.PgTrns.ShowingSubacctsYes : Tr.PgTrns.ShowingSubacctsNo) { class_ = "filter_hilite" }));

            return new object[]
            {
                filterInfo,
                table.GetHtml()
            };
        }

        private IEnumerable<object> generateAcctPath(GncAccount acct)
        {
            var path = acct.PathAsList(_account);
            for (int i = 0; i < path.Count - 1; i++)
            {
                yield return new A(path[i].Name) { class_ = "nocolor", href = Request.SameUrlExceptSet("Acct", path[i].Path(":")) };
                yield return " : ";
            }
            if (path.Count > 0)
            {
                var pathlast = path[path.Count - 1];
                yield return new A(pathlast.Name) { class_ = "nocolor", href = Request.SameUrlExceptSet("Acct", pathlast.Path(":")) };
            }
        }
    }
}
