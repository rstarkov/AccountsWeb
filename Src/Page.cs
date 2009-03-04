using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Servers;
using RT.TagSoup;
using RT.TagSoup.HtmlTags;
using RT.Util;
using RT.Util.ExtensionMethods;

namespace AccountsWeb
{
    public abstract class Page
    {
        protected readonly HttpRequest Request;

        public Page(HttpRequest request)
        {
            Request = request;
        }

        public abstract string GetBaseUrl();
        public abstract string GetTitle();
        public abstract IEnumerable<string> GetCss();
        public abstract IEnumerable<Tag> GetBody();

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
                    new LINK() { rel = "stylesheet", type = "text/css", href = "/Static/Basic.css" },
                    new LINK() { rel = "stylesheet", type = "text/css", href = "/Static/Error.css" }
                );
            }
            else if (fullscr)
            {
                head = new HEAD(
                    new TITLE(GetTitle() + " - AccountsWeb"),
                    new LINK() { rel = "stylesheet", type = "text/css", href = "/Static/Basic.css" },
                    new LINK() { rel = "stylesheet", type = "text/css", href = "/Static/BasicFullScreen.css" },
                    GetCss().Select(c => new LINK() { rel = "stylesheet", type = "text/css", href = "/Static/" + c })
                );
            }
            else
            {
                head = new HEAD(
                    new TITLE(GetTitle() + " - AccountsWeb"),
                    new LINK() { rel = "stylesheet", type = "text/css", href = "/Static/Basic.css" },
                    GetCss().Select(c => new LINK() { rel = "stylesheet", type = "text/css", href = "/Static/" + c })
                );
            }

            object[] content =
                (globmsg) ? new object[] { new H1("AccountsWeb message"), globalMessageBody() }
                          : new object[] { new H1(GetTitle()), GetBody() };

            Tag body;

            if (fullscr && !globmsg)
            {
                body = new BODY(
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
                                new LI(new A("Exch. rates") { href = "/ExRates" }),
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

        public Tag globalMessageBody()
        {
            return new DIV(
                new P("AccountsWeb cannot process your request due to the following error:"),
                Program.CurFile.GlobalErrorMessage
            );
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
    }

    public class ValidationException: RTException
    {
        public ValidationException(string varName, string varValue, string mustBe)
        {
            _message = "The value of parameter \"{0}\", \"{1}\", is not valid. It must be {2}.".Fmt(varName, varValue, mustBe);
        }
    }

}
