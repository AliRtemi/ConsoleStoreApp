using System.Collections.Generic;

namespace ConsoleStoreApp
{
    public interface IProductService
    {
        void AddProduct(string name, decimal price, int quantity, ProductCategory category);
        IEnumerable<Product> GetAllProducts();
        IEnumerable<Product> SearchProducts(string query);
        Product GetProductById(int id);
        void UpdateProduct(int id, string name, decimal price, int quantity);
        void DeleteProduct(int id);
        IEnumerable<IGrouping<ProductCategory, Product>> GetProductsGroupedByCategory();
    }
}