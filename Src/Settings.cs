using System;
using System.Collections.Generic;
using System.IO;
using RT.Util.Dialogs;
using RT.Util.ExtensionMethods;
using RT.Util.Xml;
using RT.Util.Lingo;
using RT.Util.Forms;

namespace AccountsWeb
{
    internal class Settings
    {
        [XmlIgnore]
        public string SettingsFileName;

        internal string LastFileName;
        internal List<string> RecentFiles = new List<string>();
        internal Language Language = Translation.DefaultLanguage;

        internal ManagedForm.Settings ConfigFormSettings = new ManagedForm.Settings();
        internal TranslationForm<Translation>.Settings TranslationFormSettings = new TranslationForm<Translation>.Settings();

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
                    int choice = DlgMessage.ShowWarning(Program.Tr.Settings.CouldNotLoad.Fmt(filename, E.Message),
                        Program.Tr.Settings.CouldNotLoad_TryAgain, Program.Tr.Settings.CouldNotLoad_ContinueWithDefault);
                    if (choice == 1)
                        return new Settings();
                }
            }
        }
    }
}
