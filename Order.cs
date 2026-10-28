using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleStoreApp
{
    internal class Order
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        public Order(int id, Customer customer)
        {
            Id = id;
            Customer = customer;
            OrderDate = DateTime.Now;
        }

        public decimal TotalAmount => Items.Sum(i => i.TotalPrice);
    }
}
