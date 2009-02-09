using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Servers;
using RT.Util.XmlClassify;
using System.IO;
using RT.Util.Dialogs;
using RT.Util.ExtensionMethods;

namespace AccountsWeb
{
    public class Settings
    {
        public HttpServerOptions ServerOptions;
        public string GnuCashFile;

        public Settings()
        {
            ServerOptions = new HttpServerOptions();
            ServerOptions.Port = 1771;
        }

        public void SaveToFile(string filename)
        {
            XmlClassify.SaveObjectToXmlFile(this, filename);
        }

        public static Settings LoadFromFile(string filename)
        {
            if (!File.Exists(filename))
                return new Settings();

            while (true)
            {
                try
                {
                    return XmlClassify.LoadObjectFromXmlFile<Settings>(filename);
                }
                catch (Exception E)
                {
                    int choice = DlgMessage.ShowWarning("Could not load settings from file {0}.\n{1}".Fmt(filename, E.Message),
                        "Try again", "Continue with default settings");
                    if (choice == 1)
                        return new Settings();
                }
            }
        }
    }
}
