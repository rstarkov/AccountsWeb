using RT.Util.Lingo;

namespace AccountsWeb
{
    [LingoStringClass]
    public partial class TrayFormTranslation
    {
#if DEBUG
        [LingoAutoGenerated]
#endif
        public TrString TrayForm = "TrayForm";
    }

    [LingoStringClass]
    public partial class TrayMenuTranslation
    {
#if DEBUG
        [LingoAutoGenerated]
#endif
        [LingoInGroup(TrGroup.TrayMenu)]
        public TrString miOpenInBrowser = "Open in &browser";

#if DEBUG
        [LingoAutoGenerated]
#endif
        [LingoInGroup(TrGroup.TrayMenu)]
        public TrString miReload = "&Reload";

#if DEBUG
        [LingoAutoGenerated]
#endif
        [LingoInGroup(TrGroup.TrayMenu)]
        public TrString miSettings = "&Settings...";

#if DEBUG
        [LingoAutoGenerated]
#endif
        [LingoInGroup(TrGroup.TrayMenu)]
        public TrString miNewFile = "&New file...";

#if DEBUG
        [LingoAutoGenerated]
#endif
        [LingoInGroup(TrGroup.TrayMenu)]
        public TrString miOpenFile = "&Open file...";

#if DEBUG
        [LingoAutoGenerated]
#endif
        [LingoInGroup(TrGroup.TrayMenu)]
        public TrString miOpenRecent = "Open r&ecent";

#if DEBUG
        [LingoAutoGenerated]
#endif
        [LingoInGroup(TrGroup.TrayMenu)]
        public TrString miAbout = "&About... (in browser)";

#if DEBUG
        [LingoAutoGenerated]
#endif
        [LingoInGroup(TrGroup.TrayMenu)]
        public TrString miExit = "E&xit";
    }

    [LingoStringClass]
    public partial class ConfigFormTranslation
    {
#if DEBUG
        [LingoAutoGenerated]
#endif
        [LingoInGroup(TrGroup.SettingsDialog)]
        public TrString ConfigForm = "AccountsWeb Configuration";

#if DEBUG
        [LingoAutoGenerated]
#endif
        [LingoInGroup(TrGroup.SettingsDialog)]
        public TrString tabPaths = "General";

#if DEBUG
        [LingoAutoGenerated]
#endif
        [LingoInGroup(TrGroup.SettingsDialog)]
        public TrString btnLanguage = "Select &Language";

#if DEBUG
        [LingoAutoGenerated]
#endif
        [LingoInGroup(TrGroup.SettingsDialog)]
        public TrString lblBaseCurrency = "&Base currency:";

#if DEBUG
        [LingoAutoGenerated]
#endif
        [LingoInGroup(TrGroup.SettingsDialog)]
        public TrString btnBrowseGnuCash = "...";

#if DEBUG
        [LingoAutoGenerated]
#endif
        [LingoInGroup(TrGroup.SettingsDialog)]
        public TrString lblGnuCashFile = "&GnuCash file location:";

#if DEBUG
        [LingoAutoGenerated]
#endif
        [LingoInGroup(TrGroup.SettingsDialog)]
        public TrString tabGeneral = "Network";

#if DEBUG
        [LingoAutoGenerated]
#endif
        [LingoInGroup(TrGroup.SettingsDialog)]
        public TrString lblListenPort = "Listen on &port:";

#if DEBUG
        [LingoAutoGenerated]
#endif
        [LingoInGroup(TrGroup.SettingsDialog)]
        public TrString btnOK = "OK";

#if DEBUG
        [LingoAutoGenerated]
#endif
        [LingoInGroup(TrGroup.SettingsDialog)]
        public TrString btnCancel = "Cancel";
    }
}
