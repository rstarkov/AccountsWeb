using System;
using System.Collections.Generic;
using System.Linq;
using csscript;
using CSScriptLibrary;
using GnuCashSharp;
using RT.Util.Serialization;

namespace AccountsWeb
{
    public class UserScripts
    {
        public string Code = "class MyScript : AccountsWebScript\n{\n    public override IEnumerable<ScriptDisplayValue> Compute()\n    {\n        yield return new ScriptDisplayValue(\"Example\", 47.25m);\n    }\n}";

        [ClassifyIgnore]
        public AccountsWebScript CompiledCode { get; private set; }
        [ClassifyIgnore]
        public List<ScriptDisplayValue> Values { get; private set; } = new List<ScriptDisplayValue>();
        [ClassifyIgnore]
        public List<string> Errors { get; private set; } = new List<string>();
        [ClassifyIgnore]
        public List<string> Warnings { get; private set; } = new List<string>();

        public void Recompile(GncFileWrapper file)
        {
            try
            {
                var usings = "using AccountsWeb; using GnuCashSharp; using System; using System.Collections.Generic; using System.Linq; using RT.TagSoup; ";
                CompiledCode = CSScript.Evaluator.LoadCode<AccountsWebScript>(usings + Code);
                CompiledCode.File = file;
                Errors.Clear();
                Warnings.Clear();
            }
            catch (CompilerException e)
            {
                CompiledCode = null;
                Values.Clear();
                Errors = (List<string>) e.Data["Errors"];
                Warnings = (List<string>) e.Data["Warnings"];
                Values.Clear();
                return;
            }

            try
            {
                Errors.Clear();
                Values = CompiledCode.Compute().ToList();
            }
            catch (Exception e)
            {
                Errors.Add($"{e.GetType().Name}: {e.Message}\n{e.StackTrace}");
                Values.Clear();
            }
        }
    }

    [Flags]
    public enum ScriptDisplayLocation
    {
        None = 0,
        Sidebar = 1 << 0,
    }

    public class ScriptDisplayValue
    {
        public string Name;
        public object TagSoupValue;
        public ScriptDisplayLocation Location;

        public ScriptDisplayValue(string name, decimal value, ScriptDisplayLocation location = ScriptDisplayLocation.None)
        {
            Name = name;
            Location = location;
            TagSoupValue = value.ToString("#,0.00");
        }

        public ScriptDisplayValue(string name, object tagSoupValue, ScriptDisplayLocation location = ScriptDisplayLocation.None)
        {
            Name = name;
            Location = location;
            TagSoupValue = tagSoupValue;
        }
    }

    public abstract class AccountsWebScript
    {
        public abstract IEnumerable<ScriptDisplayValue> Compute();

        public GncFileWrapper File { get; internal set; }
        public GncBook Book => File.Book;

        public GncAccount Acct(string path)
        {
            return Book.GetAccountByPath(path);
        }

        public object FmtCcy(GncMultiAmount amount, bool whole = false, bool isConverted = false)
        {
            return WebPage.FormatCcys(amount, isConverted, whole);
        }
    }
}
