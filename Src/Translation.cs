﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Util.Lingo;

namespace AccountsWeb
{
    public enum TrGroup
    {
        [LingoGroup("Page: About", null)]
        PageAbout,
        [LingoGroup("Page: Add Link", null)]
        PageAddLink,
        [LingoGroup("Page: Exchange Rates", null)]
        PageExRates,
        [LingoGroup("Page: Last Balsnap", null)]
        PageLastBalsnap,
        [LingoGroup("Page: Main", null)]
        PageMain,
        [LingoGroup("Page: Monthly Totals", null)]
        PageMonthlyTotals,
        [LingoGroup("Page: Transactions", null)]
        PageTrns,
        [LingoGroup("Page: Warnings", null)]
        PageWarnings,

        [LingoGroup("Settings dialog", null)]
        SettingsDialog,
        [LingoGroup("Tray menu", null)]
        TrayMenu,

        [LingoGroup("Miscellaneous", null)]
        Misc,
        [LingoGroup("Must-Be style validation messages", "Contains all the \"must-be\" style validation message fragments.")]
        MustBeValidation,
    }

    [LingoStringClass]
    internal class Translation : TranslationBase
    {
        public static readonly Language DefaultLanguage = Language.EnglishUK;

        public Translation() : base(DefaultLanguage) { }

        [LingoStringClass]
        public class PgAboutTranslation
        {
            [LingoInGroup(TrGroup.PageAbout)]
            public TrString NavLink = "About";
            [LingoInGroup(TrGroup.PageAbout)]
            public TrString Title = "About {0}";
            [LingoInGroup(TrGroup.PageAbout)]
            public TrString Version = "Version {0}";
        }
        public PgAboutTranslation PgAbout = new PgAboutTranslation();

        [LingoStringClass]
        public class PgAddLinkTranslation
        {
            [LingoInGroup(TrGroup.PageAddLink)]
            public TrString Title = "Add Link";
            [LingoInGroup(TrGroup.PageAddLink)]
            public TrString Prompt = "Enter a name for the new link: ";
            [LingoInGroup(TrGroup.PageAddLink)]
            public TrString CreateButton = "Create link";
            [LingoInGroup(TrGroup.PageAddLink)]
            public TrString CreatedMessage = "A new link named \"{0}\" has been created successfully!";
            [LingoInGroup(TrGroup.PageAddLink)]
            public TrString CreatedReturnLink = "Return to the linked page";

            [LingoInGroup(TrGroup.PageAddLink)]
            [LingoInGroup(TrGroup.MustBeValidation)]
            public TrString Validation_Href = "specified";
        }
        public PgAddLinkTranslation PgAddLink = new PgAddLinkTranslation();

        [LingoStringClass]
        public class PgExRatesTranslation
        {
            [LingoInGroup(TrGroup.PageExRates)]
            public TrString NavLink = "Exchange rates";
            [LingoInGroup(TrGroup.PageExRates)]
            public TrString Title = "Exchange Rates";
            [LingoInGroup(TrGroup.PageExRates)]
            public TrString BaseCurrencyMessage = "This is the base currency";
            [LingoInGroup(TrGroup.PageExRates)]
            public TrString ColDate = "Date";
            [LingoInGroup(TrGroup.PageExRates)]
            public TrString ColRate = "Rate";
            [LingoInGroup(TrGroup.PageExRates)]
            public TrString ColRateInverse = "Inverse";
        }
        public PgExRatesTranslation PgExRates = new PgExRatesTranslation();

        [LingoStringClass]
        public class PgLastBalsnapTranslation
        {
            [LingoInGroup(TrGroup.PageLastBalsnap)]
            public TrString NavLink = "Last balsnaps";
            [LingoInGroup(TrGroup.PageLastBalsnap)]
            public TrString Title = "Last Balance Snapshot";
            [LingoInGroup(TrGroup.PageLastBalsnap)]
            public TrString ColLast = "Last";
            [LingoInGroup(TrGroup.PageLastBalsnap)]
            public TrString LastNever = "never";
            [LingoInGroup(TrGroup.PageLastBalsnap)]
            public TrString LastZero = "balance = 0";
            [LingoInGroup(TrGroup.PageLastBalsnap)]
            public TrStringNum LastNDaysAgo = new TrStringNum("{0} day ago", "{0} days ago");
        }
        public PgLastBalsnapTranslation PgLastBalsnap = new PgLastBalsnapTranslation();

        [LingoStringClass]
        public class PgMainTranslation
        {
            [LingoInGroup(TrGroup.PageMain)]
            public TrString WelcomeMessage = "Welcome to AccountsWeb for GnuCash! Please select one of the links on the left.";
        }
        public PgMainTranslation PgMain = new PgMainTranslation();

