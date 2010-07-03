using System;
using System.Collections.Generic;
using System.Linq;
using GnuCashSharp;
using RT.Servers;
using RT.Spinneret;
using RT.TagSoup.HtmlTags;
using RT.Util.ExtensionMethods;
using RT.TagSoup;
using System.Text.RegularExpressions;
using RT.Util.Streams;
using System.IO;

namespace AccountsWeb
{
    public class PageReconcile : WebPage
    {
        private GncAccount _account;

        public PageReconcile(HttpRequest request, WebInterface iface)
            : base(request, iface)
        {
        }

        public override string GetTitle()
        {
            return Tr.PgReconcile.Title;
        }

        private enum btn { None, ReconcilePreset, ReconcileCustom, DeletePreset }

        private class statementSplit
        {
            public DateTime Timestamp;
            public string Comment;
            public decimal Amount;
            public decimal? Balance;
        }

        private class entry
        {
            public decimal Amount;
            public statementSplit InStatement;
            public GncSplit InAccount;
        }

        public override object GetContent()
        {
            if (Request.Method == HttpMethod.Get && !Request.Get.ContainsKey("Acct"))
                return renderAccountSelection();
            else if (Request.Method == HttpMethod.Get)
                return renderInitialForm();
            else
                return renderFormWithResults();
        }

        private object renderAccountSelection()
        {
            return new object[]
            {
                new P("Choose an account to reconcile:"),
                new DIV() { class_ = "reconcile-accts" }._(Program.CurFile.Book.AccountRoot.EnumChildren().Select(acct => listAccount(acct, 0)))
            };
        }

        private IEnumerable<object> listAccount(GncAccount acct, int depth)
        {
            yield return new P("\u2003\u2003".Repeat(depth), new A(acct.Name) { href = Request.SameUrlExceptSet("Acct", acct.Path(":")) });
            foreach (var subacct in acct.EnumChildren())
                yield return listAccount(subacct, depth + 1);
        }

        private object renderInitialForm()
        {
            return generateForm("", "", "");
        }

        private object renderFormWithResults()
        {
            _account = GetAccount("Acct");
            var amtFmt = Request.GetValidated("AmtFmt", "#,0.00");

            var statementText = "";
            var regexText = "";
            var btnPressed = btn.None;
            if (Request.Method == HttpMethod.Post)
            {
                statementText = Request.Post["stmt"].Value;
                if (Request.Post["rec_custom"].Value == Tr.PgReconcile.BtnReconcile)
                {
                    btnPressed = btn.ReconcileCustom;
                    regexText = Request.Post["rec_custom_regex"].Value;
                }
                else
                {
                    btnPressed = Request.Post["rec_preset"].Value == Tr.PgReconcile.BtnReconcile ? btn.ReconcilePreset : btn.DeletePreset;
                    regexText = Request.Post["rec_preset_regex"].Value;
                }
            }

