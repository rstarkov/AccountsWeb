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
        [XmlIgnore]
        public string SettingsFileName;

        public string LastFileName;

        public List<string> RecentFiles = new List<string>();

        public Settings()
        {
        }

        public void SaveToFile(string filename)
        {
            SettingsFileName = filename;
            XmlClassify.SaveObjectToXmlFile(this, filename);
        }

        public void SaveToFile()
        {
            SaveToFile(SettingsFileName);
        }

        public static Settings LoadFromFile(string filename)
        {
            if (!File.Exists(filename))
                return new Settings() { SettingsFileName = filename };

            while (true)
            {
                try
                {
                    var settings = XmlClassify.LoadObjectFromXmlFile<Settings>(filename);
                    settings.SettingsFileName = filename;
                    return settings;
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
