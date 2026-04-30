using OrderFlow.Models;

namespace OrderFlow.Data;

public static class SampleData
{
    public static List<Product> Products = new List<Product> {
        new Product{ Id=1, Name="Item1", Price=100, Category="Cat1", Stock=1 },
        new Product{ Id=2, Name="Item2", Price=200, Category="Cat1", Stock=10 },
        new Product{ Id=3, Name="Item3", Price=300, Category="Cat2", Stock=10 },
        new Product{ Id=4, Name="Item4", Price=400, Category="Cat2", Stock=10 },
        new Product{ Id=5, Name="Item5", Price=500, Category="Cat3", Stock=10 },
        new Product{ Id=6, Name="Item6", Price=600, Category="Cat3", Stock=10 }
    };

    public static List<Customer> Customers = new List<Customer> {
        new Customer{ Id=1, Name="Customer1", City="City1", IsVip=true},
        new Customer{ Id=2, Name="Customer2", City="City2", IsVip=false},
        new Customer{ Id=3, Name="Customer3", City="City2", IsVip=false},
        new Customer{ Id=4, Name="Customer4", City="City4", IsVip=false}
    };

    public static List<Order> Orders = new List<Order> {
        new Order{ Id=1, Customer=Customers[0], Date=DateTime.Now.AddDays(-2), Status=OrderStatus.New,
            Items=new List<OrderItem>{
                new OrderItem{ Product=Products[0], Quantity=1 },
                new OrderItem{ Product=Products[4], Quantity=2 }
            }
        },
        new Order{ Id=2, Customer=Customers[1], Date=DateTime.Now.AddDays(-3), Status=OrderStatus.Validated,
            Items=new List<OrderItem>{
                new OrderItem{ Product=Products[2], Quantity=2 },
                new OrderItem{ Product=Products[3], Quantity=1 }
            }
        },
        new Order{ Id=3, Customer=Customers[2], Date=DateTime.Now.AddDays(-2), Status=OrderStatus.Completed,
            Items=new List<OrderItem>{
                new OrderItem{ Product=Products[0], Quantity=4 }
            }
        },
        new Order{ Id=4, Customer=Customers[3], Date=DateTime.Now.AddDays(-2), Status=OrderStatus.Processing,
            Items=new List<OrderItem>{
                new OrderItem{ Product=Products[4], Quantity=2 },
                new OrderItem{ Product=Products[3], Quantity=1 }
            }
        },
        new Order{ Id=5, Customer=Customers[0], Date=DateTime.Now.AddDays(-1), Status=OrderStatus.Cancelled,
            Items=new List<OrderItem>{
                new OrderItem{ Product=Products[0], Quantity=1 }
            }
        },
        new Order{ Id=6, Customer=Customers[2], Date=DateTime.Now.AddDays(-2), Status=OrderStatus.New,
            Items=new List<OrderItem>{
                new OrderItem{ Product=Products[4], Quantity=5 }
            }
        }
    };
}