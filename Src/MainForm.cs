using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GnuCashSharp;
using RT.Util.Text;
using RT.Util.ExtensionMethods;
using RT.Util.Collections;
using RT.Util.Dialogs;
using System.Diagnostics;

namespace AccountsWeb
{
    public partial class MainForm: Form
    {
        public MainForm()
        {
            InitializeComponent();
            TrayIcon.Icon = Properties.Resources.gnucash_icon_16_gray;
            TrayIcon.Visible = true;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.Server.StopListening(true);
            TrayIcon.Visible = false;
        }

        private void TrayMenu_Opening(object sender, CancelEventArgs e)
        {
            miStartStopServer.Text = Program.Server.IsListening ? "S&top server" : "S&tart server";
        }

        private void miOpenInBrowser_Click(object sender, EventArgs e)
        {
            if (Program.Server.IsListening)
            {
                ProcessStartInfo si = new ProcessStartInfo("http://localhost:{0}".Fmt(Program.Server.Options.Port));
                si.UseShellExecute = true;
                Process.Start(si);
            }
            else
                DlgMessage.ShowInfo("Cannot open in browser because the server is not running. Start it first and try again.");
        }

        private void miStartStopServer_Click(object sender, EventArgs e)
        {
            if (Program.Server.IsListening)
                Program.StopServer();
            else
                Program.StartServer();
        }

        private void miSettings_Click(object sender, EventArgs e)
        {
            DlgMessage.ShowInfo("Sorry, not yet implemented.");
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            Close();
            Application.Exit();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Program.Server.IsListening)
            {
                TrayIcon.Text = "AccountsWeb server is running";
                TrayIcon.Icon = Properties.Resources.gnucash_icon_16;
            }
            else
            {
                TrayIcon.Text = "AccountsWeb server is stopped";
                TrayIcon.Icon = Properties.Resources.gnucash_icon_16_gray;
            }
        }

    }

    public class ReportAccountsByMonth
    {
        public DateTime DateFrom;
        public DateTime DateTo;
        public GncBook Book;

        private IEnumerable<Tuple<DateTime, DateTime>> EnumMonths(DateTime from, DateTime to)
        {
            while (from <= to)
            {
                DateTime periodTo = from.AddDays(35);
                periodTo = new DateTime(periodTo.Year, periodTo.Month, 1).AddDays(-1);
                yield return new Tuple<DateTime, DateTime>(from, periodTo);
                from = periodTo.AddDays(1);
            }
        }

        public void Execute()
        {
            CsvTable table = new CsvTable();
            table.AdvanceRight = true;
            table.Add("Счет");
            foreach (var interval in EnumMonths(DateFrom, DateTo))
                table.Add(interval.E1.ToString("MMM yy"));
            table.SetCursor(1, 0);
            processAccount(table, Book.AccountRoot, 0);
            table.SaveToFileXls("report.html");
        }

        private void processAccount(CsvTable table, GncAccount acct, int indent)
        {
            foreach (var acctChild in acct.EnumChildren())
            {
                table.Add("&nbsp;".Repeat(indent*4) + acctChild.Name);

                foreach (var interval in EnumMonths(DateFrom, DateTo))
                    table.Add(0);

                table.SetCursor(table.CurRow+1, 0);
                processAccount(table, acctChild, indent+1);
            }
        }
    }
}
