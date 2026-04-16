using System.Xml.Linq;
using OrderFlow.Models;

namespace OrderFlow.Services;

public class XmlReportBuilder
{
    public XDocument BuildReport(IEnumerable<Order> orders)
    {
        var report =
            new XDocument(
                new XElement("report",
                    new XAttribute("generated", DateTime.Now),

                    new XElement("summary",
                        new XAttribute("totalOrders", orders.Count()),
                        new XAttribute("totalRevenue", orders.Sum(o => o.TotalAmount))
                    ),

                    new XElement("byStatus",
                        orders
                            .GroupBy(o => o.Status)
                            .Select(g =>
                                new XElement("status",
                                    new XAttribute("name", g.Key),
                                    new XAttribute("count", g.Count()),
                                    new XAttribute("revenue", g.Sum(o => o.TotalAmount))
                                )
                            )
                    ),

                    new XElement("byCustomer",
                        orders
                            .GroupBy(o => o.Customer)
                            .Select(g =>
                                new XElement("customer",
                                    new XAttribute("id", g.Key.Id),
                                    new XAttribute("name", g.Key.Name),
                                    new XAttribute("isVip", g.Key.IsVip),

                                    new XElement("orderCount", g.Count()),
                                    new XElement("totalSpent", g.Sum(o => o.TotalAmount)),

                                    new XElement("orders",
                                        g.Select(o =>
                                            new XElement("orderRef",
                                                new XAttribute("id", o.Id),
                                                new XAttribute("total", o.TotalAmount)
                                            )
                                        )
                                    )
                                )
                            )
                    )
                )
            );

        return report;
    }


    public async Task SaveReportAsync(XDocument report, string path)
    {
        var dir = Path.GetDirectoryName(path);

        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        Console.WriteLine("Saving to: " + Path.GetFullPath("data/report.xml"));
        
        await using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        await report.SaveAsync(fs, SaveOptions.None, default);
    }


    public async Task<IEnumerable<int>> FindHighValueOrderIdsAsync(string path, decimal threshold)
    {
        if (!File.Exists(path))
            return Enumerable.Empty<int>();

        await using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        var doc = await XDocument.LoadAsync(fs, LoadOptions.None, default);

        var ids = doc
            .Descendants("orderRef")
            .Where(x => decimal.Parse(x.Attribute("total")!.Value) > threshold)
            .Select(x => int.Parse(x.Attribute("id")!.Value));

        return ids;
    }
}