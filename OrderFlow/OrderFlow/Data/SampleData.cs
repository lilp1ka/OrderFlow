using OrderFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFlow.Data
{
    internal class SampleData
    {
        public static List<Product> Products = new List<Product> {
            new Product{ Id = 1, Name = "Item1", Price = 111, Category ="Category1"},
            new Product{ Id = 2, Name = "Item2", Price = 222, Category ="Category1"},
            new Product{ Id = 3, Name = "Item3", Price = 333, Category ="Category2"},
            new Product{ Id = 4, Name = "Item4", Price = 444, Category ="Category1"},
            new Product{ Id = 5, Name = "Item5", Price = 555, Category ="Category1"},
            new Product{ Id = 6, Name = "Item6", Price = 666, Category ="Category1"}
        };


        public static List<Customer> Customers = new List<Customer> {
            new Customer{ Id = 1, Name = "Customer1",City = "City1", IsVip = true},
            new Customer{ Id = 2, Name = "Customer2",City = "City2", IsVip = false},
            new Customer{ Id = 3, Name = "Customer3",City = "City2", IsVip = false},
            new Customer{ Id = 4, Name = "Customer4",City = "City4", IsVip = false}
        };

    }
}
