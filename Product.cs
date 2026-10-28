using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleStoreApp
{
    internal class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public Product(int id, string name, decimal price, int quantity)
        {
            Id = id;
            Name = name;
            Price = price;
            Quantity = quantity;
        }

        public decimal TotalValue => Price * Quantity;
    }
}
