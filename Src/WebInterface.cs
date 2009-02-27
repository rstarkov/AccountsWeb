using System;
using System.Collections.Generic;
using GnuCashSharp;
using RT.Servers;
using RT.TagSoup.HtmlTags;
using RT.Util.ExtensionMethods;
using RT.Util.Streams;
using System.Linq;
using RT.Util;
using RT.Util.Dialogs;
using RT.Util.Collections;
using System.Text;

namespace AccountsWeb
{
    public class WebInterface
    {
        public HttpServer Server;

        public WebInterface()
        {
        }

        /// <summary>
        /// Attempts to start a server. If the server is already running, throws an exception.
        /// Otherwise returns successfully whether the server was started successfully or not.
        /// Depending on "suppressWarnings" will warn the user if the server could not be started.
        /// The outcome can be determined as "Server.IsListening".
        /// </summary>
        /// <remarks>
        /// <para>Throws an exception if no file is currently open.</para>
        /// </remarks>
        public void StartServer()
        {
            if (ServerRunning)
                throw new RTException("Server is already running.");

            try
            {
                // Create a new server using options for the currenly open file.
                Server = new HttpServer(Program.CurFile.ServerOptions);
                Server.StartListening(false);
                registerHandlers();
            }
            catch
            {
                DlgMessage.ShowWarning("The server could not be started. Try a different port (current port is {0}).".Fmt(Server.Options.Port));
            }
        }

        /// <summary>
        /// If the server is running, stops it. Otherwise does nothing, quietly.
        /// </summary>
        public void StopServer()
        {
            if (Server == null)
                return;
            if (Server.IsListening)
                Server.StopListening(true);
            Server = null;
        }

        public bool ServerRunning
        {
            get { return Server != null && Server.IsListening; }
        }

        public int ServerPort
        {
            get { return Server.Options.Port; }
        }

        public void registerHandlers()
        {
            HttpRequestHandler dummy = req => null;

            // Always served
            addHook(new HttpRequestHandlerHook("/Static", req => Server.FileSystemResponse("Static", req)));
            addHook(new HttpRequestHandlerHook("/About", response_About));

            // Only served if a session is open
            addHook(new HttpRequestHandlerHook(null, null, "/", false, true, dummy), response_Main);
            addHook(new HttpRequestHandlerHook("/Reports/Accounts/MonthlyTotals", dummy), response_MonthlyTotals);
            addHook(new HttpRequestHandlerHook("/ExRates", dummy), response_ExRates);
            addHook(new HttpRequestHandlerHook("/Warnings", dummy), response_Warnings);
        }

        private void addHook(HttpRequestHandlerHook hook)
        {
            Server.RequestHandlerHooks.Add(hook);
        }

        private void addHook(HttpRequestHandlerHook hook, HttpRequestHandler handler)
        {
            HttpRequestHandlerHook actualHook = new HttpRequestHandlerHook(
                hook.Domain, hook.Port, hook.Path, hook.SpecificDomain, hook.SpecificPath,
                req => response_GlobalMessage(req, handler));
            Server.RequestHandlerHooks.Add(actualHook);
        }

        private HttpResponse generatePage_Standard(string title, HtmlTag content, params string[] css)
        {
            var html = new HTML(
                new HEAD(
                    new TITLE(title + " - AccountsWeb"),
                    new LINK() { rel = "stylesheet", type = "text/css", href = "/Static/Basic.css" },
                    css.Select(c => new LINK() { rel = "stylesheet", type = "text/css", href = "/Static/" + c })
                ),
                new BODY(
                    new TABLE() { class_ = "layoutMain" }._(new TR() { class_ = "layoutMain" }._(
                        new TD() { class_ = "layoutLeftPane" }._(
                            new H2("Navigation"),
                            new UL(
                                new LI(new A("Home") { href = "/" }),
                                new LI(new A("Monthly totals") { href = "/Reports/Accounts/MonthlyTotals" }),
                                new LI(new A("Exch. rates") { href = "/ExRates" }),
                                new LI(new A("About") { href = "/About" })
                            ),
                            new H2("Links"),
                            new UL(
                                Program.CurFile.UserLinks.Select(lnk => new LI(new A(lnk.Name) { href = lnk.Href }))
                            )
                        ),
                        new TD() { class_ = "layoutMainPane" }._(
                            content
                        )
                    ))
                )
            );

            return new HttpResponse(html);
        }

        private HttpResponse response_GlobalMessage(HttpRequest request, HttpRequestHandler desiredHandler)
        {
            if (Program.CurFile.GlobalErrorMessage == null)
            {
                return desiredHandler(request);
            }
            else
            {
                var html = new HTML(
                    new HEAD(
                        new TITLE("AccountsWeb - Message"),
                        new LINK() { rel = "stylesheet", type = "text/css", href = "/Static/Basic.css" },
                        new LINK() { rel = "stylesheet", type = "text/css", href = "/Static/Error.css" }
                    ),
                    new BODY(
                        new H1("AccountsWeb message"),
                        new P("AccountsWeb cannot process your request due to the following error:"),
                        new DIV(Program.CurFile.GlobalErrorMessage)
                    )
                );

                return new HttpResponse(html) { Status = HttpStatusCode._503_ServiceUnavailable };
            }
        }

