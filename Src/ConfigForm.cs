using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RT.Util;
using RT.Util.Dialogs;
using RT.Util.ExtensionMethods;

namespace AccountsWeb
{
    public partial class ConfigForm: Form
    {
        private static int _activeTab = 0;
        private static ConfigForm _instance = null;

        private GncFileWrapper _wrapper;

        public ConfigForm(GncFileWrapper wrapper)
        {
            _wrapper = wrapper;
            InitializeComponent();

            tabsMain.SelectedIndex = _activeTab;
            SettingsToGui();
        }

        private void ConfigForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _activeTab = tabsMain.SelectedIndex;
            _instance = null;
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
                DlgMessage.ShowWarning("The \"Port\" field must contain an integer between 1 and 65535");
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
                Program.Interface.StartServer();
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
            dlgOpenFile.Title = "Select a GnuCash file";
            if (dlgOpenFile.ShowDialog() != DialogResult.OK)
                return;
            txtGnuCashFile.Text = dlgOpenFile.FileName;
        }
    }
}