        [LingoStringClass]
        public class PgMonthlyTotalsTranslation
        {
            public TrString NavLink = "Monthly totals";
            [LingoInGroup(TrGroup.PageMonthlyTotals)]
            public TrString Title = "Monthly Totals";
            [LingoInGroup(TrGroup.PageMonthlyTotals)]
            public TrString CurAccount = "Account: ";
            [LingoInGroup(TrGroup.PageMonthlyTotals)]
            public TrString SubAcctsDepth = "Show subaccounts to depth: ";
            [LingoInGroup(TrGroup.PageMonthlyTotals)]
            public TrString SubAcctsNone = "None";
            [LingoInGroup(TrGroup.PageMonthlyTotals)]
            public TrString SubAcctsAll = "All";
            [LingoInGroup(TrGroup.PageMonthlyTotals)]
            public TrString ColAverage = "Avg.";
            [LingoInGroup(TrGroup.PageMonthlyTotals)]
            public TrString MessageExRatesUsed = "All values above are in {0}, converted where necessary using {1}.";
            [LingoInGroup(TrGroup.PageMonthlyTotals)]
            public TrString MessageExRatesUsedLink = "exchange rates";

            [LingoInGroup(TrGroup.PageMonthlyTotals)]
            [LingoInGroup(TrGroup.MustBeValidation)]
            public TrString Validation_Between1and12 = "between 1 and 12";
            [LingoInGroup(TrGroup.PageMonthlyTotals)]
            [LingoInGroup(TrGroup.MustBeValidation)]
            public TrString Validation_NotSmallerYear = "no smaller than the starting year, {0}";
            [LingoInGroup(TrGroup.PageMonthlyTotals)]
            [LingoInGroup(TrGroup.MustBeValidation)]
            public TrString Validation_Between1and12_NotSmallerMonth = "between 1 and 12, and no smaller than the starting month, {0}";
        }
        public PgMonthlyTotalsTranslation PgMonthlyTotals = new PgMonthlyTotalsTranslation();

        [LingoStringClass]
        public class PgTrnsTranslation
        {
            [LingoInGroup(TrGroup.PageTrns)]
            public TrString NavLink = "Transactions";
            [LingoInGroup(TrGroup.PageTrns)]
            public TrString Title = "Transactions";
            [LingoInGroup(TrGroup.PageTrns)]
            public TrString ShowingAll = "Showing all transactions.";
            [LingoInGroup(TrGroup.PageTrns)]
            public TrString ShowingOnOrBefore = "Showing all transactions on or before {0}.";
            [LingoInGroup(TrGroup.PageTrns)]
            public TrString ShowingOnOrAfter = "Showing all transactions on or after {0}.";
            [LingoInGroup(TrGroup.PageTrns)]
            public TrString ShowingBetween = "Showing transactions between {0} and {1}, inclusive.";
            [LingoInGroup(TrGroup.PageTrns)]
            public TrString ShowingSubaccts = "Subaccounts shown: ";
            [LingoInGroup(TrGroup.PageTrns)]
            public TrString ShowingSubacctsYes = "yes";
            [LingoInGroup(TrGroup.PageTrns)]
            public TrString ShowingSubacctsNo = "no";

            [LingoInGroup(TrGroup.PageTrns)]
            public TrString ColDate = "Date";
            [LingoInGroup(TrGroup.PageTrns)]
            public TrString ColDescription = "Description";
            [LingoInGroup(TrGroup.PageTrns)]
            public TrString ColQuantity = "Qty";
            [LingoInGroup(TrGroup.PageTrns)]
            public TrString ColCurrency = "Ccy";
            [LingoInGroup(TrGroup.PageTrns)]
            public TrString ColBalance = "Bal";
            [LingoInGroup(TrGroup.PageTrns)]
            public TrString ColAccount = "Account";
            [LingoInGroup(TrGroup.PageTrns)]
            public TrString ColInBaseCcy = "In {0}";

            [LingoInGroup(TrGroup.PageTrns)]
            [LingoInGroup(TrGroup.MustBeValidation)]
            public TrString Validation_NotBeforeFr = "no earlier than the \"Fr\" date";
            [LingoInGroup(TrGroup.PageTrns)]
            [LingoInGroup(TrGroup.MustBeValidation)]
            public TrString Validation_ShowBalVsSubAccts = "false when SubAccts is true";
        }
        public PgTrnsTranslation PgTrns = new PgTrnsTranslation();

