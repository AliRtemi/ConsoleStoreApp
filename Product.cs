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
        public ProductCategory Category { get; set; }
        public decimal? DiscountPercentage { get; set; }

        public Product(int id, string name, decimal price, int quantity, ProductCategory category, decimal? discountPercentage = null)
        {
            Id = id;
            Name = name;
            Price = price;
            Quantity = quantity;
            Category = category;
            DiscountPercentage = discountPercentage;
        }

        // Effective Price takes discount into account
        public decimal EffectivePrice => DiscountPercentage.HasValue && DiscountPercentage.Value > 0
            ? Price * (1 - (DiscountPercentage.Value / 100m))
            : Price;

        public decimal TotalValue => Price * Quantity;
    }
}
