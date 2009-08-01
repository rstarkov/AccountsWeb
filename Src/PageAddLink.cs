using System.Linq;
using RT.Servers;
using RT.Spinneret;
using RT.TagSoup.HtmlTags;
using RT.Util.ExtensionMethods;

namespace AccountsWeb
{
    public class PageAddLink: WebPage
    {
        public PageAddLink(HttpRequest request, WebInterface iface)
            : base(request, iface)
        {
        }

        public override string GetTitle()
        {
            return "Add Link";
        }

        public override object GetContent()
        {
            var url = Request.GetValidated<string>("Href", href => true, "specified");
            var name = Request.Post["Name"].Value;

            if (name == null)
            {
                return
                    new FORM() { method = method.post, action = "/AddLink?Href=" + url.UrlEscape() }._(
                        "Enter a name for the new link: ",
                        new INPUT() { name = "Name", type = itype.text },
                        " ",
                        new BUTTON("Create link") { type = btype.submit }
                    );
            }
            else
            {
                Program.CurFile.UserLinks.Add(new UserLink() { Href = url, Name = name });
                Program.CurFile.SaveToFile();
                return new object[]
                {
                    new P("A new link named \"{0}\" has been created successfully!".Fmt(name)),
                    new P(new A("Return to the linked page") { href = url })
                };
            }
        }
    }
}
