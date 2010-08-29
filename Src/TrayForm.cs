using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using RT.Util;
using RT.Util.Dialogs;
using RT.Util.ExtensionMethods;
using RT.Util.Lingo;

namespace AccountsWeb
{
    partial class TrayForm : Form
    {
        public TrayForm()
        {
            InitializeComponent();
            TrayIcon.Icon = Properties.Resources.gnucash_icon_16_gray;
            TrayIcon.Visible = true;
            Translate();
        }

        internal void Translate()
        {
            Lingo.TranslateControl(this, Program.Tr.TrayForm);
            Lingo.TranslateControl(TrayMenu, Program.Tr.TrayMenu);
            dlgOpenFile.Title = Program.Tr.TrayForm.dlgOpenFile_Title;
            dlgOpenFile.Filter = Program.Tr.TrayForm.dlgFile_Filter;
            dlgSaveFile.Title = Program.Tr.TrayForm.dlgSaveFile_Title;
            dlgSaveFile.Filter = Program.Tr.TrayForm.dlgFile_Filter;
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
                TrayIcon.Text = Program.Tr.TrayForm.Tooltip_NoFileOpen;
                TrayIcon.Icon = Properties.Resources.gnucash_icon_16_gray;
            }
            else if (Program.Interface.ServerRunning)
            {
                TrayIcon.Text = Program.Tr.TrayForm.Tooltip_Running.Fmt(Program.CurFile.FileNameOnly);
                TrayIcon.Icon = Properties.Resources.gnucash_icon_16;
            }
            else
            {
                TrayIcon.Text = Program.Tr.TrayForm.Tooltip_Stopped.Fmt(Program.CurFile.FileNameOnly);
                TrayIcon.Icon = Properties.Resources.gnucash_icon_16_gray;
            }
        }

        private void TrayMenu_Opening(object sender, CancelEventArgs e)
        {
            if (Program.CurFile == null)
            {
                miCurrentFileName.Text = "   " + Program.Tr.TrayForm.Title_NoFileOpen;
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
            miStartStopServer.Text = Program.Interface.ServerRunning ? Program.Tr.TrayForm.miStartStop_Stop : Program.Tr.TrayForm.miStartStop_Start;
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
                DlgMessage.ShowInfo(Program.Tr.Warning_CannotOpenInBrowser);
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
                Program.Interface.StartServer(Program.CurFile.ServerOptions, Program.CurFile.FileSystemOptions);
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
            ToolStripMenuItem item = (ToolStripMenuItem) sender;

            if (!File.Exists(item.Text))
            {
                var choice = DlgMessage.ShowQuestion(Program.Tr.TrayForm.Recent_DoesNotExist.Fmt(item.Text),
                    Program.Tr.TrayForm.Recent_Remove, Program.Tr.TrayForm.Recent_Cancel);
                if (choice == 0)
                {
                    Program.Settings.RecentFiles.Remove(item.Text);
                    Program.Settings.SaveQuiet();
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
                DlgMessage.ShowInfo(Program.Tr.Warning_CannotOpenInBrowser);
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
