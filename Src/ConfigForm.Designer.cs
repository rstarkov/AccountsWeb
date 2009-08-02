namespace AccountsWeb
{
    partial class ConfigForm
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tabsMain = new System.Windows.Forms.TabControl();
            this.tabPaths = new System.Windows.Forms.TabPage();
            this.btnLanguage = new System.Windows.Forms.Button();
            this.txtBaseCurrency = new System.Windows.Forms.TextBox();
            this.lblBaseCurrency = new System.Windows.Forms.Label();
            this.btnBrowseGnuCash = new System.Windows.Forms.Button();
            this.txtGnuCashFile = new System.Windows.Forms.TextBox();
            this.lblGnuCashFile = new System.Windows.Forms.Label();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.txtListenPort = new System.Windows.Forms.TextBox();
            this.lblListenPort = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabsMain.SuspendLayout();
            this.tabPaths.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tabsMain, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 19F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(334, 330);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tabsMain
            // 
            this.tabsMain.Controls.Add(this.tabPaths);
            this.tabsMain.Controls.Add(this.tabGeneral);
            this.tabsMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabsMain.Location = new System.Drawing.Point(2, 3);
            this.tabsMain.Margin = new System.Windows.Forms.Padding(2, 3, 2, 0);
            this.tabsMain.Name = "tabsMain";
            this.tabsMain.SelectedIndex = 0;
            this.tabsMain.Size = new System.Drawing.Size(330, 286);
            this.tabsMain.TabIndex = 1;
            // 
            // tabPaths
            // 
            this.tabPaths.Controls.Add(this.btnLanguage);
            this.tabPaths.Controls.Add(this.txtBaseCurrency);
            this.tabPaths.Controls.Add(this.lblBaseCurrency);
            this.tabPaths.Controls.Add(this.btnBrowseGnuCash);
            this.tabPaths.Controls.Add(this.txtGnuCashFile);
            this.tabPaths.Controls.Add(this.lblGnuCashFile);
            this.tabPaths.Location = new System.Drawing.Point(4, 22);
            this.tabPaths.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tabPaths.Name = "tabPaths";
            this.tabPaths.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tabPaths.Size = new System.Drawing.Size(322, 260);
            this.tabPaths.TabIndex = 2;
            this.tabPaths.Text = "General";
            this.tabPaths.UseVisualStyleBackColor = true;
            // 
            // btnLanguage
            // 
            this.btnLanguage.Location = new System.Drawing.Point(4, 103);
            this.btnLanguage.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnLanguage.Name = "btnLanguage";
            this.btnLanguage.Size = new System.Drawing.Size(152, 23);
            this.btnLanguage.TabIndex = 6;
            this.btnLanguage.Text = "Select &Language";
            this.btnLanguage.UseVisualStyleBackColor = true;
            this.btnLanguage.Click += new System.EventHandler(this.btnLanguage_Click);
            // 
            // txtBaseCurrency
            // 
            this.txtBaseCurrency.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBaseCurrency.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtBaseCurrency.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.txtBaseCurrency.Location = new System.Drawing.Point(4, 65);
            this.txtBaseCurrency.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.txtBaseCurrency.Name = "txtBaseCurrency";
            this.txtBaseCurrency.Size = new System.Drawing.Size(86, 20);
            this.txtBaseCurrency.TabIndex = 5;
            this.txtBaseCurrency.Tag = "notranslate";
            // 
            // lblBaseCurrency
            // 
            this.lblBaseCurrency.AutoSize = true;
            this.lblBaseCurrency.Location = new System.Drawing.Point(4, 49);
            this.lblBaseCurrency.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBaseCurrency.Name = "lblBaseCurrency";
            this.lblBaseCurrency.Size = new System.Drawing.Size(78, 13);
            this.lblBaseCurrency.TabIndex = 4;
            this.lblBaseCurrency.Text = "&Base currency:";
            // 
            // btnBrowseGnuCash
            // 
            this.btnBrowseGnuCash.Location = new System.Drawing.Point(295, 21);
            this.btnBrowseGnuCash.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnBrowseGnuCash.Name = "btnBrowseGnuCash";
            this.btnBrowseGnuCash.Size = new System.Drawing.Size(24, 21);
            this.btnBrowseGnuCash.TabIndex = 3;
            this.btnBrowseGnuCash.Text = "...";
            this.btnBrowseGnuCash.UseVisualStyleBackColor = true;
            this.btnBrowseGnuCash.Click += new System.EventHandler(this.btnBrowseGnuCash_Click);
            // 
            // txtGnuCashFile
            // 
            this.txtGnuCashFile.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGnuCashFile.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtGnuCashFile.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.txtGnuCashFile.Location = new System.Drawing.Point(4, 22);
            this.txtGnuCashFile.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.txtGnuCashFile.Name = "txtGnuCashFile";
            this.txtGnuCashFile.Size = new System.Drawing.Size(288, 20);
            this.txtGnuCashFile.TabIndex = 2;
            this.txtGnuCashFile.Tag = "notranslate";
            // 
            // lblGnuCashFile
            // 
            this.lblGnuCashFile.AutoSize = true;
            this.lblGnuCashFile.Location = new System.Drawing.Point(4, 6);
            this.lblGnuCashFile.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGnuCashFile.Name = "lblGnuCashFile";
            this.lblGnuCashFile.Size = new System.Drawing.Size(110, 13);
            this.lblGnuCashFile.TabIndex = 1;
            this.lblGnuCashFile.Text = "&GnuCash file location:";
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.txtListenPort);
            this.tabGeneral.Controls.Add(this.lblListenPort);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tabGeneral.Size = new System.Drawing.Size(322, 260);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "Network";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // txtListenPort
            // 
            this.txtListenPort.Location = new System.Drawing.Point(4, 22);
            this.txtListenPort.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.txtListenPort.Name = "txtListenPort";
            this.txtListenPort.Size = new System.Drawing.Size(83, 20);
            this.txtListenPort.TabIndex = 3;
            this.txtListenPort.Tag = "notranslate";
            // 
            // lblListenPort
            // 
            this.lblListenPort.AutoSize = true;
            this.lblListenPort.Location = new System.Drawing.Point(4, 6);
            this.lblListenPort.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblListenPort.Name = "lblListenPort";
            this.lblListenPort.Size = new System.Drawing.Size(74, 13);
            this.lblListenPort.TabIndex = 2;
            this.lblListenPort.Text = "Listen on &port:";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.btnOK);
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(4, 295);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(158, 29);
            this.flowLayoutPanel1.TabIndex = 5;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(2, 3);
            this.btnOK.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(81, 3);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // dlgOpenFile
            // 
            this.dlgOpenFile.AddExtension = false;
            // 
            // ConfigForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(334, 330);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = global::AccountsWeb.Properties.Resources.gnucash_icon;
            this.Name = "ConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AccountsWeb Configuration";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ConfigForm_FormClosed);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabsMain.ResumeLayout(false);
            this.tabPaths.ResumeLayout(false);
            this.tabPaths.PerformLayout();
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TabControl tabsMain;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.TextBox txtListenPort;
        private System.Windows.Forms.Label lblListenPort;
        private System.Windows.Forms.TabPage tabPaths;
        private System.Windows.Forms.Label lblGnuCashFile;
        private System.Windows.Forms.TextBox txtGnuCashFile;
        private System.Windows.Forms.Button btnBrowseGnuCash;
        private System.Windows.Forms.OpenFileDialog dlgOpenFile;
        private System.Windows.Forms.TextBox txtBaseCurrency;
        private System.Windows.Forms.Label lblBaseCurrency;
        private System.Windows.Forms.Button btnLanguage;

    }
}