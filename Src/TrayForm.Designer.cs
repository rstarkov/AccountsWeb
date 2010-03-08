namespace AccountsWeb
{
    partial class TrayForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.TrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.TrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miCurrentFileName = new System.Windows.Forms.ToolStripMenuItem();
            this.miOpenInBrowser = new System.Windows.Forms.ToolStripMenuItem();
            this.miReload = new System.Windows.Forms.ToolStripMenuItem();
            this.miSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.miStartStopServer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.miNewFile = new System.Windows.Forms.ToolStripMenuItem();
            this.miOpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.miOpenRecent = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.miAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();
            this.Timer = new System.Windows.Forms.Timer(this.components);
            this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.dlgSaveFile = new System.Windows.Forms.SaveFileDialog();
            this.TrayMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // TrayIcon
            // 
            this.TrayIcon.ContextMenuStrip = this.TrayMenu;
            this.TrayIcon.Text = "AccountsWeb";
            this.TrayIcon.Visible = true;
            this.TrayIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TrayIcon_MouseClick);
            // 
            // TrayMenu
            // 
            this.TrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miCurrentFileName,
            this.miOpenInBrowser,
            this.miReload,
            this.miSettings,
            this.miStartStopServer,
            this.toolStripSeparator2,
            this.miNewFile,
            this.miOpenFile,
            this.miOpenRecent,
            this.toolStripSeparator1,
            this.miAbout,
            this.miExit});
            this.TrayMenu.Name = "TrayMenu";
            this.TrayMenu.Size = new System.Drawing.Size(177, 258);
            this.TrayMenu.Opening += new System.ComponentModel.CancelEventHandler(this.TrayMenu_Opening);
            // 
            // miCurrentFileName
            // 
            this.miCurrentFileName.Enabled = false;
            this.miCurrentFileName.Name = "miCurrentFileName";
            this.miCurrentFileName.Size = new System.Drawing.Size(176, 22);
            this.miCurrentFileName.Tag = "notranslate";
            this.miCurrentFileName.Text = "RomanGnuCash.xml:";
            // 
            // miOpenInBrowser
            // 
            this.miOpenInBrowser.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.miOpenInBrowser.Name = "miOpenInBrowser";
            this.miOpenInBrowser.Size = new System.Drawing.Size(176, 22);
            this.miOpenInBrowser.Text = "Open in &browser";
            this.miOpenInBrowser.Click += new System.EventHandler(this.miOpenInBrowser_Click);
            // 
            // miReload
            // 
            this.miReload.Name = "miReload";
            this.miReload.Size = new System.Drawing.Size(176, 22);
            this.miReload.Text = "&Reload";
            this.miReload.Click += new System.EventHandler(this.miReload_Click);
            // 
            // miSettings
            // 
            this.miSettings.Name = "miSettings";
            this.miSettings.Size = new System.Drawing.Size(176, 22);
            this.miSettings.Text = "&Settings...";
            this.miSettings.Click += new System.EventHandler(this.miSettings_Click);
            // 
            // miStartStopServer
            // 
            this.miStartStopServer.Name = "miStartStopServer";
            this.miStartStopServer.Size = new System.Drawing.Size(176, 22);
            this.miStartStopServer.Tag = "notranslate";
            this.miStartStopServer.Text = "S&tart server";
            this.miStartStopServer.Click += new System.EventHandler(this.miStartStopServer_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(173, 6);
            // 
            // miNewFile
            // 
            this.miNewFile.Name = "miNewFile";
            this.miNewFile.Size = new System.Drawing.Size(176, 22);
            this.miNewFile.Text = "&New file...";
            this.miNewFile.Click += new System.EventHandler(this.miNewFile_Click);
            // 
            // miOpenFile
            // 
            this.miOpenFile.Name = "miOpenFile";
            this.miOpenFile.Size = new System.Drawing.Size(176, 22);
            this.miOpenFile.Text = "&Open file...";
            this.miOpenFile.Click += new System.EventHandler(this.miOpenFile_Click);
            // 
            // miOpenRecent
            // 
            this.miOpenRecent.Name = "miOpenRecent";
            this.miOpenRecent.Size = new System.Drawing.Size(176, 22);
            this.miOpenRecent.Text = "Open r&ecent";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(173, 6);
            // 
            // miAbout
            // 
            this.miAbout.Name = "miAbout";
            this.miAbout.Size = new System.Drawing.Size(176, 22);
            this.miAbout.Text = "&About... (in browser)";
            this.miAbout.Click += new System.EventHandler(this.miAbout_Click);
            // 
            // miExit
            // 
            this.miExit.Name = "miExit";
            this.miExit.Size = new System.Drawing.Size(176, 22);
            this.miExit.Text = "E&xit";
            this.miExit.Click += new System.EventHandler(this.miExit_Click);
            // 
            // Timer
            // 
            this.Timer.Enabled = true;
            this.Timer.Interval = 333;
            this.Timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // dlgOpenFile
            // 
            this.dlgOpenFile.DefaultExt = "accweb";
            this.dlgOpenFile.ValidateNames = false;
            // 
            // dlgSaveFile
            // 
            this.dlgSaveFile.DefaultExt = "accweb";
            // 
            // TrayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(181, 122);
            this.Name = "TrayForm";
            this.Text = "TrayForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TrayForm_FormClosed);
            this.TrayMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon TrayIcon;
        private System.Windows.Forms.ToolStripMenuItem miSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem miExit;
        private System.Windows.Forms.ToolStripMenuItem miStartStopServer;
        private System.Windows.Forms.Timer Timer;
        private System.Windows.Forms.ToolStripMenuItem miOpenInBrowser;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem miCurrentFileName;
        private System.Windows.Forms.ToolStripMenuItem miNewFile;
        private System.Windows.Forms.ToolStripMenuItem miOpenFile;
        private System.Windows.Forms.ToolStripMenuItem miOpenRecent;
        private System.Windows.Forms.ToolStripMenuItem miReload;
        private System.Windows.Forms.OpenFileDialog dlgOpenFile;
        private System.Windows.Forms.SaveFileDialog dlgSaveFile;
        private System.Windows.Forms.ToolStripMenuItem miAbout;
        public System.Windows.Forms.ContextMenuStrip TrayMenu;
    }
}