        [LingoStringClass]
        public class PgWarningsTranslation
        {
            [LingoInGroup(TrGroup.PageWarnings)]
            public TrString Title = "Warnings";
            [LingoInGroup(TrGroup.PageWarnings)]
            public TrString MessageNoWarnings = "There were no warnings.";
        }
        public PgWarningsTranslation PgWarnings = new PgWarningsTranslation();



#if DEBUG
        [LingoDebug(@"..\..\users\rs\GnuCash\AccountsWeb\Translation.cs")]
#endif
        [LingoStringClass]
        public class ConfigFormTranslation
        {
            #region ConfigFormTranslation
            [LingoInGroup(TrGroup.SettingsDialog)]
            public TrString ConfigForm = "AccountsWeb Configuration";
            [LingoInGroup(TrGroup.SettingsDialog)]
            public TrString tabPaths = "General";
            [LingoInGroup(TrGroup.SettingsDialog)]
            public TrString btnLanguage = "Select &Language";
            [LingoInGroup(TrGroup.SettingsDialog)]
            public TrString lblBaseCurrency = "&Base currency:";
            [LingoInGroup(TrGroup.SettingsDialog)]
            public TrString btnBrowseGnuCash = "...";
            [LingoInGroup(TrGroup.SettingsDialog)]
            public TrString lblGnuCashFile = "&GnuCash file location:";
            [LingoInGroup(TrGroup.SettingsDialog)]
            public TrString tabGeneral = "Network";
            [LingoInGroup(TrGroup.SettingsDialog)]
            public TrString lblListenPort = "Listen on &port:";
            [LingoInGroup(TrGroup.SettingsDialog)]
            public TrString btnOK = "OK";
            [LingoInGroup(TrGroup.SettingsDialog)]
            public TrString btnCancel = "Cancel";
            #endregion

            [LingoInGroup(TrGroup.SettingsDialog)]
            public TrString Warning_PortValue = "The \"Port\" field must contain an integer between 1 and 65535";
            [LingoInGroup(TrGroup.SettingsDialog)]
            public TrString OpenDialog_Title = "Select a GnuCash file";
        }
        public ConfigFormTranslation Config = new ConfigFormTranslation();

#if DEBUG
        [LingoDebug(@"..\..\users\rs\GnuCash\AccountsWeb\Translation.cs")]
#endif
        [LingoStringClass]
        public class TrayFormTranslation
        {
            #region TrayFormTranslation
            [LingoInGroup(TrGroup.TrayMenu)]
            [LingoNotes("NOT USED ANYWHERE")]
            public TrString TrayForm = "TrayForm";
            #endregion

            [LingoInGroup(TrGroup.TrayMenu)]
            public TrString Tooltip_NoFileOpen = "No file open";
            [LingoInGroup(TrGroup.TrayMenu)]
            public TrString Tooltip_Running = "{0}\nAccountsWeb server is running.";
            [LingoInGroup(TrGroup.TrayMenu)]
            public TrString Tooltip_Stopped = "{0}\nAccountsWeb server is stopped.";
            [LingoInGroup(TrGroup.TrayMenu)]
            public TrString Title_NoFileOpen = "<no file open>";
            [LingoInGroup(TrGroup.TrayMenu)]
            public TrString Recent_DoesNotExist = "File \"{0}\" does not exist.\n\nWould you like to remove the file from the Recent menu?";
            [LingoInGroup(TrGroup.TrayMenu)]
            public TrString Recent_Remove = "&Remove from Recent";
            [LingoInGroup(TrGroup.TrayMenu)]
            public TrString Recent_Cancel = "&Cancel";
            [LingoInGroup(TrGroup.TrayMenu)]
            public TrString miStartStop_Start = "S&tart server";
            [LingoInGroup(TrGroup.TrayMenu)]
            public TrString miStartStop_Stop = "S&top server";

            [LingoInGroup(TrGroup.Misc)]
            public TrString dlgOpenFile_Title = "Select an AccountsWeb file to open";
            [LingoInGroup(TrGroup.Misc)]
            public TrString dlgSaveFile_Title = "Choose a location to save the new file";
            [LingoInGroup(TrGroup.Misc)]
            public TrString dlgFile_Filter = "AccountsWeb files|*.accweb|All files|*.*";
        }
        public TrayFormTranslation TrayForm = new TrayFormTranslation();

#if DEBUG
        [LingoDebug(@"..\..\users\rs\GnuCash\AccountsWeb\Translation.cs")]
#endif
        [LingoStringClass]
        public class TrayMenuTranslation
        {
            #region TrayMenuTranslation
            [LingoInGroup(TrGroup.TrayMenu)]
            public TrString miOpenInBrowser = "Open in &browser";
            [LingoInGroup(TrGroup.TrayMenu)]
            public TrString miReload = "&Reload";
            [LingoInGroup(TrGroup.TrayMenu)]
            public TrString miSettings = "&Settings...";
            [LingoInGroup(TrGroup.TrayMenu)]
            public TrString miNewFile = "&New file...";
            [LingoInGroup(TrGroup.TrayMenu)]
            public TrString miOpenFile = "&Open file...";
            [LingoInGroup(TrGroup.TrayMenu)]
            public TrString miOpenRecent = "Open r&ecent";
            [LingoInGroup(TrGroup.TrayMenu)]
            public TrString miAbout = "&About... (in browser)";
            [LingoInGroup(TrGroup.TrayMenu)]
            public TrString miExit = "E&xit";
            #endregion
        }
        public TrayMenuTranslation TrayMenu = new TrayMenuTranslation();



