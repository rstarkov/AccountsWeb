using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using RT.Util;
using RT.Util.Dialogs;
using RT.Util.ExtensionMethods;

namespace AccountsWeb
{
    public partial class TrayForm: Form
    {
        public TrayForm()
        {
            InitializeComponent();
            TrayIcon.Icon = Properties.Resources.gnucash_icon_16_gray;
            TrayIcon.Visible = true;
        }

        private void TrayForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.Interface.StopServer();
            TrayIcon.Visible = false;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Program.CurFile == null)
            {
                TrayIcon.Text = "No file open";
                TrayIcon.Icon = Properties.Resources.gnucash_icon_16_gray;
            }
            else if (Program.Interface.ServerRunning)
            {
                TrayIcon.Text = "{0}\nAccountsWeb server is running.".Fmt(Program.CurFile.FileNameOnly);
                TrayIcon.Icon = Properties.Resources.gnucash_icon_16;
            }
            else
            {
                TrayIcon.Text = "{0}\nAccountsWeb server is stopped.".Fmt(Program.CurFile.FileNameOnly);
                TrayIcon.Icon = Properties.Resources.gnucash_icon_16_gray;
            }
        }

        private void TrayMenu_Opening(object sender, CancelEventArgs e)
        {
            if (Program.CurFile == null)
            {
                miCurrentFileName.Text = "   <no file open>";
                miOpenInBrowser.Enabled = false;
                miReload.Enabled = false;
                miSettings.Enabled = false;
                miStartStopServer.Enabled = false;
            }
            else
            {
                miCurrentFileName.Text = "   " + Program.CurFile.FileNameOnly;
                miOpenInBrowser.Enabled = true;
                miReload.Enabled = true;
                miSettings.Enabled = true;
                miStartStopServer.Enabled = true;
            }
            miStartStopServer.Text = Program.Interface.ServerRunning ? "S&top server" : "S&tart server";
            miOpenRecent.DropDownItems.Clear();
            foreach (var filename in Program.Settings.RecentFiles)
                miOpenRecent.DropDownItems.Add(filename, null, miOpenRecent_Click);
        }

        private void miOpenInBrowser_Click(object sender, EventArgs e)
        {
            if (Program.Interface.ServerRunning)
            {
                ProcessStartInfo si = new ProcessStartInfo("http://localhost:{0}".Fmt(Program.Interface.ServerPort));
                si.UseShellExecute = true;
                Process.Start(si);
            }
            else
                DlgMessage.ShowInfo("Cannot open in browser because the server is not running. Start it first and try again.");
        }

        private void miReload_Click(object sender, EventArgs e)
        {
            Program.CurFile.ReloadSession();
        }

        private void miSettings_Click(object sender, EventArgs e)
        {
            ConfigForm.Show(Program.CurFile);
        }

        private void miStartStopServer_Click(object sender, EventArgs e)
        {
            if (Program.Interface.ServerRunning)
                Program.Interface.StopServer();
            else
                Program.Interface.StartServer();
        }

        private void miNewFile_Click(object sender, EventArgs e)
        {
            if (dlgSaveFile.ShowDialog() != DialogResult.OK)
                return;

            Program.NewFile(dlgSaveFile.FileName);

            ConfigForm.Show(Program.CurFile);
        }

        private void miOpenFile_Click(object sender, EventArgs e)
        {
            if (Program.Settings.LastFileName != null)
                dlgOpenFile.InitialDirectory = PathUtil.ExtractParent(Program.Settings.LastFileName);

            if (dlgOpenFile.ShowDialog() != DialogResult.OK)
                return;

            Program.OpenFile(dlgOpenFile.FileName);
        }

        private void miOpenRecent_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            if (!File.Exists(item.Text))
            {
                var choice = DlgMessage.ShowQuestion("File \"{0}\" does not exist.\n\nWould you like to remove the file from the Recent menu?",
                    "Remove from Recent", "Cancel");
                if (choice == 0)
                {
                    Program.Settings.RecentFiles.Remove(item.Text);
                    Program.Settings.SaveToFile();
                }
                return;
            }

            Program.OpenFile(item.Text);
        }

        private void miAbout_Click(object sender, EventArgs e)
        {
            if (Program.Interface.ServerRunning)
            {
                ProcessStartInfo si = new ProcessStartInfo("http://localhost:{0}/About".Fmt(Program.Interface.ServerPort));
                si.UseShellExecute = true;
                Process.Start(si);
            }
            else
                DlgMessage.ShowInfo("Cannot open in browser because the server is not running. Start it first and try again.");
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            if (Program.Interface.ServerRunning)
                Program.Interface.StopServer();
            Close();
            Application.Exit();
        }

        private void TrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                miOpenInBrowser_Click(sender, null);
        }

    }
}
