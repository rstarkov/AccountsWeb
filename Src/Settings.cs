using System.Collections.Generic;
using RT.Lingo;
using RT.Util;
using RT.Util.Forms;

namespace AccountsWeb
{
    class Settings
    {
        public string LastFileName;
        public List<string> RecentFiles = new List<string>();
        public Language Language = Translation.DefaultLanguage;

        public Dictionary<string, string> ReconcileRegexes = new Dictionary<string, string>() { { "Example", "^regex$" } };

        public ManagedForm.Settings ConfigFormSettings = new ManagedForm.Settings();
        public TranslationForm<Translation>.Settings TranslationFormSettings = new TranslationForm<Translation>.Settings();
    }
}
