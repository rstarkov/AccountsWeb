using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Servers;
using RT.TagSoup;
using RT.TagSoup.HtmlTags;
using RT.Util;
using RT.Util.ExtensionMethods;
using GnuCashSharp;

namespace AccountsWeb
{
    public abstract class Page
    {
        protected readonly HttpRequest Request;

        public Page(HttpRequest request)
        {
            Request = request;
            Program.CurFile.ReloadSessionIfNecessary();
        }

        public abstract string GetBaseUrl();
        public abstract string GetTitle();
        public abstract object GetBody();

        public virtual bool IgnoreGlobalMessage
        {
            get { return false; }
        }

        public HttpResponse GetResponse()
        {
            if (Request.OriginIP.Address.ToString() != "127.0.0.1")
                return HttpServer.ErrorResponse(HttpStatusCode._403_Forbidden);

            bool globmsg = Program.CurFile.GlobalErrorMessage != null && !IgnoreGlobalMessage;
            bool fullscr = GetValidated("FullScreen", false);

            Tag head;

            if (globmsg)
            {
                head = new HEAD(
                    new TITLE("Message - AccountsWeb"),
                    new LINK() { rel = "stylesheet", type = "text/css", href = "/Static/" + Program.CurFile.Skin }
                );
            }
            else
            {
                head = new HEAD(
                    new TITLE(GetTitle() + " - AccountsWeb"),
                    new LINK() { rel = "stylesheet", type = "text/css", href = "/Static/" + Program.CurFile.Skin }
                );
            }

            object[] content;
            try
            {
                content = (globmsg) ? new object[] { new H1("AccountsWeb message"), globalMessageBody() }
                                    : new object[] { new H1(GetTitle()), GetBody() };
            }
            catch (Exception e)
            {
                content = GenerateErrorContent("Exception", GenerateExceptionTrace(e));
            }

            Tag body;

            if (fullscr && !globmsg)
            {
                body = new BODY() { class_ = "full_screen" }._(
                    content
                );
            }
            else
            {
                body = new BODY(
                    new TABLE() { class_ = "layoutMain" }._(new TR() { class_ = "layoutMain" }._(
                        new TD() { class_ = "layoutLeftPane" }._(
                            new P(Program.CurFile.FileNameOnly.Replace(".accweb", "")) { class_ = "filename" },
                            new H2("Navigation"),
                            new UL(
                                new LI(new A("Home") { href = "/" }),
                                new LI(new A("Monthly totals") { href = "/MonthlyTotals" }),
                                new LI(new A("Exchange rates") { href = "/ExRates" }),
                                Program.CurFile.Session.EnumWarnings().Any() ? new LI(new A() { href = "/Warnings" }._(new IMG() { src = "/Static/warning_10.png" }, " Warnings")) : null,
                                new LI(new A("About") { href = "/About" })
                            ),
                            new H2("Links"),
                            new UL(
                                Program.CurFile.UserLinks.Select(lnk => new LI(new A(lnk.Name) { href = lnk.Href }))
                            )
                        ),
                        new TD() { class_ = "layoutMainPane" }._(
                            new DIV() { class_ = "fullscrlink" }._(new A("Full screen") { href = Request.SameUrlExceptSet("FullScreen", "true") }),
                            content
                        )
                    ))
                );
            }

            return new HttpResponse(new HTML(head, body))
            {
                Status = globmsg ? HttpStatusCode._503_ServiceUnavailable
                                 : HttpStatusCode._200_OK
            };
        }

        public object[] GenerateExceptionTrace(Exception exception)
        {
            List<object> result = new List<object>();
            while (exception != null)
            {
                result.Add(new H3(exception.GetType().FullName));
                result.Add(new P(exception.Message));
                result.Add(new UL() { class_ = "exception" }._(exception.StackTrace.Split('\n').Select(
                    x => (object) new LI(x)
                )));
                exception = exception.InnerException;
            }
            return result.ToArray();
        }

        public object[] GenerateErrorContent(string title, object content)
        {
            return new object[]
            {
                new H1(title) { class_ = "error" },
                new DIV(content) { class_ = "error" }
            };
        }

        public Tag globalMessageBody()
        {
            return new DIV(
                new P("AccountsWeb cannot process your request due to the following error:"),
                Program.CurFile.GlobalErrorMessage
            );
        }

        public Tag GenerateBreadCrumbs(HttpRequest request, GncAccount acct)
        {
            Tag breadcrumbs;
            var path = acct == null ? null : acct.PathAsList();

            var acct_link = (acct == Program.CurFile.Book.AccountRoot)
                    ? (object) ""
                    : new SPAN(" (", new A("account's page") { href = "/Account/" + acct.Path("/") }, ")");

            if (acct == null || path.Count == 0)
                breadcrumbs = new P() { class_ = "breadcrumbs" }._("> ", new SPAN("Root") { class_ = "breadcrumbs" }, acct_link);
            else
                breadcrumbs = new P() { class_ = "breadcrumbs" }._("> ",
                    new A("Root") { class_ = "breadcrumbs", href = request.SameUrlExceptSetRest("/") }, " > ",
                    path.Take(path.Count - 1).SelectMany(item => new object[] {
                        new A(item.Name) { class_ = "breadcrumbs", href = request.SameUrlExceptSetRest("/" + item.Path("/")) },
                        " : "
                    }),
                    new SPAN(path.Last().Name) { class_ = "breadcrumbs" },
                    acct_link
                );
            return breadcrumbs;
        }

        public T GetValidated<T>(string varName, T varDefault)
        {
            return GetValidated<T>(varName, varDefault, x => true, null);
        }

        public T GetValidated<T>(string varName, T varDefault, Func<T, bool> validator, string mustBe)
        {
            if (!Request.Get.ContainsKey(varName))
                return varDefault;

            T value;
            try { value = RConvert.Exact<T>(Request.Get[varName]); }
            catch (RConvertException) { throw new ValidationException(varName, Request.Get[varName], "convertible to {0}".Fmt(typeof(T))); }

            if (!validator(value))
                throw new ValidationException(varName, Request.Get[varName], mustBe);

            return value;
        }

        public GncAccount GetAccountFromRestUrl()
        {
            var acctpath = Request.RestUrlWithoutQuery.UrlUnescape().Replace("/", ":");
            if (acctpath.StartsWith(":"))
                acctpath = acctpath.Substring(1);
            return Program.CurFile.Book.GetAccountByPath(acctpath);
        }
    }

    public class ValidationException: RTException
    {
        public ValidationException(string varName, string varValue, string mustBe)
        {
            _message = "The value of parameter \"{0}\", \"{1}\", is not valid. It must be {2}.".Fmt(varName, varValue, mustBe);
        }
    }

}
