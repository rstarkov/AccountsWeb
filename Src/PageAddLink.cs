using RT.Servers;
using RT.Spinneret;
using RT.TagSoup;
using RT.Util;
using RT.Util.ExtensionMethods;

namespace AccountsWeb
{
    public class PageAddLink: WebPage
    {
        public PageAddLink(UrlPathRequest request, WebInterface iface)
            : base(request, iface)
        {
            EqatecAnalytics.Monitor.TrackFeature("PageAddLink.Load");
        }

        public override string GetTitle()
        {
            return Tr.PgAddLink.Title;
        }

        public override object GetContent()
        {
            var url = Request.GetValidated<string>("Href", href => true, Tr.PgAddLink.Validation_Href);
            var name = Request.Post["Name"].Value;

            if (name == null)
            {
                return
                    new FORM() { method = method.post, action = "/AddLink?Href=" + url.UrlEscape() }._(
                        Tr.PgAddLink.Prompt,
                        new INPUT() { name = "Name", type = itype.text },
                        " ",
                        new BUTTON(Tr.PgAddLink.CreateButton) { type = btype.submit }
                    );
            }
            else
            {
                Program.CurFile.UserLinks.Add(new UserLink() { Href = url, Name = name });
                Program.CurFile.SaveToFile();
                return new object[]
                {
                    new P(Tr.PgAddLink.CreatedMessage.Fmt(name)),
                    new P(new A(Tr.PgAddLink.CreatedReturnLink) { href = url })
                };
            }
        }
    }
}
