using System.Collections.Generic;
using RT.Util;
using RT.Util.Forms;
using RT.Util.Lingo;
using RT.Util.Xml;

namespace AccountsWeb
{
    [Settings("AccountsWeb", SettingsKind.UserSpecific)]
    class Settings : SettingsBase
    {
        [XmlIgnore]
        public string SettingsFileName;

        public string LastFileName;
        public List<string> RecentFiles = new List<string>();
        public Language Language = Translation.DefaultLanguage;

        public ManagedForm.Settings ConfigFormSettings = new ManagedForm.Settings();
        public TranslationForm<Translation>.Settings TranslationFormSettings = new TranslationForm<Translation>.Settings();
    }
}
