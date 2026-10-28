using System;

namespace ConsoleStoreApp
{
    internal class Program
    {
        private static List<Product> products = new List<Product>();
        private static int nextProductId = 1;

        private static List<Customer> customers = new List<Customer>();
        private static int nextCustomerId = 1;

        private static List<Order> orders = new List<Order>();
        private static int nextOrderId = 1;
        static void Main(string[] args)
        {
            bool exitRequested = false;

            while (!exitRequested) 
            {
                Console.Clear();

                Console.WriteLine("----------------");
                Console.WriteLine("Console store application");
                Console.WriteLine("=================================");
                Console.WriteLine("1. Manage Products");
                Console.WriteLine("2. Manage Customers");
                Console.WriteLine("3. Manage Orders");
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

            Product product = new Product(nextProductId++, name, price, quantity, category);

            products.Add(product);
            Console.WriteLine($"Product '{name}' added successfully with ID {product.Id}.");
        }

        static void ListProducts()
        {
            if (!products.Any())
            {
                Console.WriteLine("No products found.");
                return;
            }

            Console.WriteLine("\nID  | Name                 | Category    | Price      | Qty | Total Value");
            Console.WriteLine("-------------------------------------------------------------------------");
            foreach (var p in products)
            {
                Console.WriteLine($"{p.Id,-3} | {p.Name,-20} | {p.Category,-11} | {p.Price,10:C} | {p.Quantity,3} | {p.TotalValue,11:C}");
            }

            decimal totalInventoryValue = products.Sum(p => p.TotalValue);
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine($"Total Inventory Value: {totalInventoryValue:C}");
        }

        static void SearchProducts()
        {
            string query = InputValidator.ReadNonEmptyString("Enter product ID or name to search: ");

            if (string.IsNullOrWhiteSpace(query))
            {
                Console.WriteLine("Search term cannot be empty.");
                return;
            }

            // Search by ID or partial Name (case-insensitive)
            var results = products.Where(p =>
                p.Id.ToString() == query ||
                p.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
            ).ToList();

            if (!results.Any())
            {
                Console.WriteLine($"No products found matching '{query}'.");
                return;
            }

            Console.WriteLine($"\nFound {results.Count} matching product(s):");
            Console.WriteLine("ID  | Name                 | Category    | Price      | Qty | Total Value");
            Console.WriteLine("-------------------------------------------------------------------------");
            foreach (var p in products)
            {
                Console.WriteLine($"{p.Id,-3} | {p.Name,-20} | {p.Category,-11} | {p.Price,10:C} | {p.Quantity,3} | {p.TotalValue,11:C}");
            }
        }

        static void UpdateProduct()
        {
            Console.Write("Enter Product ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid Product ID.");
                return;
            }

            Product? product = products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                Console.WriteLine($"Product with ID {id} not found.");
                return;
            }

            Console.WriteLine($"Updating '{product.Name}' (Press Enter to keep current value):");

            // Update Name
            product.Name = InputValidator.ReadNonEmptyString("Enter new Product Name: ");

            // Update Price
            product.Price = InputValidator.ReadNonNegativeDecimal("Enter New Price: ");

            // Update Quantity
            product.Quantity = InputValidator.ReadNonNegativeInt("Enter new Quantity: ");

            Console.WriteLine($"Product ID {id} updated successfully!");
        }

        static void DeleteProduct()
        {
            Console.Write("Enter Product ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid Product ID.");
                return;
            }

