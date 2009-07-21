using System.Collections.Generic;
using System.Linq;
using GnuCashSharp;
using RT.Servers;
using RT.Spinneret;
using RT.TagSoup.HtmlTags;
using RT.Util;
using RT.Util.ExtensionMethods;

namespace AccountsWeb
{
    public class ReportAccounts
    {
        private GncAccount _baseAcct;
        private HttpRequest _request;
        private bool _autoAddAcct;
        private bool _autoAddCol;

        public ReportTable Table;

        private Dictionary<GncAccount, ReportTable.Row> _rowMap;
        private Dictionary<object, ReportTable.Col> _colMap;
        private ReportTable.Col _colAcctName;

        public ReportAccounts(GncAccount baseacct, HttpRequest request, bool autoAddAcct, bool autoAddCol)
        {
            _baseAcct = baseacct;
            _request = request;
            _autoAddAcct = autoAddAcct;
            _autoAddCol = autoAddCol;
            Table = new ReportTable();
            _rowMap = new Dictionary<GncAccount, ReportTable.Row>();
            _colMap = new Dictionary<object, ReportTable.Col>();
            _colAcctName = Table.AddCol("Account", "acct_name");
        }

        public void AddAcct(GncAccount acct)
        {
            var row = Table.AddRow();
            _rowMap.Add(acct, row);

            row.Depth = acct.Depth - _baseAcct.Depth;

            string indent = "\u2003\u2003".Repeat(acct.Depth - _baseAcct.Depth - 1);
            string name = (acct == _baseAcct) ? ("TOTAL: " + acct.Name) : acct.Name;
            if (acct.EnumChildren().Any())
                row.SetValue(_colAcctName, new object[] { indent, new A(name) { class_ = "nocolor", href = _request.SameUrlExceptSetRest("/" + acct.Path("/")) } });
            else
                row.SetValue(_colAcctName, indent + name);
        }

        public void AddCol(object colref)
        {
            _colMap.Add(colref, Table.AddCol(colref.ToString()));
        }

        public void AddCol(object colref, string title)
        {
            _colMap.Add(colref, Table.AddCol(title));
        }

        public void AddCol(object colref, string title, string cssclass)
        {
            _colMap.Add(colref, Table.AddCol(title, cssclass));
        }

        public void SetValue(GncAccount acct, object colref, object content)
        {
            ensureAcct(acct);
            ensureCol(colref);
            _rowMap[acct].SetValue(_colMap[colref], content);
        }

        public void SetValue(GncAccount acct, object colref, object content, string cssclass)
        {
            ensureAcct(acct);
            ensureCol(colref);
            _rowMap[acct].SetValue(_colMap[colref], content, cssclass);
        }

        private void ensureAcct(GncAccount acct)
        {
            if (_rowMap.ContainsKey(acct))
                return;
            if (_autoAddAcct)
                AddAcct(acct);
            else
                throw new RTException("ReportAccounts: account \"{0}\" not yet defined and AutoAdd is disabled.".Fmt(acct.Path(":")));
        }

        private void ensureCol(object colref)
        {
            if (_colMap.ContainsKey(colref))
                return;
            if (_autoAddCol)
                AddCol(colref);
            else
                throw new RTException("ReportAccounts: column \"{0}\" not yet defined and AutoAdd is disabled.".Fmt(colref));
        }

        public object GetHtml()
        {
            return Table.GetHtml();
        }
    }
}
