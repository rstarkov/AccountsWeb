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

        public override object GetBody()
        {
            HtmlPrinter prn = new HtmlPrinter(new DIV());

            foreach (var ccy in Program.CurFile.Book.EnumCommodities().OrderBy(cmdty => cmdty.Identifier))
            {
                prn.AddTag(new H2(ccy.Identifier));
                if (ccy.Identifier == Program.CurFile.Book.BaseCurrencyId)
                {
                    prn.AddTag(new P("This is the base currency") { class_ = "info_msg" });
                }
                else
                {
                    ReportTable tbl = new ReportTable();
                    tbl.AddCol("Date");
                    tbl.AddCol("Rate");
                    tbl.AddCol("Inverse");
                    foreach (var pt in ccy.ExRate)
                        tbl.AddRow(pt.Key.ToShortDateString(), pt.Value.ToString("0.0000"), (1m / pt.Value).ToString("0.0000"));
                    prn.AddTag(tbl.GetHtml());
                }
            }

            return prn.GetHtml();
        }
    }
}
