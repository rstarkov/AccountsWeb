using System.Linq;
using RT.Servers;
using RT.Spinneret;
using RT.TagSoup.HtmlTags;

namespace AccountsWeb
{
    public class PageExRates: WebPage
    {
        public PageExRates(HttpRequest request, WebInterface iface)
            : base(request, iface)
        {
        }

        public override string GetTitle()
        {
            return "Exchange Rates";
        }

        public override object GetContent()
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
                        tbl.AddRow(null, pt.Key.ToShortDateString(), pt.Value.ToString("0.0000"), (1m / pt.Value).ToString("0.0000"));
                    prn.AddTag(tbl.GetHtml());
                }
            }

            return prn.GetHtml();
        }
    }
}
