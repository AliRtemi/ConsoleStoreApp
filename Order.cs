using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleStoreApp
{
    public class Order
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        // Order-level promo code percentage (e.g., 10 for 10% off)
        public decimal DiscountPercentage { get; set; } = 0;

        public Order(int id, Customer customer)
        {
            Id = id;
            Customer = customer;
            OrderDate = DateTime.Now;
        }

        public decimal SubTotal => Items.Sum(i => i.TotalPrice);
        public decimal DiscountAmount => SubTotal * (DiscountPercentage / 100m);
        public decimal TotalAmount => SubTotal - DiscountAmount;
    }
}
