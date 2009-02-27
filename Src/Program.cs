﻿using System;
using System.IO;
using System.Windows.Forms;
using GnuCashSharp;
using RT.Servers;
using RT.Util;
using RT.Util.Dialogs;
using RT.Util.ExtensionMethods;
using System.Linq;
using System.Diagnostics;
using RT.TagSoup.HtmlTags;

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

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Settings = Settings.LoadFromFile(PathUtil.AppPath + "AccountsWeb.xml");
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
        /// <para>Throws an exception if a file is already open.</para>
        /// <para>If there is an exception loading the GnuCash session, displays the message
        /// in a dialog box and does not open a file, but returns successfully.</para>
        /// <para>If the session opens successfully but has warnings, displays a dialog box
        /// with a button to view the warnings in the browser.</para>
        /// </remarks>
        public static void OpenFile(string filename)
        {
            if (CurFile != null)
                throw new RTException("Can't OpenFile because a file is already open.");

            try
            {
                CurFile = GncFileWrapper.LoadFromFile(filename);
                CurFile.ReloadSession();
            }
            catch (Exception e)
            {
                CurFile = null;
                DlgMessage.ShowWarning("Could not open AccountsWeb file.\nWrapper: \"{0}\"\nGnuCash: \"{1}\"\nError: {2}".Fmt(filename, CurFile == null ? "N/A" : (CurFile.GnuCashFile ?? "-"), e.Message));
                return;
            }

            Interface.StartServer();

            //if (Interface.ServerRunning && CurFile.Session != null && CurFile.Session.EnumWarnings().Count() > 0)
            //{
            //    var choice = DlgMessage.ShowWarning("There are warnings related to the GnuCash file \"{0}\".\nWould you like to view them?".Fmt(filename),
            //        "View warnings in browser", "Skip viewing warnings");
            //    if (choice == 0)
            //    {
            //        ProcessStartInfo si = new ProcessStartInfo("http://localhost:{0}/Warnings".Fmt(Interface.ServerPort));
            //        si.UseShellExecute = true;
            //        Process.Start(si);
            //    }
            //}

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
                throw new RTException("Cannot SaveFile because no file is currently open.");

            CurFile.SaveToFile();
        }

        /// <summary>
        /// Closes the currently open file.
        /// </summary>
        /// <remarks>
        /// <para>Throws an exception if no file is open.</para>
        /// <para>Any unsaved changes will be discarded. This method does not display any
        /// prompts or dialogs to the user.</para>
        /// </remarks>
        public static void CloseFile()
        {
            if (CurFile == null)
                throw new RTException("Cannot CloseFile because no file is currently open.");

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