        private HttpResponse response_Main(HttpRequest request)
        {
            return generatePage_Standard("Main",
                    new UL(
                        new LI(new A("Monthly totals") { href = "Reports/Accounts/MonthlyTotals" })
                    )
                );
        }

        private HttpResponse response_About(HttpRequest request)
        {
            return generatePage_Standard("About",
                new DIV(
                    new H1("About AccountsWeb"),
                    new P("Version {0}".Fmt(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version)),
                    new P("Copyright (C) 2009 Roman Starkov")
                )
            );
        }

        private HttpResponse response_Warnings(HttpRequest request)
        {
            return generatePage_Standard("Warnings",
                new DIV(
                    new H1("Warnings"),
                    Program.CurFile.Session.EnumWarnings().Count() == 0
                        ? (HtmlTag) new P("None")
                        : (HtmlTag) new UL(Program.CurFile.Session.EnumWarnings().Select(str => new LI(str)))
                )
            );
        }

        private HttpResponse response_MonthlyTotals(HttpRequest request)
        {
            // Default to the last 12 months
            var toDefault = DateTime.Now;
            var frDefault = toDefault - new TimeSpan(360, 0, 0, 0);
            if (frDefault < Program.CurFile.Book.EarliestDate)
                frDefault = Program.CurFile.Book.EarliestDate;

            var fy = GetAndValidate<int>(request.Get, "FrYr", frDefault.Year);
            var fm = GetAndValidate<int>(request.Get, "FrMo", frDefault.Month, x => x >= 1 && x <= 12, "between 1 and 12");
            var ty = GetAndValidate<int>(request.Get, "ToYr", toDefault.Year, x => x >= fy, "no smaller than the starting year, {0}".Fmt(fy));
            var tm = GetAndValidate<int>(request.Get, "ToMo", toDefault.Month, x => x >= 1 && x <= 12 && (fy < ty || x >= fm), "between 1 and 12, and no smaller than the starting month, {0}".Fmt(fm));
            var maxDepth = GetAndValidate<int>(request.Get, "MaxDepth", -1, x => x >= 0, "non-negative");
            var negate = GetAndValidate<bool>(request.Get, "Neg", false);

            GncAccount acct = null;
            if (request.Get.ContainsKey("Acct"))
                acct = Program.CurFile.Book.GetAccountByPath(request.Get["Acct"]);

            var report = new ReportMonthlyTotals(fy, fm, ty, tm, maxDepth, acct, negate, request);
            var acctrep = report.Generate();

            var html = new DIV(
                (acct == null)
                    ? (object)""
                    : new P(new A("View this account's page") { href = "/Account/" + acct.Path("/") }),
                new P("All values below are in {0}, converted where necessary using ".Fmt(Program.CurFile.BaseCurrency), new A("exchange rates") { href = "/ExRates" }, "." ),
                acctrep.GetContent()
            );

            return generatePage_Standard("Monthly totals", html, "ReportAccounts.css", "MonthlyTotals.css");
        }

        private HttpResponse response_ExRates(HttpRequest request)
        {
            HtmlPrinter prn = new HtmlPrinter(new DIV());

            foreach (var ccy in Program.CurFile.Book.EnumCommodities().OrderBy(cmdty => cmdty.Identifier))
            {
                prn.AddTag(new H1(ccy.Identifier));
                if (ccy.Identifier == Program.CurFile.Book.BaseCurrencyId)
                {
                    prn.AddTag(new P("This is the base currency"));
                }
                else
                {
                    prn.OpenTag(new TABLE());
                    foreach (var pt in ccy.ExRate)
                        prn.AddTag(new TR(new TD(pt.Key.ToShortDateString()), new TD(pt.Value.ToString())));
                    prn.CloseTag();
                }
            }

            return generatePage_Standard("Exchange Rates", prn.GetHtml(), "ExRates.css");
        }

        public static T GetAndValidate<T>(Dictionary<string, string> vars, string varName, T varDefault)
        {
            return GetAndValidate<T>(vars, varName, varDefault, x => true, null);
        }

        public static T GetAndValidate<T>(Dictionary<string, string> vars, string varName, T varDefault, Func<T, bool> validator, string mustBe)
        {
            if (!vars.ContainsKey(varName))
                return varDefault;

            T value;
            try { value = RConvert.Exact<T>(vars[varName]); }
            catch (RConvertException e) { throw new ValidationException(varName, vars[varName], "convertible to {0}".Fmt(typeof(T))); }

            if (!validator(value))
                throw new ValidationException(varName, vars[varName], mustBe);

            return value;
        }

