using System;

namespace ConsoleStoreApp
{
    internal class Program
    {
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
                        Console.WriteLine("Product management feature coming soon...");
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
    }
}