using RT.Servers;
using RT.TagSoup;
using RT.Util;

namespace AccountsWeb;

public class PageCalculationsEdit : WebPage
{
    public PageCalculationsEdit(HttpRequest request, WebInterface iface)
        : base(request, iface)
    {
    }

    public override string GetTitle()
    {
        return Program.Tr.PgCalculations.EditTitle;
    }

    public override object GetContent()
    {
        if (Request.Method == HttpMethod.Post)
        {
            Program.CurFile.UserScripts.Code = Request.Post["code"].Value;
            Program.CurFile.SaveToFile();
            Program.CurFile.UserScripts.Recompile(Program.CurFile);
        }

        return Ut.NewArray<object>(
            new PageCalculations(Request, (WebInterface) Interface).GetContent(),
            new HR(),
            new FORM() { method = method.post, action = "/CalculationsEdit" }._(
                new BUTTON(Program.Tr.PgCalculations.EditSaveButton) { type = btype.submit },
                new DIV { style = "height: 600px; position: relative; margin-top: 5px;" }._(
                    new PRE(Program.CurFile.UserScripts.Code) { id = "editor", style = "position: absolute; margin: 0; top: 0; bottom: 0; left: 0; right: 0;" }
                ),
                new TEXTAREA { id = "code", name = "code", style = "display:none" },
                new SCRIPT { src = "Static/ace-editor/ace.js" },
                new SCRIPTLiteral(@"
                        var editor = ace.edit('editor');
                        editor.setTheme('ace/theme/cobalt');
                        editor.session.setMode('ace/mode/csharp');
                        editor.setShowPrintMargin(false);
                        editor.setDisplayIndentGuides(false);
                        editor.setFontSize(14);
                        editor.setBehavioursEnabled(false);
                        editor.setWrapBehavioursEnabled(false);
                        var textarea = document.getElementById('code');
                        textarea.value = editor.getSession().getValue();
                        editor.getSession().on('change', function () {
                            textarea.value = editor.getSession().getValue();
                        });
                    ")
            )
        );
    }
}
