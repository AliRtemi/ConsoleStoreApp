using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleStoreApp
{
    internal class OrderItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public OrderItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
            UnitPrice = product.Price;
        }

        public decimal TotalPrice => UnitPrice * Quantity;
    }
}