            string regexName = "";
            List<object> content = new List<object>();
            if (btnPressed != btn.None)
            {
                var regexSplit = Regex.Match(regexText, @"^(.*):\s*(.*)$");
                if (!regexSplit.Success)
                    throw new ValidationException(Tr.PgReconcile.Validation_ReconcilePreset, regexText, Tr.PgReconcile.Validation_ReconcilePresetMsg);
                regexName = regexSplit.Groups[1].Value;
                string regexValue = regexSplit.Groups[2].Value;
                var regex = new Regex(regexValue);

                if (btnPressed == btn.ReconcileCustom)
                {
                    Program.Settings.ReconcileRegexes[regexName] = regexValue;
                    Program.Settings.SaveQuiet();
                }
                else if (btnPressed == btn.DeletePreset)
                {
                    Program.Settings.ReconcileRegexes.Remove(regexName);
                    Program.Settings.SaveQuiet();
                }

                if (btnPressed == btn.ReconcilePreset || btnPressed == btn.ReconcileCustom)
                {
                    // Parse lines into transactions and create the augmented statement for display
                    var statementSplits = new List<statementSplit>();
                    var linesContent = statementText.Split('\n').Select(line =>
                        {
                            var m = regex.Match(line);

                            if (m.Success)
                                try
                                {
                                    statementSplits.Add(new statementSplit
                                    {
                                        Timestamp = parseapproxDate(m.Groups["date"]),
                                        Comment = m.Groups["comment"].Value,
                                        Amount = parseapproxAmount(m.Groups["dr"], m.Groups["cr"]),
                                        Balance = parseapproxBalance(m.Groups["balance"])
                                    });
                                }
                                catch (Exception e)
                                {
                                    return new P(line, new SPAN(" - " + e.Message) { style = "color: #f00" }) { class_ = "reconcile-match" };
                                }

                            if (!m.Success)
                                return new P(line) { class_ = "reconcile-nomatch" };
                            else
                                return new P(line) { class_ = "reconcile-match" };
                        }).ToArray();
                    content.Add(new H1(Tr.PgReconcile.HeaderParsedStatement));
                    content.Add(new DIV(linesContent) { class_ = "reconcile-parsed" });
                    statementSplits.Sort((spl1, spl2) => spl1.Timestamp.CompareTo(spl2.Timestamp));

                    if (statementSplits.Count == 0)
                    {
                        content.Add(new P("Warning: no valid transactions were extracted from the statement."));
                    }
                    else
                    {
                        // Group splits in statement and in account by amount
                        var fromDate = statementSplits.Min(tx => tx.Timestamp) - TimeSpan.FromDays(7);
                        var toDate = statementSplits.Max(tx => tx.Timestamp) + TimeSpan.FromDays(7);
                        var accountSplits = _account.EnumSplits(false).Where(spl => !spl.IsBalsnap && spl.Transaction.DatePosted >= fromDate && spl.Transaction.DatePosted <= toDate).ToList();
                        var groups = accountSplits
                            .Select(spl => spl.Amount.Quantity).Concat(statementSplits.Select(tx => tx.Amount))
                            .Distinct()
                            .ToDictionary(
                                amount => amount,
                                amount => new
                                    {
                                        InAccount = accountSplits.Where(spl => spl.Amount.Quantity == amount).ToList(),
                                        InStatement = statementSplits.Where(spl => spl.Amount == amount).ToList()
                                    });

                        // Match up the entries in each amount group and create a flat list
                        var entries = new List<entry>();
                        foreach (var kvp in groups)
                        {
                            var val = kvp.Value;
                            val.InAccount.Sort((spl1, spl2) => spl1.Transaction.DatePosted.CompareTo(spl2.Transaction.DatePosted));
                            val.InStatement.Sort((spl1, spl2) => spl1.Timestamp.CompareTo(spl2.Timestamp));

                            if (val.InAccount.Count == 1 && val.InStatement.Count == 1)
                            {
                                // just treat as a match
                                entries.Add(new entry { Amount = kvp.Key, InAccount = val.InAccount[0], InStatement = val.InStatement[0] });
                            }
                            else if (val.InAccount.Count == 0 || val.InStatement.Count == 0)
                            {
                                // no match
                                entries.AddRange(val.InAccount.Select(spl => new entry { Amount = kvp.Key, InAccount = spl }));
                                entries.AddRange(val.InStatement.Select(spl => new entry { Amount = kvp.Key, InStatement = spl }));
                            }
                            else
                            {
                                // both lists have at least one entry, one of them has at least two.

                                // First try an order-based approach
                                bool done = false;
                                if (val.InAccount.Count == val.InStatement.Count)
                                {
                                    var diffs = val.InStatement.Select(spl => spl.Timestamp).Zip(val.InAccount.Select(spl => spl.Transaction.DatePosted))
                                        .Select(tup => Math.Abs((tup.E1 - tup.E2).TotalDays));
                                    if (diffs.Max() <= 2)
                                    {
                                        entries.AddRange(val.InAccount.Zip(val.InStatement).Select(tup => new entry { Amount = kvp.Key, InAccount = tup.E1, InStatement = tup.E2 }));
                                        done = true;
                                    }
                                }

                                // If didn't work fall back onto a more generic algorithm that's prone to re-order matches
                                if (!done)
                                {
                                    var bestmatches1 =
                                        val.InStatement.Select(inst =>
                                            val.InAccount.Select((inacc, index) =>
                                                new { Index = index, Diff = Math.Abs((inacc.Transaction.DatePosted - inst.Timestamp).TotalDays) }
                                            )
                                            .OrderBy(v => v.Diff)
                                            .ToList()
                                        )
                                        .Select(options => (options.Count == 1)
                                            ? options[0].Index
                                            : (options[1].Diff / options[0].Diff >= 2.5) ? options[0].Index : -1)
                                        .ToList();

                                    var bestmatches2 =
                                        val.InAccount.Select(inacc =>
                                            val.InStatement.Select((inst, index) =>
                                                new { Index = index, Diff = Math.Abs((inacc.Transaction.DatePosted - inst.Timestamp).TotalDays) }
                                            )
                                            .OrderBy(v => v.Diff)
                                            .ToList()
                                        )
                                        .Select(options => (options.Count == 1)
                                            ? options[0].Index
                                            : (options[1].Diff / options[0].Diff >= 2.5) ? options[0].Index : -1)
                                        .ToList();

                                    // Pick any matches that were optimal in both directions
                                    var bestmatches = bestmatches1
                                        .Select((indexAcct, indexStmt) => new { indexAcct, indexStmt })
                                        .Where(m => m.indexAcct >= 0 && bestmatches2[m.indexAcct] == m.indexStmt)
                                        .ToList();

                                    entries.AddRange(bestmatches.Select(m => new entry { Amount = kvp.Key, InAccount = val.InAccount[m.indexAcct], InStatement = val.InStatement[m.indexStmt] }));
                                    entries.AddRange(val.InAccount.Where((spl, index) => !bestmatches.Any(m => m.indexAcct == index)).Select(spl => new entry { Amount = kvp.Key, InAccount = spl }));
                                    entries.AddRange(val.InStatement.Where((spl, index) => !bestmatches.Any(m => m.indexStmt == index)).Select(spl => new entry { Amount = kvp.Key, InStatement = spl }));
                                }
                            }
                        }

                        // Sort the entries so as to preserve the original account split ordering
                        entries.Sort((e1, e2) =>
                            {
                                if (e1.InAccount != null && e2.InAccount != null)
                                    return accountSplits.IndexOf(e1.InAccount).CompareTo(accountSplits.IndexOf(e2.InAccount));
                                else if (e1.InAccount == null && e2.InAccount != null)
                                    return e1.InStatement.Timestamp.CompareTo(e2.InAccount.Transaction.DatePosted);
                                else if (e1.InAccount != null && e2.InAccount == null)
                                    return e1.InAccount.Transaction.DatePosted.CompareTo(e2.InStatement.Timestamp);
                                else
                                    return e1.InStatement.Timestamp.CompareTo(e2.InStatement.Timestamp);
                            });

                        // Create the table summarising the results
                        var table = new ReportTable();
                        var colAcctDate = table.AddCol(Tr.PgReconcile.ColAcctDate);
                        var colStmtDate = table.AddCol(Tr.PgReconcile.ColStmtDate);
                        var colComment = table.AddCol(Tr.PgReconcile.ColComment);
                        var colAmount = table.AddCol(Tr.PgReconcile.ColAmount);
                        var colBalance = table.AddCol(Tr.PgReconcile.ColBalance);

                        foreach (var entry in entries)
                        {
                            string type;
                            if (entry.InAccount == null || entry.InStatement == null)
                                type = "none";
                            else if (Math.Abs((entry.InAccount.Transaction.DatePosted - entry.InStatement.Timestamp).TotalDays) <= 3)
                                type = "perfect";
                            else
                                type = "uncertain";
                            var row = table.AddRow();
                            var cellcss = "reconcile-match-" + type;
                            if (entry.InAccount != null)
                                row.SetValue(colAcctDate, entry.InAccount.Transaction.DatePosted.ToShortDateString(), cellcss);
                            if (entry.InStatement != null)
                                row.SetValue(colStmtDate, entry.InStatement.Timestamp.ToShortDateString(), cellcss);
                            row.SetValue(colComment, new object[] { entry.InAccount == null ? null : new object[] { entry.InAccount.ReadableDescAndMemo, new BR() }, entry.InStatement == null ? null : entry.InStatement.Comment }, cellcss);
                            row.SetValue(colAmount, entry.Amount.ToString(amtFmt), cellcss);

                            if (entry.InAccount != null)
                            {
                                var balance = entry.InAccount.AccountBalanceAfter;
                                var matchingBalanceSplits = statementSplits.Where(spl => spl.Balance != null && spl.Balance.Value == balance);
                                bool matchingBalance = false;
                                foreach (var matchingBalanceSplit in matchingBalanceSplits)
                                {
                                    // Verify that the InStatement entries up to now and the statement splits up to the matching split are exactly the same set.
                                    var balanceContributingStmtStmt = statementSplits.TakeWhile(spl => spl != matchingBalanceSplit).Concat(matchingBalanceSplit).ToList();
                                    var balanceContributingStmtAcct = entries.TakeWhile(e => e != entry).Concat(entry).Where(e => e.InStatement != null).Select(e => e.InStatement).ToList();
                                    if (balanceContributingStmtStmt.Count != balanceContributingStmtAcct.Count)
                                        continue;
                                    if (balanceContributingStmtAcct.Any(spl => !balanceContributingStmtStmt.Contains(spl)))
                                        continue;
                                    if (balanceContributingStmtStmt.Any(spl => !balanceContributingStmtAcct.Contains(spl)))
                                        continue;
                                    matchingBalance = true;
                                    break;
                                }
                                row.SetValue(colBalance, balance.ToString(amtFmt), cellcss + (matchingBalance ? " reconcile-balance-match" : ""));
                            }
                        }

                        content.Add(new H1(Tr.PgReconcile.HeaderReconciledTransactions));
                        content.Add(table.GetHtml());
                    }
                }
            }

