using System;
using System.Collections.Generic;
using System.IO;
using GnuCashSharp;
using RT.Servers;
using RT.TagSoup.HtmlTags;
using RT.Util;
using RT.Util.ExtensionMethods;
using RT.Util.Xml;

namespace AccountsWeb
{
    public class GncFileWrapper
    {
        public string GnuCashFile;
        public HttpServerOptions ServerOptions;
        public FileSystemOptions FileSystemOptions;
        public string BaseCurrency;
        public string BalsnapPrefix;
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
            BaseCurrency = Program.Tr.GncWrapper.DefaultBaseCurrency;
            BalsnapPrefix = Program.Tr.GncWrapper.DefaultBalsnapPrefix;
            UserLinks = new List<UserLink>() { new UserLink() { Name = Program.Tr.GncWrapper.DefaultExampleUserlink, Href = "/MonthlyTotals?MaxDepth=1" } };
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
                GlobalErrorMessage = new P(Program.Tr.GncWrapper.Error_FileNotConfigured);
            else if (!File.Exists(GnuCashFile))
                GlobalErrorMessage = new P(Program.Tr.GncWrapper.Error_FileNotFound.Translation.Fmt(GnuCashFile));
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
                    GlobalErrorMessage = new P(Program.Tr.GncWrapper.Error_CouldNotLoadFile.Fmt(GnuCashFile, E.Message));
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
                    ? Program.Tr.GncWrapper.FileNameUnknown.Translation
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
