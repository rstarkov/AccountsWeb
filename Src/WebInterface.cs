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
using RT.TagSoup;

namespace AccountsWeb
{
    public class WebInterface
    {
        public HttpServer Server;

        /// <summary>
        /// Attempts to start a server. If the server is already running, stops it first.
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
                StopServer();

            try
            {
                // Create a new server using options for the currenly open file.
                Server = new HttpServer(Program.CurFile.ServerOptions);
                Server.StartListening(false);
                RegisterHandlers();
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

        public void RegisterHandlers()
        {
            HttpRequestHandler dummy = req => null;

            registerHandler("/Static", req => Server.FileSystemResponse("Static", req));

            registerHandler(Server, req => new PageMain(req));

            registerHandler(Server, req => new PageAbout(req));
            registerHandler(Server, req => new PageExRates(req));
            registerHandler(Server, req => new PageMonthlyTotals(req));
            registerHandler(Server, req => new PageTrns(req));
            registerHandler(Server, req => new PageWarnings(req));
        }

        private void registerHandler(string path, HttpRequestHandler func)
        {
            Server.RequestHandlerHooks.Add(new HttpRequestHandlerHook(path, func));
        }

        private void registerHandler(HttpServer server, Func<HttpRequest, Page> pageMaker)
        {
            Page page = pageMaker(null);
            var baseUrl = page.GetBaseUrl();
            server.RequestHandlerHooks.Add(new HttpRequestHandlerHook(null, null, baseUrl, false, baseUrl.EndsWith("/"),
                request => pageMaker(request).GetResponse()));
        }
    }

    public static class Extensions
    {
        public static string SameUrlExcept(this HttpRequest request, Dictionary<string, string> qsAddOrReplace, string[] qsRemove, string resturl)
        {
            StringBuilder sb = new StringBuilder(request.BaseUrl);
            if (resturl == null)
                sb.Append(request.RestUrlWithoutQuery);
            else
                sb.Append(resturl);
            char sep = '?';
            foreach (var kvp in request.Get)
            {
                if (qsRemove != null && qsRemove.Contains(kvp.Key))
                    continue;
                sb.Append(sep);
                sb.Append(kvp.Key.UrlEscape());
                sb.Append("=");
                if (qsAddOrReplace != null && qsAddOrReplace.ContainsKey(kvp.Key))
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
            if (qsAddOrReplace != null)
            {
                foreach (var kvp in qsAddOrReplace)
                {
                    sb.Append(sep);
                    sb.Append(kvp.Key.UrlEscape());
                    sb.Append("=");
                    sb.Append(kvp.Value.UrlEscape());
                    sep = '&';
                }
            }
            return sb.ToString();
        }

        public static string SameUrlExceptSet(this HttpRequest request, params string[] qsAddOrReplace)
        {
            var dict = new Dictionary<string, string>();
            if ((qsAddOrReplace.Length & 1) == 1)
                throw new RTException("Expected an even number of strings - one pair per query string argument");
            for (int i = 0; i < qsAddOrReplace.Length; i += 2)
                dict.Add(qsAddOrReplace[i], qsAddOrReplace[i+1]);
            return request.SameUrlExcept(dict, null, null);
        }

        public static string SameUrlExceptRemove(this HttpRequest request, params string[] qsRemove)
        {
            return request.SameUrlExcept(null, qsRemove, null);
        }

        public static string SameUrlExceptSetRest(this HttpRequest request, string resturl)
        {
            return request.SameUrlExcept(null, null, resturl);
        }
    }
}