        [LingoStringClass]
        public class GncWrapperTranslation
        {
            [LingoInGroup(TrGroup.Misc)]
            public TrString DefaultBaseCurrency = "USD";
            [LingoInGroup(TrGroup.Misc)]
            public TrString DefaultBalsnapPrefix = "BALANCE:";
            [LingoInGroup(TrGroup.Misc)]
            public TrString DefaultExampleUserlink = "Example";

            [LingoInGroup(TrGroup.Misc)]
            public TrString Error_FileNotConfigured = "The path to a GnuCash file is not configured. Please specify one in Settings.";
            [LingoInGroup(TrGroup.Misc)]
            public TrString Error_FileNotFound = "GnuCash file cannot be found.\nFile: \"{0}\"";
            [LingoInGroup(TrGroup.Misc)]
            public TrString Error_CouldNotLoadFile = "Could not load GnuCash file.\nFile: \"{0}\".\n\n{1}";

            [LingoInGroup(TrGroup.Misc)]
            public TrString FileNameUnknown = "unknown";
        }
        public GncWrapperTranslation GncWrapper = new GncWrapperTranslation();

        [LingoStringClass]
        public class SettingsTranslation
        {
            [LingoInGroup(TrGroup.Misc)]
            public TrString CouldNotLoad = "Could not load settings from file {0}.\n{1}";
            [LingoInGroup(TrGroup.Misc)]
            public TrString CouldNotLoad_TryAgain = "Try again";
            [LingoInGroup(TrGroup.Misc)]
            public TrString CouldNotLoad_ContinueWithDefault = "Continue with default settings";
        }
        public SettingsTranslation Settings = new SettingsTranslation();

        [LingoInGroup(TrGroup.Misc)]
        public TrString NavigationHeader = "Navigation";
        [LingoInGroup(TrGroup.Misc)]
        public TrString LinksHeader = "Links";
        [LingoInGroup(TrGroup.Misc)]
        public TrString AddLink = "Add link";

        [LingoInGroup(TrGroup.Misc)]
        public TrString Warning_CouldNotOpenAccountsWeb = "Could not open AccountsWeb file.\nWrapper: \"{0}\"\nGnuCash: \"{1}\"\nError: {2}";
        [LingoInGroup(TrGroup.Misc)]
        [LingoNotes("Used in the \"Warning_CouldNotOpenAccountsWeb\" message to indicate that no GnuCash file path is available.")]
        public TrString Warning_AccWeb_NA = "N/A";
        [LingoInGroup(TrGroup.Misc)]
        public TrString Warning_CannotOpenInBrowser = "Cannot open in browser because the server is not running. Start it first and try again.";

        [LingoInGroup(TrGroup.Misc)]
        public TrString WarningsLink = "Warnings";

        [LingoStringClass]
        public class GlobalMessageTranslation
        {
            [LingoInGroup(TrGroup.Misc)]
            public TrString Title = "Warning";
            [LingoInGroup(TrGroup.Misc)]
            public TrString Explanation = "{0} cannot process your request due to the following error:";
            [LingoInGroup(TrGroup.Misc)]
            public TrString AlsoException = "Additionally, an exception has occurred:";
        }
        public GlobalMessageTranslation GlobalMessage = new GlobalMessageTranslation();

        [LingoInGroup(TrGroup.Misc)]
        public TrString ReportTable_ColAccount = "Account";
        [LingoInGroup(TrGroup.Misc)]
        [LingoNotes("{0} is the name of the account whose grand total is shown.")]
        public TrString ReportTable_GrandTotal = "TOTAL: {0}";

        [LingoInGroup(TrGroup.Misc)]
        [LingoInGroup(TrGroup.MustBeValidation)]
        public TrString Spinneret_Validation_AcctMustExist = "the name of an existing account";
    }
}
