using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.TagSoup;
using RT.Servers;
using GnuCashSharp;
using RT.TagSoup.HtmlTags;
using RT.Util.ExtensionMethods;

namespace AccountsWeb
{
    public class PageTrns: Page
    {
        GncAccount _account;
        DateInterval _interval;
        bool _subaccts;

        public PageTrns(HttpRequest request)
            : base(request)
        {
        }

        public override string GetBaseUrl()
        {
            return "/Trns";
        }

        public override string GetTitle()
        {
            return "Transactions";
        }

        public override object GetBody()
        {
            var minDate = DateTime.MinValue;
            var maxDate = DateTime.MaxValue - TimeSpan.FromDays(1);
            var frDate = GetValidated("Fr", minDate);
            var toDate = GetValidated("To", maxDate, dt => dt >= frDate, "no earlier than the \"Fr\" date");
            var amtFmt = GetValidated("AmtFmt", "#,0");
            
            _interval = new DateInterval(frDate.Date, toDate.Date + TimeSpan.FromDays(1) - TimeSpan.FromTicks(1));
            _account = GetAccountFromRestUrl();
            _subaccts = GetValidated<bool>("SubAccts", false) && _account.EnumChildren().Any();

            ReportTable table = new ReportTable();
            var colDate = table.AddCol("Date");
            var colDesc = table.AddCol("Description");
            var colQty = table.AddCol("Qty");
            var colCcy = table.AddCol("Ccy");
            var colAcct = table.AddCol("Account");
            var colInBase = table.AddCol("In " + Program.CurFile.Book.BaseCurrencyId);

            if (!_subaccts)
                table.Cols.Remove(colAcct);

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
                row.SetValue(colInBase, split.Amount.ConvertTo(Program.CurFile.Book.BaseCurrency).Quantity.ToString(amtFmt), ReportTable.CssClassNumber(split.Quantity));

                if (_subaccts)
                    row.SetValue(colAcct, generateAcctPath(split.Account));
            }

            HtmlPrinter filterInfo = new HtmlPrinter(new DIV() { class_ = "filter_info" });
            if (frDate == minDate && toDate == maxDate)
                filterInfo.AddTag(new P("Showing all transactions."));
            else if (frDate == minDate)
                filterInfo.AddTag(new P("Showing all transactions on or before ", new SPAN(toDate.ToShortDateString()) { class_ = "filter_hilite" }, "."));
            else if (toDate == maxDate)
                filterInfo.AddTag(new P("Showing all transactions on or after ", new SPAN(frDate.ToShortDateString()) { class_ = "filter_hilite" }, "."));
            else
                filterInfo.AddTag(new P("Showing transactions between ", new SPAN(frDate.ToShortDateString()) { class_ = "filter_hilite" }, " and ", new SPAN(toDate.ToShortDateString()) { class_ = "filter_hilite" }, ", inclusive."));
            filterInfo.AddTag(new P("Subaccounts shown: ", new SPAN(_subaccts ? "yes" : "no") { class_ = "filter_hilite" }));

            return new object[]
            {
                GenerateBreadCrumbs(Request, _account),
                filterInfo.GetHtml(),
                table.GetHtml()
            };
        }

        private IEnumerable<object> generateAcctPath(GncAccount acct)
        {
            var path = acct.PathAsList(_account);
            for (int i = 0; i < path.Count - 1; i++)
            {
                yield return new A(path[i].Name) { class_ = "nocolor", href = Request.SameUrlExceptSetRest("/" + path[i].Path("/")) };
                yield return " : ";
            }
            if (path.Count > 0)
            {
                var pathlast = path[path.Count - 1];
                yield return new A(pathlast.Name) { class_ = "nocolor", href = Request.SameUrlExceptSetRest("/" + pathlast.Path("/")) };
            }
        }
    }
}
