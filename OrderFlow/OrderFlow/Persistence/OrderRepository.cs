using System.Text.Json;
using System.Xml.Serialization;
using OrderFlow.Models;

namespace OrderFlow.Persistence;

public class OrderRepository
{
    private JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    
    public async Task SaveToJsonAsync(IEnumerable<Order> orders, string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        await using var stream = new FileStream(path, FileMode.Create);
        await JsonSerializer.SerializeAsync(stream, orders, _jsonOptions);
    }

    public async Task<List<Order>> LoadFromJsonAsync(string path)
    {
        if (!File.Exists(path))
            return new List<Order>();

        await using var stream = new FileStream(path, FileMode.Open);
        var data = await JsonSerializer.DeserializeAsync<List<Order>>(stream, _jsonOptions);

        return data ?? new List<Order>();
    }

    
    

    public async Task SaveToXmlAsync(IEnumerable<Order> orders, string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        var serializer = new XmlSerializer(typeof(List<Order>));

        await using var stream = new FileStream(path, FileMode.Create);
        serializer.Serialize(stream, orders.ToList());
    }

    public async Task<List<Order>> LoadFromXmlAsync(string path)
    {
        if (!File.Exists(path))
            return new List<Order>();

        var serializer = new XmlSerializer(typeof(List<Order>));

        await using var stream = new FileStream(path, FileMode.Open);
        return (List<Order>)serializer.Deserialize(stream)!;
    }
}