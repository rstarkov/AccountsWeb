using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Servers;
using RT.Util.XmlClassify;
using GnuCashSharp;
using RT.TagSoup.HtmlTags;
using System.IO;
using RT.Util.ExtensionMethods;
using RT.Util;

namespace AccountsWeb
{
    public class GncFileWrapper
    {
        public string GnuCashFile;
        public HttpServerOptions ServerOptions;
        public string BaseCurrency;
        public string BalsnapPrefix = "BALANCE:";
        public string Skin;
        public List<UserLink> UserLinks;

        /// <summary>
        /// If not null, every web request will respond with this error message.
        /// </summary>
        [XmlIgnore]
        public HtmlTag GlobalErrorMessage = null;

        /// <summary>
        /// Holds the currently opened GnuCash Session.
        /// "null" if the session hasn't been loaded (eg no GnuCashFile is configured).
        /// </summary>
        [XmlIgnore]
        public GncSession Session;

        /// <summary>
        /// Holds the main book from the currently opened GnuCash <see cref="Session"/>.
        /// </summary>
        [XmlIgnore]
        public GncBook Book;

        /// <summary>
        /// Holds the name of the file this class was loaded from.
        /// "null" if this is a new file that has never been saved.
        /// </summary>
        [XmlIgnore]
        public string LoadedFromFile;

        public GncFileWrapper()
        {
            ServerOptions = new HttpServerOptions();
            ServerOptions.Port = 1771;
            BaseCurrency = "USD";
            Skin = "SnowWhite.css";
            UserLinks = new List<UserLink>() { new UserLink() { Name = "Example", Href = "/MonthlyTotals?MaxDepth=0" } };
        }

        public void SaveToFile(string filename)
        {
            LoadedFromFile = filename;
            XmlClassify.SaveObjectToXmlFile(this, filename);
        }

        public void SaveToFile()
        {
            SaveToFile(LoadedFromFile);
        }

        public static GncFileWrapper LoadFromFile(string filename)
        {
            var wrapper = XmlClassify.LoadObjectFromXmlFile<GncFileWrapper>(filename);
            wrapper.LoadedFromFile = filename;
            return wrapper;
        }

        /// <summary>
        /// Reloads the <see cref="GncSession"/> underlying the current open file.
        /// </summary>
        /// <remarks>
        /// <para>Throws an exception if no file is open.</para>
        /// <para>Does not throw any exceptions on any other error - instead configures
        /// a <see cref="GlobalErrorMessage"/> to explain the problem.</para>
        /// </remarks>
        public void ReloadSession()
        {
            GlobalErrorMessage = null;

            if (GnuCashFile == null || GnuCashFile == "")
                GlobalErrorMessage = new P("The path to a GnuCash file is not configured. Please specify one in Settings.");
            else if (!File.Exists(GnuCashFile))
                GlobalErrorMessage = new P("GnuCash file cannot be found.\nFile: \"{0}\"".Fmt(GnuCashFile));
            else
            {
                try
                {
                    Session = new GncSession();
                    Session.LoadFromFile(GnuCashFile, BaseCurrency, BalsnapPrefix);
                    Book = Session.Book;
                }
                catch (Exception E)
                {
                    Session = null;
                    GlobalErrorMessage = new P("Could not load GnuCash file.\nFile: \"{0}\".\n\n{1}".Fmt(GnuCashFile, E.Message));
                }
            }
        }

        /// <summary>
        /// Reloads the <see cref="GncSession"/> underlying the current open file
        /// only if the gnucash file has changed. Otherwise does nothing.
        /// </summary>
        public void ReloadSessionIfNecessary()
        {
            DateTime curTimestamp;
            try { curTimestamp = File.GetLastWriteTimeUtc(GnuCashFile); }
            catch { curTimestamp = new DateTime(); }

            if (Session == null || curTimestamp != Session.ModifiedTimestamp)
                ReloadSession();
        }

        public string FileNameOnly
        {
            get
            {
                return Program.CurFile.LoadedFromFile == null
                    ? "unknown"
                    : PathUtil.ExtractNameAndExt(Program.CurFile.LoadedFromFile);
            }
        }

    }

    public class UserLink
    {
        public string Name;
        public string Href;
    }
}
