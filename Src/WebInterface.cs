using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Servers;
using System.IO;
using RT.TagSoup;
using RT.TagSoup.XhtmlTags;
using RT.Util.Streams;

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
            _server.RequestHandlerHooks.Add(new HttpRequestHandlerHook("/Accounts", response_Accounts));
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

            return new HttpResponse();
        }
    }
}
