using System.Linq;
using RT.Servers;
using RT.Spinneret;
using RT.TagSoup.HtmlTags;
using System.Collections.Generic;
using RT.TagSoup;

namespace AccountsWeb
{
    public class PageExRates : WebPage
    {
        public PageExRates(HttpRequest request, WebInterface iface)
            : base(request, iface)
        {
        }

        public override string GetTitle()
        {
            return Tr.PgExRates.Title;
        }

        public override object GetContent()
        {
            DIV div = new DIV();
            foreach (var ccy in Program.CurFile.Book.EnumCommodities().OrderBy(cmdty => cmdty.Identifier))
            {
                div.Add(new H2(ccy.Identifier));
                if (ccy.Identifier == Program.CurFile.Book.BaseCurrencyId)
                {
                    div.Add(new P(Tr.PgExRates.BaseCurrencyMessage) { class_ = "aw-info-msg" });
                }
                else
                {
                    ReportTable tbl = new ReportTable();
                    tbl.AddCol(Tr.PgExRates.ColDate);
                    tbl.AddCol(Tr.PgExRates.ColRate);
                    tbl.AddCol(Tr.PgExRates.ColRateInverse);
                    foreach (var pt in ccy.ExRate)
                        tbl.AddRow(null, pt.Key.ToShortDateString(), pt.Value.ToString("0.0000"), (1m / pt.Value).ToString("0.0000"));
                    div.Add(tbl.GetHtml());
                }
            }

            return div;
        }
    }
}
