using System.Linq;
using RT.Servers;
using RT.TagSoup;
using RT.Util;

namespace AccountsWeb
{
    public class PageCalculations : WebPage
    {
        public PageCalculations(HttpRequest request, WebInterface iface)
            : base(request, iface)
        {
        }

        public override string GetTitle()
        {
            return Program.Tr.PgCalculations.Title;
        }

        public override object GetContent()
        {
            var scripts = Program.CurFile.UserScripts;
            if (scripts.Errors.Count > 0)
            {
                return Ut.NewArray<object>(
                    new P(new B(Program.Tr.PgCalculations.UserScriptErrorsHeading)),
                    new UL(scripts.Errors.Select(err => new LI(new PRE(err))))
                );
            }
            else
            {
                return Ut.NewArray<object>(
                    scripts.Values.Select(v => new object[] { new H2(v.Name), new DIV(v.TagSoupValue) }),
                    scripts.Warnings.Count == 0 ? null : new object[] { new P(new B(Program.Tr.PgCalculations.UserScriptWarningsHeading)), new UL(scripts.Warnings.Select(w => new LI(w))) }
                );
            }
        }
    }
}
