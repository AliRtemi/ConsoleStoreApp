using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleStoreApp
{
    public class OrderService : IOrderService
    {
        private readonly List<Order> _orders;
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;
        private int _nextOrderId = 1;

        public OrderService(List<Order> orders, IProductService productService, ICustomerService customerService)
        {
            _orders = orders ?? new List<Order>();
            _productService = productService;
            _customerService = customerService;

            if (_orders.Any())
            {
                _nextOrderId = _orders.Max(o => o.Id) + 1;
            }
        }

        public Order CreateOrder(int customerId, List<(int productId, int quantity)> itemsToBuy, string? promoCode)
        {
            // Verify Customer exists using CustomerService
            Customer customer = _customerService.GetCustomerById(customerId);

            Order newOrder = new Order(_nextOrderId++, customer);

            foreach (var (productId, qty) in itemsToBuy)
            {
                // Verify Product exists using ProductService
                Product product = _productService.GetProductById(productId);

                if (qty > product.Quantity)
                {
                    throw new InsufficientStockException($"Insufficient stock! Only {product.Quantity} items available for '{product.Name}'.");
                }

                newOrder.Items.Add(new OrderItem(product, qty));
                product.Quantity -= qty;
            }

            if (!newOrder.Items.Any())
            {
                throw new InvalidOperationException("Cannot create an order with no items.");
            }

            // Apply Promo Code
            if (!string.IsNullOrWhiteSpace(promoCode))
            {
                switch (promoCode.Trim().ToUpper())
                {
                    case "SAVE10":
                        newOrder.DiscountPercentage = 10;
                        break;
                    case "SUPER20":
                        newOrder.DiscountPercentage = 20;
                        break;
                }
            }

            _orders.Add(newOrder);

            // Save changes to orders and updated inventory levels
            DataManager.SaveData("orders.json", _orders);
            DataManager.SaveData("products.json", _productService.GetAllProducts().ToList());

            return newOrder;
        }

        public IEnumerable<Order> GetAllOrders() => _orders;
    }
}