using System;

namespace ConsoleStoreApp
{
    public class Program
    {
        private static IProductService productService = null!;
        private static ICustomerService customerService = null!;
        private static IOrderService orderService = null!;
        static void Main(string[] args)
        {
            string connectionString = "Server=PC-ALLO;Database=StoreDb;Trusted_Connection=True;TrustServerCertificate=True;";

            // Inject the ADO.NET connection string into service constructors
            productService = new ProductServiceAdo(connectionString);
            customerService = new CustomerServiceAdo(connectionString);
            orderService = new OrderServiceAdo(connectionString);

            bool exitRequested = false;

            while (!exitRequested) 
            {
                try
                {
                    Console.Clear();

                    Console.WriteLine("----------------");
                    Console.WriteLine("Console store application");
                    Console.WriteLine("=================================");
                    Console.WriteLine("1. Manage Products");
                    Console.WriteLine("2. Manage Customers");
                    Console.WriteLine("3. Manage Orders");
                    Console.WriteLine("4. Manage Reports");
                    Console.WriteLine("0. Exit");
                    Console.WriteLine("=================================");
                    Console.Write("Select an option: ");

                    string? input = Console.ReadLine()?.Trim();

                    Console.WriteLine();

                    switch (input)
                    {
                        case "1":
                            ManageProducts();
                            break;
                        case "2":
                            ManageCustomers();
                            break;
                        case "3":
                            ManageOrders();
                            break;
                        case "4":
                            ManageReports();
                            break;
                        case "0":
                            Console.WriteLine("Thank you for using Console Store App. Goodbye!");
                            exitRequested = true;
                            break;
                        default:
                            Console.WriteLine("Invalid option! Please select a valid number from the menu.");
                            break;
                    }

                    if (!exitRequested)
                    {
                        Console.WriteLine("\nPress any key to return to the main menu...");
                        Console.ReadKey();
                    }
                }
                catch (EntityNotFoundException ex)
                {
                    ConsoleHelper.WriteWarning(ex.Message);
                }
                catch (DuplicateEntityException ex)
                {
                    ConsoleHelper.WriteError(ex.Message);
                }
                catch (InsufficientStockException ex)
                {
                    ConsoleHelper.WriteError(ex.Message);
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteError($"An unexpected error occurred: {ex.Message}");
                }
            }
        }
        static void ManageProducts()
        {
            bool back = false;
            while (!back)
            {
                Console.Clear();
                Console.WriteLine("--- Product Management ---");
                Console.WriteLine("1. Add Product");
                Console.WriteLine("2. List All Products");
                Console.WriteLine("3. Search a product");
                Console.WriteLine("4. Update a product");
                Console.WriteLine("5. Delete a product");
                Console.WriteLine("6. List Products Grouped by Category");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("Select an option: ");

                string? choice = Console.ReadLine()?.Trim();
                switch (choice)
                {
                    case "1":
                        AddProduct();
                        break;
                    case "2":
                        ListProducts();
                        break;
                    case "3": 
                        SearchProducts();
                        break;
                    case "4":
                        UpdateProduct();
                        break;
                    case "5":
                        DeleteProduct();
                        break;
                    case "6":
                        ListProductsGroupedByCategory();
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                if (!back)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void AddProduct()
        {
            string name = InputValidator.ReadNonEmptyString("Enter Product Name: ");
            decimal price = InputValidator.ReadNonNegativeDecimal("Enter Price: ");
            int quantity = InputValidator.ReadNonNegativeInt("Enter Quantity: ");
            ProductCategory category = PromptCategory();

            // Delegate creation & saving to ProductService
            productService.AddProduct(name, price, quantity, category);
            ConsoleHelper.WriteSuccess($"Product '{name}' added successfully.");

        }

        static void ListProducts()
        {
            var allProducts = productService.GetAllProducts();

            if (!allProducts.Any())
            {
                Console.WriteLine("No products found.");
                return;
            }

            Console.WriteLine("\nID  | Name                 | Category    | Price      | Qty | Total Value");
            Console.WriteLine("-------------------------------------------------------------------------");
            foreach (var p in allProducts)
            {
                Console.WriteLine($"{p.Id,-3} | {p.Name,-20} | {p.Category,-11} | {p.Price,10:C} | {p.Quantity,3} | {p.TotalValue,11:C}");
            }

            decimal totalInventoryValue = allProducts.Sum(p => p.TotalValue);
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine($"Total Inventory Value: {totalInventoryValue:C}");
        }

        static void SearchProducts()
        {
            string query = InputValidator.ReadNonEmptyString("Enter product ID or name to search: ");

            var results = productService.SearchProducts(query);

            if (!results.Any())
            {
                Console.WriteLine($"No products found matching '{query}'.");
                return;
            }

            Console.WriteLine($"\nFound {results.Count()} matching product(s):");
            Console.WriteLine("ID  | Name                 | Category    | Price      | Qty | Total Value");
            Console.WriteLine("-------------------------------------------------------------------------");
            foreach (var p in results)
            {
                Console.WriteLine($"{p.Id,-3} | {p.Name,-20} | {p.Category,-11} | {p.Price,10:C} | {p.Quantity,3} | {p.TotalValue,11:C}");
            }
        }

        static void UpdateProduct()
        {
            int id = InputValidator.ReadNonNegativeInt("Enter Product ID to update: ");

            // Fetch product to display current name
            Product current = productService.GetProductById(id);

            Console.WriteLine($"Updating '{current.Name}':");
            string name = InputValidator.ReadNonEmptyString("Enter new Product Name: ");
            decimal price = InputValidator.ReadNonNegativeDecimal("Enter New Price: ");
            int quantity = InputValidator.ReadNonNegativeInt("Enter new Quantity: ");

            // Delegate update & saving to ProductService
            productService.UpdateProduct(id, name, price, quantity);
            ConsoleHelper.WriteSuccess($"Product ID {id} updated successfully!");
        }

        static void DeleteProduct()
        {
            int id = InputValidator.ReadNonNegativeInt("Enter Product ID to delete: ");

            Product product = productService.GetProductById(id);

            if (InputValidator.ReadConfirmation($"Are you sure you want to delete '{product.Name}'? (y/n): "))
            {
                productService.DeleteProduct(id);
                ConsoleHelper.WriteSuccess($"Product '{product.Name}' deleted successfully.");
            }
            else
            {
                Console.WriteLine("Deletion cancelled.");
            }
        }

        static void ManageCustomers()
        {
            bool back = false;
            while (!back)
            {
                Console.Clear();
                Console.WriteLine("--- Customer Management ---");
                Console.WriteLine("1. Add Customer");
                Console.WriteLine("2. List All Customers");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("Select an option: ");

                string? choice = Console.ReadLine()?.Trim();
                switch (choice)
                {
                    case "1":
                        AddCustomer();
                        break;
                    case "2":
                        ListCustomers();
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                if (!back)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void AddCustomer()
        {
            string name = InputValidator.ReadNonEmptyString("Enter Customer Name: ");
            string email = InputValidator.ReadNonEmptyString("Enter Email: ");
            string phone = InputValidator.ReadNonEmptyString("Enter Phone Number (digits only): ");

            if (!phone.All(char.IsDigit))
            {
                Console.WriteLine("Invalid phone number! Must contain digits only.");
                return;
            }

            customerService.AddCustomer(name, email, phone);
            ConsoleHelper.WriteSuccess($"Customer '{name}' added successfully.");
        }

        static void ListCustomers()
        {
            var customerList = customerService.GetAllCustomers();

            if (!customerList.Any())
            {
                Console.WriteLine("No customers found.");
                return;
            }

            Console.WriteLine("\nID  | Name                 | Email                     | Phone");
            Console.WriteLine("------------------------------------------------------------------");
            foreach (var c in customerList)
            {
                Console.WriteLine($"{c.Id,-3} | {c.Name,-20} | {c.Email,-25} | {c.PhoneNumber}");
            }
        }

        static void ManageOrders()
        {
            bool back = false;
            while (!back)
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine("--- Order Management ---");
                    Console.WriteLine("1. Create New Order");
                    Console.WriteLine("2. List All Orders");
                    Console.WriteLine("0. Back to Main Menu");
                    Console.Write("Select an option: ");

                    string? choice = Console.ReadLine()?.Trim();
                    switch (choice)
                    {
                        case "1":
                            CreateOrder();
                            break;
                        case "2":
                            ListOrders();
                            break;
                        case "0":
                            back = true;
                            break;
                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }

                    if (!back)
                    {
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey();
                    }
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteError($"An unexpected error occurred: {ex.Message}");
                }
            }
        }

        static void CreateOrder()
        {
            int customerId = InputValidator.ReadNonNegativeInt("Enter Customer ID: ");

            var itemsToBuy = new List<(int productId, int quantity)>();
            bool addingItems = true;

            while (addingItems)
            {
                int productId = InputValidator.ReadNonNegativeInt("\nEnter Product ID to add (or 0 to finish): ");
                if (productId == 0) break;

                int qty = InputValidator.ReadNonNegativeInt("Enter Quantity: ");
                itemsToBuy.Add((productId, qty));

                if (!InputValidator.ReadConfirmation("Add another product? (y/n): "))
                {
                    addingItems = false;
                }
            }

            if (!itemsToBuy.Any())
            {
                Console.WriteLine("Order cancelled (no items added).");
                return;
            }

            Console.Write("\nEnter Promo Code (or press Enter to skip): ");
            string? promoCode = Console.ReadLine()?.Trim();

            orderService.CreateOrder(customerId, itemsToBuy, promoCode);
            ConsoleHelper.WriteSuccess("Order placed successfully!");
        }

        static void ListOrders()
        {
            var orderList = orderService.GetAllOrders();

            if (!orderList.Any())
            {
                Console.WriteLine("No orders placed yet.");
                return;
            }

            foreach (var order in orderList)
            {
                Console.WriteLine($"\n==========================================");
                Console.WriteLine($"Order #{order.Id} | Date: {order.OrderDate:g}");
                Console.WriteLine($"Customer: {order.Customer.Name} ({order.Customer.Email})");
                Console.WriteLine($"------------------------------------------");
                foreach (var item in order.Items)
                {
                    Console.WriteLine($"  - {item.Product.Name,-20} x{item.Quantity,-3} @ {item.UnitPrice,8:C} = {item.TotalPrice,9:C}");
                }
                Console.WriteLine($"------------------------------------------");
                Console.WriteLine($"Total Order Amount: {order.TotalAmount:C}");
                Console.WriteLine($"==========================================");
            }
        }

        static ProductCategory PromptCategory()
        {
            Console.WriteLine("\nSelect Product Category:");
            var categories = Enum.GetValues<ProductCategory>();

            foreach (var cat in categories)
            {
                Console.WriteLine($"{(int)cat}. {cat}");
            }

            while (true)
            {
                int choice = InputValidator.ReadNonNegativeInt("Enter Category Number: ");
                if (Enum.IsDefined(typeof(ProductCategory), choice))
                {
                    return (ProductCategory)choice;
                }
                Console.WriteLine("Invalid category selection. Please try again.");
            }
        }

        static void ListProductsGroupedByCategory()
        {
            var groups = productService.GetProductsGroupedByCategory();

            if (!groups.Any())
            {
                Console.WriteLine("No products found.");
                return;
            }

            foreach (var group in groups)
            {
                Console.WriteLine($"\n=== Category: {group.Key} ===");
                Console.WriteLine("ID  | Name                 | Price      | Qty | Total Value");
                Console.WriteLine("-----------------------------------------------------------");
                foreach (var p in group)
                {
                    Console.WriteLine($"{p.Id,-3} | {p.Name,-20} | {p.Price,10:C} | {p.Quantity,3} | {p.TotalValue,11:C}");
                }
            }
        }

        static void ManageReports()
        {
            bool back = false;
            while (!back)
            {
                Console.Clear();
                Console.WriteLine("--- Reports & Statistics ---");
                Console.WriteLine("1. Best Selling Products");
                Console.WriteLine("2. Top Customers by Spending");
                Console.WriteLine("3. Daily Sales Summary");
                Console.WriteLine("4. Low Stock Inventory Alert");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("Select an option: ");

                string? choice = Console.ReadLine()?.Trim();
                switch (choice)
                {
                    case "1":
                        GetTopSellingProducts();
                        break;
                    case "2":
                        GetTopCustomers();
                        break;
                    case "3":
                        GetDailySales();
                        break;
                    case "4":
                        GetLowStockProducts();
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                if (!back)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void GetTopSellingProducts()
        {
            var allOrders = orderService.GetAllOrders();

            if (!allOrders.Any())
            {
                Console.WriteLine("No orders recorded yet.");
                return;
            }

            var topProducts = allOrders.SelectMany(o => o.Items)
                .GroupBy(item => item.Product.Name)
                .Select(g => new
                {
                    ProductName = g.Key,
                    TotalQuantitySold = g.Sum(item => item.Quantity),
                    TotalRevenue = g.Sum(item => item.TotalPrice)
                })
                .OrderByDescending(p => p.TotalQuantitySold)
                .Take(5)
                .ToList();

            Console.WriteLine("\n--- Top 5 Best Selling Products ---");
            Console.WriteLine("Product Name         | Total Qty Sold | Total Revenue");
            Console.WriteLine("-----------------------------------------------------");
            foreach (var p in topProducts)
            {
                Console.WriteLine($"{p.ProductName,-20} | {p.TotalQuantitySold,-14} | {p.TotalRevenue,13:C}");
            }
        }

        static void GetTopCustomers()
        {
            var allOrders = orderService.GetAllOrders();

            if (!allOrders.Any())
            {
                Console.WriteLine("No orders recorded yet.");
                return;
            }

            var topCustomers = allOrders
                .GroupBy(o => o.Customer.Name)
                .Select(g => new
                {
                    CustomerName = g.Key,
                    OrderCount = g.Count(),
                    TotalSpent = g.Sum(o => o.TotalAmount)
                })
                .OrderByDescending(c => c.TotalSpent)
                .Take(5)
                .ToList();

            Console.WriteLine("\n--- Top Customers by Total Spending ---");
            Console.WriteLine("Customer Name        | Orders Count | Total Spent");
            Console.WriteLine("-------------------------------------------------");
            foreach (var c in topCustomers)
            {
                Console.WriteLine($"{c.CustomerName,-20} | {c.OrderCount,-12} | {c.TotalSpent,11:C}");
            }
        }

        static void GetDailySales()
        {
            var allOrders = orderService.GetAllOrders();

            if (!allOrders.Any())
            {
                Console.WriteLine("No orders recorded yet.");
                return;
            }

            var dailySales = allOrders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalAmount)
                })
                .OrderByDescending(d => d.Date)
                .ToList();

            Console.WriteLine("\n--- Daily Sales Summary ---");
            Console.WriteLine("Date       | Total Orders | Total Revenue");
            Console.WriteLine("-----------------------------------------");
            foreach (var d in dailySales)
            {
                Console.WriteLine($"{d.Date:yyyy-MM-dd} | {d.TotalOrders,-12} | {d.TotalRevenue,13:C}");
            }
        }

        static void GetLowStockProducts()
        {
            int threshold = 5;
            var allProducts = productService.GetAllProducts();
            var lowStock = allProducts.Where(p => p.Quantity < threshold).ToList();

            if (!lowStock.Any())
            {
                Console.WriteLine($"All products have sufficient stock (>= {threshold} items).");
                return;
            }

            Console.WriteLine($"\n--- Low Stock Alert (< {threshold} items) ---");
            Console.WriteLine("ID  | Product Name         | In Stock");
            Console.WriteLine("-------------------------------------");
            foreach (var p in lowStock)
            {
                Console.WriteLine($"{p.Id,-3} | {p.Name,-20} | {p.Quantity,8}");
            }
        }
    }
}