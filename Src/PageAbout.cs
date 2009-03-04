using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Servers;
using RT.TagSoup;
using RT.TagSoup.HtmlTags;
using RT.Util.ExtensionMethods;

namespace AccountsWeb
{
    public class PageAbout: Page
    {
        public PageAbout(HttpRequest request)
            : base(request)
        {
        }

        public override string GetBaseUrl()
        {
            return "/About";
        }

        public override string GetTitle()
        {
            return "About AccountsWeb";
        }

        public override IEnumerable<string> GetCss()
        {
            yield break;
        }

        public override IEnumerable<Tag> GetBody()
        {
            yield return new DIV(
                new P("Version {0}".Fmt(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version)),
                new P("Copyright (C) 2009 Roman Starkov")
            );
        }

        public override bool IgnoreGlobalMessage
        {
            get { return true; }
        }
    }
}
