using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using RT.Util;
using RT.Servers;
using RT.Util.Dialogs;
using RT.Util.ExtensionMethods;

namespace AccountsWeb
{
    static class Program
    {
        /// <summary>Stores all program settings.</summary>
        public static Settings Settings;

        /// <summary>The main server instance.</summary>
        public static HttpServer Server;

        public static WebInterface Interface;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Settings = Settings.LoadFromFile(PathUtil.AppPath + "WebGnuCash.xml");
            Server = new HttpServer(Settings.ServerOptions);
            Interface = new WebInterface(Server);
            Interface.RegisterHandlers();

            try { Server.StartListening(false); }
            catch { }

            MainForm form = new MainForm();
            Application.Run();

            Settings.SaveToFile(PathUtil.AppPath + "WebGnuCash.xml");
        }

        public static void StartServer()
        {
            try { Server.StartListening(false); }
            catch { DlgMessage.ShowWarning("The server could not be started. Try a different port (current port is {0}).".Fmt(Settings.ServerOptions.Port)); }
        }

        public static void StopServer()
        {
            Server.StopListening();
        }
    }
}
