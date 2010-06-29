using System;
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
        [LingoGroup("Page: Monthly (shared)", null)]
        PageMonthly,
        [LingoGroup("Page: Monthly Totals", null)]
        PageMonthlyTotals,
        [LingoGroup("Page: Monthly Balances", null)]
        PageMonthlyBalances,
        [LingoGroup("Page: Transactions", null)]
        PageTrns,
        [LingoGroup("Page: Reconcile", null)]
        PageReconcile,
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
    partial class Translation : TranslationBase
    {
        public static readonly Language DefaultLanguage = Language.EnglishUK;

        public Translation() : base(DefaultLanguage) { }

        public NumberSystem Ns { get { return Language.GetNumberSystem(); } }

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
        public class PgMonthlyTranslation
        {
            [LingoInGroup(TrGroup.PageMonthly)]
            public TrString CurAccount = "Account: ";
            [LingoInGroup(TrGroup.PageMonthly)]
            public TrString SubAcctsDepth = "Show subaccounts to depth: ";
            [LingoInGroup(TrGroup.PageMonthly)]
            public TrString SubAcctsNone = "None";
            [LingoInGroup(TrGroup.PageMonthly)]
            public TrString SubAcctsAll = "All";
            [LingoInGroup(TrGroup.PageMonthly)]
            public TrString GroupMonthsCount = "Group months: ";
            [LingoInGroup(TrGroup.PageMonthly)]
            public TrString PastYears = "Show past years: ";
            [LingoInGroup(TrGroup.PageMonthly)]
            public TrString ViewModeTotals = "View totals";
            [LingoInGroup(TrGroup.PageMonthly)]
            public TrString ViewModeBalances = "View balances";

            [LingoInGroup(TrGroup.PageMonthly)]
            public TrString MessageExRatesUsed = "All values above are in {0}, converted where necessary using {1}.";
            [LingoInGroup(TrGroup.PageMonthly)]
            public TrString MessageExRatesUsedLink = "exchange rates";
            [LingoInGroup(TrGroup.PageMonthly)]
            [LingoNotes("Only used when highlighting columns which don't cover exactly as many months as requested. Added after the actual number of months covered by a column.")]
            public TrString MoSuffix = "mo";
            [LingoInGroup(TrGroup.PageMonthly)]
            [LingoNotes("When displaying a range of months in a single column, is placed between the first month last month in the column title.")]
            public TrString MonthGroupJoiner = "to";

            [LingoInGroup(TrGroup.PageMonthly)]
            public TrStringNum YearsMonths = new TrStringNum(new[] { "{0} year + {1} month", "{0} years + {1} month", "{0} year + {1} months", "{0} years + {1} months" }, new[] { true, true });

            [LingoInGroup(TrGroup.PageMonthly)]
            [LingoInGroup(TrGroup.MustBeValidation)]
            public TrString Validation_Between1and12 = "between 1 and 12";
            [LingoInGroup(TrGroup.PageMonthly)]
            [LingoInGroup(TrGroup.MustBeValidation)]
            public TrString Validation_NotSmallerYear = "no smaller than the starting year, {0}";
            [LingoInGroup(TrGroup.PageMonthly)]
            [LingoInGroup(TrGroup.MustBeValidation)]
            public TrString Validation_Between1and12_NotSmallerMonth = "between 1 and 12, and no smaller than the starting month, {0}";
            [LingoInGroup(TrGroup.PageMonthly)]
            [LingoInGroup(TrGroup.MustBeValidation)]
            public TrString Validation_1OrGreater = "1 or greater";
            [LingoInGroup(TrGroup.PageMonthly)]
            [LingoInGroup(TrGroup.MustBeValidation)]
            public TrString Validation_NonNegative = "non-negative";
        }
        public PgMonthlyTranslation PgMonthly = new PgMonthlyTranslation();

        [LingoStringClass]
        public class PgMonthlyTotalsTranslation
        {
            [LingoInGroup(TrGroup.PageMonthlyTotals)]
            public TrString NavLink = "Monthly totals";
            [LingoInGroup(TrGroup.PageMonthlyTotals)]
            public TrString Title = "Monthly Totals";
            [LingoInGroup(TrGroup.PageMonthlyTotals)]
            public TrString ColAverage = "Avg.";
        }
        public PgMonthlyTotalsTranslation PgMonthlyTotals = new PgMonthlyTotalsTranslation();

        [LingoStringClass]
        public class PgMonthlyBalancesTranslation
        {
            [LingoInGroup(TrGroup.PageMonthlyBalances)]
            public TrString NavLink = "Monthly balances";
            [LingoInGroup(TrGroup.PageMonthlyBalances)]
            public TrString Title = "Monthly Balances";
        }
        public PgMonthlyBalancesTranslation PgMonthlyBalances = new PgMonthlyBalancesTranslation();

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
        public class PgReconcileTranslation
        {
            // TODO: example (default regex in settings)

            [LingoInGroup(TrGroup.PageReconcile)]
            public TrString NavLink = "Reconcile";
            [LingoInGroup(TrGroup.PageReconcile)]
            public TrString Title = "Reconcile";

            [LingoInGroup(TrGroup.PageReconcile)]
            public TrString BtnReconcile = "Reconcile";
            [LingoInGroup(TrGroup.PageReconcile)]
            public TrString BtnDelete = "Delete";
            [LingoInGroup(TrGroup.PageReconcile)]
            public TrString FormUseExistingPreset = "Use existing preset: ";
            [LingoInGroup(TrGroup.PageReconcile)]
            public TrString FormCreateModifyPreset = "Create/modify preset: ";
            [LingoInGroup(TrGroup.PageReconcile)]
            [LingoInGroup(TrGroup.MustBeValidation)]
            public TrString Validation_ReconcilePreset = "Reconcile preset";
            [LingoInGroup(TrGroup.PageReconcile)]
            [LingoInGroup(TrGroup.MustBeValidation)]
            public TrString Validation_ReconcilePresetMsg = "composed of a name and a regex separated by a colon";
            [LingoInGroup(TrGroup.PageReconcile)]
            [LingoInGroup(TrGroup.MustBeValidation)]
            public TrString Validation_RequireDrOrCr = "have both dr and cr, or neither cr nor dr.";
            [LingoInGroup(TrGroup.PageReconcile)]
            public TrString HeaderParsedStatement = "Parsed statement";
            [LingoInGroup(TrGroup.PageReconcile)]
            public TrString HeaderReconciledTransactions = "Reconciled transactions";
            [LingoInGroup(TrGroup.PageReconcile)]
            public TrString ColAcctDate = "Acct\nDate";
            [LingoInGroup(TrGroup.PageReconcile)]
            public TrString ColStmtDate = "Stmt\nDate";
            [LingoInGroup(TrGroup.PageReconcile)]
            public TrString ColComment = "Comment";
            [LingoInGroup(TrGroup.PageReconcile)]
            public TrString ColAmount = "Amt";
            [LingoInGroup(TrGroup.PageReconcile)]
            public TrString ColBalance = "Bal";
        }
        public PgReconcileTranslation PgReconcile = new PgReconcileTranslation();

        [LingoStringClass]
        public class PgWarningsTranslation
        {
            [LingoInGroup(TrGroup.PageWarnings)]
            public TrString Title = "Warnings";
            [LingoInGroup(TrGroup.PageWarnings)]
            public TrString MessageNoWarnings = "There were no warnings.";
        }

        public PgWarningsTranslation PgWarnings = new PgWarningsTranslation();
        public ConfigFormTranslation Config = new ConfigFormTranslation();
        public TrayFormTranslation TrayForm = new TrayFormTranslation();
        public TrayMenuTranslation TrayMenu = new TrayMenuTranslation();
        public GncWrapperTranslation GncWrapper = new GncWrapperTranslation();

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

    partial class ConfigFormTranslation
    {
        [LingoInGroup(TrGroup.SettingsDialog)]
        public TrString Warning_PortValue = "The \"Port\" field must contain an integer between 1 and 65535";
        [LingoInGroup(TrGroup.SettingsDialog)]
        public TrString OpenDialog_Title = "Select a GnuCash file";
    }

    partial class TrayFormTranslation
    {
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

    [LingoStringClass]
    class GncWrapperTranslation
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
}