using System;
using System.Windows.Forms;
using RT.Util.Dialogs;
using RT.Util.ExtensionMethods;
using RT.Util.Forms;
using RT.Util.Lingo;

namespace AccountsWeb
{
    public partial class ConfigForm: ManagedForm
    {
        private static int _activeTab = 0;
        private static ConfigForm _instance = null;
        private LanguageContextMenuHelper<Translation> _languageMenuHelper;

        private GncFileWrapper _wrapper;

        public ConfigForm(GncFileWrapper wrapper)
            : base(Program.Settings.ConfigFormSettings)
        {
            _wrapper = wrapper;
            InitializeComponent();
            translate();

            _languageMenuHelper = new LanguageContextMenuHelper<Translation>(
                "AccountsWeb", "AccountsWeb", Translation.DefaultLanguage,
                Program.Settings.TranslationFormSettings, Icon, setLanguage);
            _languageMenuHelper.TranslationEditingEnabled = true;

            tabsMain.SelectedIndex = _activeTab;
            SettingsToGui();
        }

        private void ConfigForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _activeTab = tabsMain.SelectedIndex;
            _instance = null;
        }

        private void setLanguage(Translation translation)
        {
            Program.Tr = translation;
            Program.Settings.Language = translation.Language;
            translate();
            Program.TrayForm.Translate();

            Program.Interface.StopServer();
            Program.Interface = new WebInterface();
            Program.Interface.StartServer(Program.CurFile.ServerOptions);
        }

        private void translate()
        {
            Lingo.TranslateControl(this, Program.Tr.Config);
        }

        private void SettingsToGui()
        {
            txtGnuCashFile.Text = _wrapper.GnuCashFile ?? "";
            txtBaseCurrency.Text = _wrapper.BaseCurrency ?? "";
            txtListenPort.Text = _wrapper.ServerOptions.Port.ToString();
        }

        private bool SettingsFromGui()
        {
            int port;
            if (!int.TryParse(txtListenPort.Text, out port) || port < 1 || port > 65535)
            {
                tabsMain.SelectedTab = txtListenPort.ParentTab();
                txtListenPort.Focus();
                DlgMessage.ShowWarning(Program.Tr.Config.Warning_PortValue);
                return false;
            }

            bool restartRequired =
                _wrapper.ServerOptions.Port != port;

            bool reloadRequired =
                _wrapper.BaseCurrency != txtBaseCurrency.Text ||
                _wrapper.GnuCashFile != txtGnuCashFile.Text;

            _wrapper.ServerOptions.Port = port;
            _wrapper.GnuCashFile = txtGnuCashFile.Text;
            _wrapper.BaseCurrency = txtBaseCurrency.Text;

            if (reloadRequired)
                Program.CurFile.ReloadSession();

            if (restartRequired)
            {
                Program.Interface.StopServer();
                Program.Interface.StartServer(Program.CurFile.ServerOptions);
            }

            return true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (SettingsFromGui())
            {
                _wrapper.SaveToFile();
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        public static void Show(GncFileWrapper wrapper)
        {
            if (_instance == null)
            {
                _instance = new ConfigForm(wrapper);
                _instance.Show();
            }
            else
            {
                _instance.BringToFront();
            }
        }

        private void btnBrowseGnuCash_Click(object sender, EventArgs e)
        {
            dlgOpenFile.Title = Program.Tr.Config.OpenDialog_Title;
            if (dlgOpenFile.ShowDialog() != DialogResult.OK)
                return;
            txtGnuCashFile.Text = dlgOpenFile.FileName;
        }

        private void btnLanguage_Click(object sender, EventArgs e)
        {
            _languageMenuHelper.ShowContextMenu(Program.Tr.Language, btnLanguage, new System.Drawing.Point(0, btnLanguage.Height));
        }
    }
}
