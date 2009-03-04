using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Servers;
using RT.TagSoup;
using RT.TagSoup.HtmlTags;

namespace AccountsWeb
{
    public class PageExRates: Page
    {
        public PageExRates(HttpRequest request)
            : base(request)
        {
        }

        public override string GetBaseUrl()
        {
            return "/ExRates";
        }

        public override string GetTitle()
        {
            return "Exchange Rates";
        }

        public override IEnumerable<string> GetCss()
        {
            yield return "ExRates.css";
        }

        public override IEnumerable<Tag> GetBody()
        {
            HtmlPrinter prn = new HtmlPrinter(new DIV());

            foreach (var ccy in Program.CurFile.Book.EnumCommodities().OrderBy(cmdty => cmdty.Identifier))
            {
                prn.AddTag(new H2(ccy.Identifier));
                if (ccy.Identifier == Program.CurFile.Book.BaseCurrencyId)
                {
                    prn.AddTag(new P("This is the base currency"));
                }
                else
                {
                    prn.OpenTag(new TABLE());
                    foreach (var pt in ccy.ExRate)
                        prn.AddTag(new TR(new TD(pt.Key.ToShortDateString()), new TD(pt.Value.ToString())));
                    prn.CloseTag();
                }
            }

            yield return prn.GetHtml();
        }
    }
}
