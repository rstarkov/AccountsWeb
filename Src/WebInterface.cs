using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Servers;
using System.IO;
using RT.TagSoup;
using RT.TagSoup.XhtmlTags;
using RT.Util.Streams;
using GnuCashSharp;
using RT.Util.ExtensionMethods;

namespace AccountsWeb
{
    public class WebInterface
    {
        private HttpServer _server;

        public WebInterface(HttpServer server)
        {
            _server = server;
        }

        public void RegisterHandlers()
        {
            _server.RequestHandlerHooks.Add(new HttpRequestHandlerHook(null, null, "/", false, true, response_Main));
            _server.RequestHandlerHooks.Add(new HttpRequestHandlerHook("/Static", req => _server.FileSystemResponse("Static", req)));
            _server.RequestHandlerHooks.Add(new HttpRequestHandlerHook("/Accounts", response_Accounts));
            _server.RequestHandlerHooks.Add(new HttpRequestHandlerHook("/Reports/Accounts/MonthlyTurnover", response_MonthlyTurnover));
        }

        private HttpResponse response_Main(HttpRequest request)
        {
            var html = new html(
                new head(),
                new body(
                    new ul(
                        new li(
                            new a("Monthly turnover") { href = "Reports/Accounts/MonthlyTurnover" }))));

            return new HttpResponse()
            {
                Status = HttpStatusCode._200_OK,
                Headers = new HttpResponseHeaders() { ContentType = "text/html; charset=utf-8" },
                Content = new DynamicContentStream(html.ToEnumerable())
            };
        }

        private HttpResponse response_Accounts(HttpRequest request)
        {
            throw new NotImplementedException();
        }

        private HttpResponse response_MonthlyTurnover(HttpRequest request)
        {
            var report = new ReportMonthlyTurnover(new DateInterval(2008, 01, 01, 2009, 01, 31));
            var acctrep = report.Generate();
            return acctrep.Respond(request);
        }
    }

    public class ReportMonthlyTurnover
    {
        private DateInterval _interval;
        private ReportAccounts _report;
        private Dictionary<DateInterval, ReportAccounts.Col> _colMap;
        private Dictionary<GncAccount, ReportAccounts.Acct> _acctMap;

        public ReportMonthlyTurnover(DateInterval interval)
        {
            _interval = interval;
        }

        public ReportAccounts Generate()
        {
            _report = new ReportAccounts();
            _colMap = new Dictionary<DateInterval, ReportAccounts.Col>();
            _acctMap = new Dictionary<GncAccount, ReportAccounts.Acct>();

            List<ReportAccounts.Col> cols = new List<ReportAccounts.Col>();
            foreach (var interval in _interval.EnumMonths())
            {
                var rCol = new ReportAccounts.Col(interval.Start.ToString("MMM yy"));
                _colMap.Add(interval, rCol);
                _report.Cols.Add(rCol);
            }

            foreach (var acctChild in Program.Session.Book.AccountRoot.EnumChildren())
                processAccount(acctChild, null);

            return _report;
        }

        private void processAccount(GncAccount acct, ReportAccounts.Acct parent)
        {
            var rAcct = new ReportAccounts.Acct(acct.Name, parent);
            _acctMap.Add(acct, rAcct);
            _report.Accounts.Add(rAcct);

            foreach (var interval in _interval.EnumMonths())
            {
                decimal tot = acct.GetTotalDebit(interval) - acct.GetTotalCredit(interval);
                var value = tot == 0 ? "-" : "{0:0}".Fmt(tot);
                _report[rAcct, _colMap[interval]] = new ReportAccounts.Val(value);
            }

            foreach (var acctChild in acct.EnumChildren())
                processAccount(acctChild, rAcct);
        }

    }
}
