using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OrderFlow.Models
{
    public class Order
    {
        [JsonPropertyName("order_id")]
        [XmlAttribute]
        public int Id { get; set; }
        
        [XmlElement("customer_data")]
        public Customer Customer { get; set; }
        public List<OrderItem> Items { get; set; }
        public DateTime Date { get; set; }
        public OrderStatus Status { get; set; }

        public bool IsValid { get; set; }
        public List<string> ValidationErrors { get; set; } = new();
        
        [JsonIgnore]
        [XmlIgnore]
        public decimal TotalAmount => Items.Sum(i => i.TotalPrice);
    }
}
