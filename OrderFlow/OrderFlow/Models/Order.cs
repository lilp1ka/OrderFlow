using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFlow.Models
{
    internal class Order
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public List<OrderItem> Items { get; set; }
        public DateTime Date { get; set; }
        public OrderStatus Status { get; set; }

        public decimal TotalAmount => Items.Sum(i => i.TotalPrice);
    }
}
