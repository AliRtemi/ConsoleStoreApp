using System;

namespace ConsoleStoreApp
{
    internal class Program
    {
        private static List<Product> products = new List<Product>();
        private static int nextProductId = 1;
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
                        Console.WriteLine("Customer management feature coming soon...");
                        break;
                    case "3":
                        Console.WriteLine("Order management feature coming soon...");
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
            Console.Write("Enter Product Name: ");
            string name = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Enter Price: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price < 0)
            {
                Console.WriteLine("Invalid price!");
                return;
            }

            Console.Write("Enter Quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity < 0)
            {
                Console.WriteLine("Invalid quantity!");
                return;
            }

            Product product = new Product(nextProductId++, name, price, quantity);
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

            Console.WriteLine("\nID  | Name                 | Price      | Qty | Total Value");
            Console.WriteLine("-----------------------------------------------------------");
            foreach (var p in products)
            {
                Console.WriteLine($"{p.Id,-3} | {p.Name,-20} | {p.Price,10:C} | {p.Quantity,3} | {p.TotalValue,11:C}");
            }

            decimal totalInventoryValue = products.Sum(p => p.TotalValue);
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine($"Total Inventory Value: {totalInventoryValue:C}");
        }
    }
}