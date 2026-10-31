using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace ConsoleStoreApp
{
    public class ProductServiceAdo : IProductService
    {
        private readonly string _connectionString;

        public ProductServiceAdo(string connectionString)
        {
            _connectionString = connectionString;
        }

        // INSERT operation using ExecuteNonQuery and Parameterized Queries
        public void AddProduct(string name, decimal price, int quantity, ProductCategory category)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            string sql = @"INSERT INTO Products (Name, Price, Quantity, Category) 
                           VALUES (@Name, @Price, @Quantity, @Category)";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@Price", price);
            command.Parameters.AddWithValue("@Quantity", quantity);
            command.Parameters.AddWithValue("@Category", (int)category);

            command.ExecuteNonQuery();
        }

        // SELECT operation using ExecuteReader to read rows forward-only
        public IEnumerable<Product> GetAllProducts()
        {
            var products = new List<Product>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            string sql = "SELECT Id, Name, Price, Quantity, Category FROM Products";
            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                products.Add(new Product(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetDecimal(2),
                    reader.GetInt32(3),
                    (ProductCategory)reader.GetInt32(4)
                ));
            }

            return products;
        }

        public Product GetProductById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            string sql = "SELECT Id, Name, Price, Quantity, Category FROM Products WHERE Id = @Id";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Product(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetDecimal(2),
                    reader.GetInt32(3),
                    (ProductCategory)reader.GetInt32(4)
                );
            }

            throw new EntityNotFoundException($"Product with ID {id} was not found.");
        }

        public void UpdateProduct(int id, string name, decimal price, int quantity)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            string sql = @"UPDATE Products 
                           SET Name = @Name, Price = @Price, Quantity = @Quantity 
                           WHERE Id = @Id";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@Price", price);
            command.Parameters.AddWithValue("@Quantity", quantity);

            int rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected == 0)
                throw new EntityNotFoundException($"Product with ID {id} was not found.");
        }

        public void DeleteProduct(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            string sql = "DELETE FROM Products WHERE Id = @Id";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            int rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected == 0)
                throw new EntityNotFoundException($"Product with ID {id} was not found.");
        }

        public IEnumerable<Product> SearchProducts(string query)
        {
            var products = new List<Product>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            string sql = @"SELECT Id, Name, Price, Quantity, Category 
                           FROM Products 
                           WHERE Id LIKE @Query OR Name LIKE @LikeQuery";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Query", query);
            command.Parameters.AddWithValue("@LikeQuery", $"%{query}%");

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                products.Add(new Product(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetDecimal(2),
                    reader.GetInt32(3),
                    (ProductCategory)reader.GetInt32(4)
                ));
            }

            return products;
        }

        public IEnumerable<IGrouping<ProductCategory, Product>> GetProductsGroupedByCategory()
        {
            return GetAllProducts().GroupBy(p => p.Category);
        }
    }
}