            Product? product = products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                Console.WriteLine($"Product with ID {id} not found.");
                return;
            }

            if (InputValidator.ReadConfirmation($"Are you sure you want to delete '{product.Name}'? (y/n): "))
            {
                products.Remove(product);
                Console.WriteLine($"Product '{product.Name}' deleted successfully.");
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
            if (!email.Contains("@"))
            {
                Console.WriteLine("Invalid email address.");
                return;
            }

            // Check for duplicate Email using LINQ Any
            if (customers.Any(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("A customer with this email address already exists!");
                return;
            }

            string phone = InputValidator.ReadNonEmptyString("Enter Phone Number (digits only): ");

            // Validate phone number using string.All(char.IsDigit)
            if (!phone.All(char.IsDigit))
            {
                Console.WriteLine("Invalid phone number! Must contain digits only.");
                return;
            }

            // Check for duplicate Phone Number
            if (customers.Any(c => c.PhoneNumber == phone))
            {
                Console.WriteLine("A customer with this phone number already exists!");
                return;
            }

            Customer customer = new Customer(nextCustomerId++, name, email, phone);
            customers.Add(customer);
            Console.WriteLine($"Customer '{name}' added successfully with ID {customer.Id}.");
        }

        static void ListCustomers()
        {
            if (!customers.Any())
            {
                Console.WriteLine("No customers found.");
                return;
            }

            Console.WriteLine("\nID  | Name                 | Email                     | Phone");
            Console.WriteLine("------------------------------------------------------------------");
            foreach (var c in customers)
            {
                Console.WriteLine($"{c.Id,-3} | {c.Name,-20} | {c.Email,-25} | {c.PhoneNumber}");
            }
        }

        static void ManageOrders()
        {
            bool back = false;
            while (!back)
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
        }

        static void CreateOrder()
        {
            if (!customers.Any())
            {
                Console.WriteLine("No customers found! Add a customer before creating an order.");
                return;
            }

            if (!products.Any(p => p.Quantity > 0))
            {
                Console.WriteLine("No products in stock!");
                return;
            }

            // Select Customer
            Console.Write("Enter Customer ID for the order: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                Console.WriteLine("Invalid Customer ID.");
                return;
            }

            Customer? customer = customers.FirstOrDefault(c => c.Id == customerId);
            if (customer == null)
            {
                Console.WriteLine($"Customer with ID {customerId} not found.");
                return;
            }

            Order newOrder = new Order(nextOrderId++, customer);
            bool addingItems = true;

            while (addingItems)
            {
                Console.Write("\nEnter Product ID to add to order (or 0 to finish): ");
                if (!int.TryParse(Console.ReadLine(), out int productId) || productId == 0)
                {
                    break;
                }

                Product? product = products.FirstOrDefault(p => p.Id == productId);
                if (product == null)
                {
                    Console.WriteLine("Product not found.");
                    continue;
                }

                Console.Write($"Enter Quantity for '{product.Name}' (In stock: {product.Quantity}): ");
                if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0)
                {
                    Console.WriteLine("Invalid quantity.");
                    continue;
                }

                // Business Rule: Check stock availability
                if (qty > product.Quantity)
                {
                    Console.WriteLine($"Insufficient stock! Only {product.Quantity} available.");
                    continue;
                }

                // Add item to order & reduce inventory stock
                newOrder.Items.Add(new OrderItem(product, qty));
                product.Quantity -= qty;

                Console.WriteLine($"Added {qty} x {product.Name} to order.");

                if (!InputValidator.ReadConfirmation("Add another item? (y/n): "))
                {
                    addingItems = false;
                }
            }

            if (newOrder.Items.Any())
            {
                orders.Add(newOrder);
                Console.WriteLine($"\nOrder #{newOrder.Id} created successfully for {customer.Name}!");
                Console.WriteLine($"Total Amount: {newOrder.TotalAmount:C}");
            }
            else
            {
                Console.WriteLine("Order cancelled (no items added).");
            }
        }

        static void ListOrders()
        {
            if (!orders.Any())
            {
                Console.WriteLine("No orders placed yet.");
                return;
            }

            foreach (var order in orders)
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
            if (!products.Any())
            {
                Console.WriteLine("No products found.");
                return;
            }

            // Grouping products by Category using LINQ GroupBy
            var groupedProducts = products.GroupBy(p => p.Category);

            foreach (var group in groupedProducts)
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
    }
}