using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace ConsoleStoreApp
{
    public class OrderServiceAdo : IOrderService
    {
        private readonly string _connectionString;

        public OrderServiceAdo(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Order CreateOrder(int customerId, List<(int productId, int quantity)> itemsToBuy, string? promoCode)
        {
            if (itemsToBuy == null || !itemsToBuy.Any())
            {
                throw new InvalidOperationException("Cannot create an order with no items.");
            }

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            // 1. Fetch Customer directly using ADO.NET before starting transaction
            string customerSql = "SELECT Id, Name, Email, PhoneNumber FROM Customers WHERE Id = @CustomerId";
            Customer customer;

            using (var customerCmd = new SqlCommand(customerSql, connection))
            {
                customerCmd.Parameters.AddWithValue("@CustomerId", customerId);
                using var reader = customerCmd.ExecuteReader();
                if (!reader.Read())
                {
                    throw new EntityNotFoundException($"Customer with ID {customerId} was not found.");
                }

                customer = new Customer(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3)
                );
            }

            // 2. Start Transaction for inserting Order, Items, and updating Product Stock
            using var transaction = connection.BeginTransaction();
            try
            {
                decimal discount = GetDiscountPercentage(promoCode);

                // Step A: Insert Order record and get newly generated Order ID
                string insertOrderSql = @"INSERT INTO Orders (CustomerId, OrderDate, DiscountPercentage) 
                                 OUTPUT INSERTED.Id 
                                 VALUES (@CustomerId, @OrderDate, @Discount)";

                using var orderCmd = new SqlCommand(insertOrderSql, connection, transaction);
                orderCmd.Parameters.AddWithValue("@CustomerId", customerId);
                orderCmd.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                orderCmd.Parameters.AddWithValue("@Discount", discount);

                int newOrderId = (int)orderCmd.ExecuteScalar();

                Order newOrder = new Order(newOrderId, customer)
                {
                    DiscountPercentage = discount
                };

                // Step B: Process each line item
                foreach (var (productId, qty) in itemsToBuy)
                {
                    // Fetch Product directly to verify existence & stock level
                    string productSql = "SELECT Id, Name, Price, Quantity, Category FROM Products WHERE Id = @ProductId";
                    Product product;

                    using (var productCmd = new SqlCommand(productSql, connection, transaction))
                    {
                        productCmd.Parameters.AddWithValue("@ProductId", productId);
                        using var reader = productCmd.ExecuteReader();
                        if (!reader.Read())
                        {
                            throw new EntityNotFoundException($"Product with ID {productId} was not found.");
                        }

                        product = new Product(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetDecimal(2),
                            reader.GetInt32(3),
                            (ProductCategory)reader.GetInt32(4)
                        );
                    }

                    // Check stock availability
                    if (qty > product.Quantity)
                    {
                        throw new InsufficientStockException($"Insufficient stock for '{product.Name}'! Requested: {qty}, In Stock: {product.Quantity}.");
                    }

                    // Insert into OrderItems table
                    string insertItemSql = @"INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) 
                                     VALUES (@OrderId, @ProductId, @Qty, @Price)";
                    using var itemCmd = new SqlCommand(insertItemSql, connection, transaction);
                    itemCmd.Parameters.AddWithValue("@OrderId", newOrderId);
                    itemCmd.Parameters.AddWithValue("@ProductId", productId);
                    itemCmd.Parameters.AddWithValue("@Qty", qty);
                    itemCmd.Parameters.AddWithValue("@Price", product.EffectivePrice);
                    itemCmd.ExecuteNonQuery();

                    // Update inventory stock in Products table
                    string updateStockSql = "UPDATE Products SET Quantity = Quantity - @Qty WHERE Id = @ProductId";
                    using var stockCmd = new SqlCommand(updateStockSql, connection, transaction);
                    stockCmd.Parameters.AddWithValue("@Qty", qty);
                    stockCmd.Parameters.AddWithValue("@ProductId", productId);
                    stockCmd.ExecuteNonQuery();

                    // Attach item to newOrder instance
                    newOrder.Items.Add(new OrderItem(product, qty));
                }

                // Commit transaction if all operations succeed
                transaction.Commit();
                return newOrder;
            }
            catch
            {
                // Rollback all changes if any error occurs
                transaction.Rollback();
                throw;
            }
        }

        public IEnumerable<Order> GetAllOrders()
        {
            var ordersDict = new Dictionary<int, Order>();

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            // SQL JOIN querying Orders, Customers, OrderItems, and Products
            string sql = @"
                SELECT 
                    o.Id AS OrderId, o.OrderDate, o.DiscountPercentage,
                    c.Id AS CustomerId, c.Name AS CustomerName, c.Email, c.PhoneNumber,
                    oi.Quantity, oi.UnitPrice,
                    p.Id AS ProductId, p.Name AS ProductName, p.Price, p.Quantity AS ProductQty, p.Category
                FROM Orders o
                INNER JOIN Customers c ON o.CustomerId = c.Id
                INNER JOIN OrderItems oi ON o.Id = oi.OrderId
                INNER JOIN Products p ON oi.ProductId = p.Id
                ORDER BY o.Id DESC";

            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                int orderId = reader.GetInt32(0);

                if (!ordersDict.TryGetValue(orderId, out var order))
                {
                    var customer = new Customer(
                        reader.GetInt32(3),
                        reader.GetString(4),
                        reader.GetString(5),
                        reader.GetString(6)
                    );

                    order = new Order(orderId, customer)
                    {
                        OrderDate = reader.GetDateTime(1),
                        DiscountPercentage = reader.GetDecimal(2)
                    };

                    ordersDict.Add(orderId, order);
                }

                var product = new Product(
                    reader.GetInt32(9),
                    reader.GetString(10),
                    reader.GetDecimal(11),
                    reader.GetInt32(12),
                    (ProductCategory)reader.GetInt32(13)
                );

                int itemQty = reader.GetInt32(7);
                var orderItem = new OrderItem(product, itemQty)
                {
                    UnitPrice = reader.GetDecimal(8)
                };

                order.Items.Add(orderItem);
            }

            return ordersDict.Values;
        }

        private static decimal GetDiscountPercentage(string? promoCode)
        {
            if (string.IsNullOrWhiteSpace(promoCode)) return 0;

            return promoCode.Trim().ToUpper() switch
            {
                "SAVE10" => 10m,
                "SUPER20" => 20m,
                _ => 0m
            };
        }
    }
}