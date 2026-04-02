using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFlow.Models
{
    public class Order
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public List<OrderItem> Items { get; set; }
        public DateTime Date { get; set; }
        public OrderStatus Status { get; set; }

        public bool IsValid { get; set; }
        public List<string> ValidationErrors { get; set; } = new();
        public decimal TotalAmount => Items.Sum(i => i.TotalPrice);
    }
}
