using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleStoreApp
{
    public class ProductService : IProductService
    {
        private readonly List<Product> _products;
        private int _nextProductId = 1;

        public ProductService(List<Product> products)
        {
            _products = products ?? new List<Product>();
            if (_products.Any())
            {
                _nextProductId = _products.Max(p => p.Id) + 1;
            }
        }

        public void AddProduct(string name, decimal price, int quantity, ProductCategory category)
        {
            var product = new Product(_nextProductId++, name, price, quantity, category);
            _products.Add(product);
            DataManager.SaveData("products.json", _products);
        }

        public IEnumerable<Product> GetAllProducts() => _products;

        public IEnumerable<Product> SearchProducts(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<Product>();

            return _products.Where(p =>
                p.Id.ToString() == query ||
                p.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        public Product GetProductById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id)
                ?? throw new EntityNotFoundException($"Product with ID {id} was not found.");
        }

        public void UpdateProduct(int id, string name, decimal price, int quantity)
        {
            var product = GetProductById(id);
            product.Name = name;
            product.Price = price;
            product.Quantity = quantity;

            DataManager.SaveData("products.json", _products);
        }

        public void DeleteProduct(int id)
        {
            var product = GetProductById(id);
            _products.Remove(product);
            DataManager.SaveData("products.json", _products);
        }

        public IEnumerable<IGrouping<ProductCategory, Product>> GetProductsGroupedByCategory()
        {
            return _products.GroupBy(p => p.Category);
        }
    }
}