        public class ValidationException: RTException
        {
            public ValidationException(string varName, string varValue, string mustBe)
            {
                _message = "The value of parameter \"{0}\", \"{1}\", is not valid. It must be {2}.".Fmt(varName, varValue, mustBe);
            }
        }

    }

    public static class Extensions
    {
        public static string SameUrlExcept(this HttpRequest request, Dictionary<string, string> qsAddOrReplace, List<string> qsRemove)
        {
            StringBuilder sb = new StringBuilder(request.UrlWithoutQuery);
            char sep = '?';
            foreach (var kvp in request.Get)
            {
                if (qsRemove != null && qsRemove.Contains(kvp.Key))
                    continue;
                sb.Append(sep);
                sb.Append(kvp.Key.UrlEscape());
                sb.Append("=");
                if (qsAddOrReplace.ContainsKey(kvp.Key))
                {
                    sb.Append(qsAddOrReplace[kvp.Key].UrlEscape());
                    qsAddOrReplace.Remove(kvp.Key);
                }
                else
                {
                    sb.Append(kvp.Value.UrlEscape());
                }
                sep = '&';
            }
            foreach (var kvp in qsAddOrReplace)
            {
                sb.Append(sep);
                sb.Append(kvp.Key.UrlEscape());
                sb.Append("=");
                sb.Append(kvp.Value.UrlEscape());
                sep = '&';
            }
            return sb.ToString();
        }
    }

    public class HtmlPrinter
    {
        private Stack<HtmlTag> _stack = new Stack<HtmlTag>();

        public HtmlPrinter(HtmlTag printInto)
        {
            _stack.Push(printInto);
        }

        public void AddTag(object value)
        {
            _stack.Peek().Add(value);
        }

        public void AddTag(object value1, object value2)
        {
            _stack.Peek().Add(value1);
            _stack.Peek().Add(value2);
        }

        public void AddTag(object value1, object value2, object value3, params object[] values)
        {
            _stack.Peek().Add(value1);
            _stack.Peek().Add(value2);
            _stack.Peek().Add(value3);
            foreach (var value in values)
                _stack.Peek().Add(value);
        }

        public void OpenTag(HtmlTag value)
        {
            _stack.Peek().Add(value);
            _stack.Push(value);
        }

        public void CloseTag()
        {
            _stack.Pop();
        }

        public HtmlTag GetHtml()
        {
            var result = _stack.Pop();
            if (_stack.Count != 0)
                throw new RTException("HtmlPrinter found {0} unclosed tags.".Fmt(_stack.Count));
            return result;
        }
    }

    public class ReportMonthlyTotals
    {
        private DateInterval _interval;
        private int _maxDepth;
        private bool _negate;
        private GncAccount _account;
        private ReportAccounts _report;
        private Dictionary<DateInterval, ReportAccounts.Col> _colMap;
        private Dictionary<GncAccount, ReportAccounts.Acct> _acctMap;
        private HttpRequest _request;

        public ReportMonthlyTotals(int fromYear, int fromMonth, int toYear, int toMonth, int maxDepth, GncAccount account, bool negate, HttpRequest request)
        {
            _interval = new DateInterval(fromYear, fromMonth, 1, toYear, toMonth, DateTime.DaysInMonth(toYear, toMonth));
            _maxDepth = maxDepth;
            _account = account ?? Program.CurFile.Book.AccountRoot;
            _negate = negate;
            _request = request;
        }

        public ReportAccounts Generate()
        {
            _report = new ReportAccounts();
            _colMap = new Dictionary<DateInterval, ReportAccounts.Col>();
            _acctMap = new Dictionary<GncAccount, ReportAccounts.Acct>();

            List<ReportAccounts.Col> cols = new List<ReportAccounts.Col>();
            foreach (var interval in _interval.EnumMonths())
            {
                var rCol = new ReportAccounts.Col(interval.Start.ToString("MMM\nyy"));
                _colMap.Add(interval, rCol);
                _report.Cols.Add(rCol);
            }

            foreach (var acctChild in _account.EnumChildren())
                processAccount(acctChild, null, 0);

            return _report;
        }

        private void processAccount(GncAccount acct, ReportAccounts.Acct parent, int depth)
        {
            var rAcct = new ReportAccounts.Acct(acct.Name, parent, _request.SameUrlExcept(new Dictionary<string,string>() { {"Acct", acct.Path(":")} }, null));
            _acctMap.Add(acct, rAcct);
            _report.Accounts.Add(rAcct);

            foreach (var interval in _interval.EnumMonths())
            {
                decimal tot = acct.GetTotal(interval, true, acct.Book.GetCommodity(acct.Book.BaseCurrencyId));
                if (_negate)
                    tot = -tot;
                var value = tot == 0 ? "-" : "{0:# ###}".Fmt(tot);
                _report[rAcct, _colMap[interval]] = new ReportAccounts.Val(value);
            }

            if (depth < _maxDepth)
            {
                foreach (var acctChild in acct.EnumChildren())
                    processAccount(acctChild, rAcct, depth + 1);
            }
        }

    }
}