            return new object[]
            {
                generateForm(statementText, regexName, regexText),
                (btnPressed != btn.ReconcilePreset && btnPressed != btn.ReconcileCustom)
                    ? null
                    : content
            };
        }

        private object generateForm(string statementText, string regexName, string regexText)
        {
            return new FORM() { action = Request.SameUrlExcept(), method = method.post }._(
                new TEXTAREA(statementText) { cols = 100, rows = 20, name = "stmt" },

                new BR(),
                Tr.PgReconcile.FormUseExistingPreset,
                new SELECT() { name = "rec_preset_regex", style = "min-width: 30em" }._(
                Program.Settings.ReconcileRegexes.Select(kvp => new OPTION(kvp.Key + ": " + kvp.Value) { selected = kvp.Key == regexName ? "selected" : null })
                ),
                new INPUT() { type = itype.submit, name = "rec_preset", value = Tr.PgReconcile.BtnReconcile }, new INPUT() { type = itype.submit, name = "rec_preset", value = Tr.PgReconcile.BtnDelete },

                new BR(),
                Tr.PgReconcile.FormCreateModifyPreset,
                new INPUT() { type = itype.text, name = "rec_custom_regex", value = regexText, style = "min-width: 30em" },
                new INPUT() { type = itype.submit, name = "rec_custom", value = Tr.PgReconcile.BtnReconcile }
            );
        }

        private decimal? parseapproxBalance(Group match)
        {
            if (numberPresent(match))
                return parseapproxNumber(match.Value);
            else
                return null;
        }

        private decimal parseapproxAmount(Group matchDr, Group matchCr)
        {
            bool haveDr = numberPresent(matchDr);
            bool haveCr = numberPresent(matchCr);
            if (haveDr == haveCr)
                throw new Exception(Tr.PgReconcile.Validation_RequireDrOrCr);
            if (haveDr)
                return parseapproxNumber(matchDr.Value);
            else
                return -parseapproxNumber(matchCr.Value);
        }

        private decimal parseapproxNumber(string num)
        {
            num = num.Trim().Replace("£", "").Replace("$", "").Replace("€", "");
            return decimal.Parse(num.Replace(",", ""));
        }

        private DateTime parseapproxDate(Group match)
        {
            return DateTime.Parse(match.Value);
        }

        private bool numberPresent(Group numgroup)
        {
            if (!numgroup.Success)
                return false;
            if (numgroup.Value.Trim() == "-")
                return false;
            if (numgroup.Value.Trim() == "")
                return false;
            return true;
        }
    }
}
