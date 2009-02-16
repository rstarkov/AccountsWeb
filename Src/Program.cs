using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using RT.Util;
using RT.Servers;
using RT.Util.Dialogs;
using RT.Util.ExtensionMethods;
using GnuCashSharp;

namespace AccountsWeb
{
    static class Program
    {
        /// <summary>Stores all program settings.</summary>
        public static Settings Settings;

        /// <summary>The Server - accepts connections, sends responses.</summary>
        public static HttpServer Server;

        /// <summary>Web interface is what responds to the requests received by The Server.</summary>
        public static WebInterface Interface;

        /// <summary>Represents the GnuCash file the user is currently working with.</summary>
        public static GncSession Session;

        /// <summary>The form that is responsible for the tray icon and menu</summary>
        public static TrayForm TrayForm;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Load settings
            Settings = Settings.LoadFromFile(PathUtil.AppPath + "WebGnuCash.xml");
            // Create server
            Server = new HttpServer(Settings.ServerOptions);
            // Create web interface
            Interface = new WebInterface(Server);
            Interface.RegisterHandlers();

            StartServer(true);

            TrayForm = new TrayForm();
            Application.Run();

            Settings.SaveToFile(PathUtil.AppPath + "WebGnuCash.xml");
        }

        public static void StartServer()
        {
            StartServer(false);
        }

        public static void StartServer(bool suppressWarnings)
        {
            if (Settings.GnuCashFile == null)
            {
                //DlgMessage.ShowInfo("
            }
            else
            {
                Session = new GncSession();
                Session.LoadFromFile(Settings.GnuCashFile);
            }

            try { Server.StartListening(false); }
            catch { DlgMessage.ShowWarning("The server could not be started. Try a different port (current port is {0}).".Fmt(Settings.ServerOptions.Port)); }
        }

        public static void StopServer()
        {
            Server.StopListening(true);
            Session = null;
        }
    }
}
