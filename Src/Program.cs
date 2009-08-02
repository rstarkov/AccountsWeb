using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GnuCashSharp;
using RT.Util;
using RT.Util.Dialogs;
using RT.Util.ExtensionMethods;
using RT.Util.Lingo;

namespace AccountsWeb
{
    static class Program
    {
        /// <summary>Stores all program settings.</summary>
        public static Settings Settings;

        /// <summary>This represents the current "open file". Or null, if none.</summary>
        public static GncFileWrapper CurFile;

        /// <summary>Web interface is what responds to the requests received by The Server.</summary>
        public static WebInterface Interface;

        /// <summary>The form that is responsible for the tray icon and menu</summary>
        public static TrayForm TrayForm;

        /// <summary>The currently used translation.</summary>
        public static Translation Tr;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Settings = Settings.LoadFromFile(PathUtil.AppPath + "AccountsWeb.xml");
            Tr = Lingo.LoadTranslation<Translation>("AccountsWeb", ref Settings.Language);

            Interface = new WebInterface();
            TrayForm = new TrayForm();

            if (Settings.LastFileName != null)
                OpenFile(Settings.LastFileName);

            Application.Run();

            Settings.SaveToFile(PathUtil.AppPath + "AccountsWeb.xml");
        }

        #region File loading / unloading

        /// <summary>
        /// Opens a GncFileWrapper of the specified file name. Loads a <see cref="GncSession"/> for
        /// the underlying file.
        /// </summary>
        /// <remarks>
        /// <para>If successful, attempts to start the server.</para>
        /// <para>If there is an exception loading the GnuCash session, displays the message
        /// in a dialog box and does not open a file, but returns successfully.</para>
        /// <para>If the session opens successfully but has warnings, displays a dialog box
        /// with a button to view the warnings in the browser.</para>
        /// </remarks>
        public static void OpenFile(string filename)
        {
            Program.CloseFile();

            try
            {
                CurFile = GncFileWrapper.LoadFromFile(filename);
                CurFile.ReloadSession();
            }
            catch (Exception e)
            {
                CurFile = null;
                DlgMessage.ShowWarning(Tr.Warning_CouldNotOpenAccountsWeb.Fmt(filename, CurFile == null ? Tr.Warning_AccWeb_NA.Translation : (CurFile.GnuCashFile ?? "-"), e.Message));
                return;
            }

            Interface.StartServer(Program.CurFile.ServerOptions);

            AddRecent(filename);
        }

        /// <summary>
        /// Saves the currently opened file. Note: only the <see cref="GncFileWrapper"/> is saved!
        /// The underlying GncSession is not saved.
        /// </summary>
        /// <remarks>
        /// <para>Throws an exception if no file is open.</para>
        /// </remarks>
        public static void SaveFile()
        {
            if (CurFile == null)
                throw new RTException("Internal Error: Cannot SaveFile because no file is currently open.");

            CurFile.SaveToFile();
        }

        /// <summary>
        /// Closes the currently open file.
        /// </summary>
        /// <remarks>
        /// <para>Any unsaved changes will be discarded. This method does not display any
        /// prompts or dialogs to the user.</para>
        /// </remarks>
        public static void CloseFile()
        {
            CurFile = null;
        }

        /// <summary>
        /// Closes the current file, if any, and creates a new one under the specified name.
        /// The new file will have no GnuCash session loaded.
        /// </summary>
        public static void NewFile(string filename)
        {
            if (CurFile != null)
                CloseFile();

            CurFile = new GncFileWrapper();
            CurFile.LoadedFromFile = filename;
            CurFile.SaveToFile();
            CurFile.ReloadSession();

            AddRecent(filename);
        }

        public static void AddRecent(string filename)
        {
            if (Settings.RecentFiles.Contains(filename))
                Settings.RecentFiles.Remove(filename);
            Settings.RecentFiles.Insert(0, filename);
            Settings.LastFileName = filename;
            Settings.SaveToFile();
        }

        #endregion

    }
}
