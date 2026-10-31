using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleStoreApp
{
    public static class InputValidator
    {
        public static string ReadNonEmptyString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine()?.Trim();

                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input;
                }

                Console.WriteLine("Error: Input cannot be empty. Please try again.");
            }
        }

        public static decimal ReadNonNegativeDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine()?.Trim();

                if (decimal.TryParse(input, out decimal value) && value >= 0)
                {
                    return value;
                }

                Console.WriteLine("Error: Please enter a valid non-negative decimal amount.");
            }
        }

        public static int ReadNonNegativeInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine()?.Trim();

                if (int.TryParse(input, out int value) && value >= 0)
                {
                    return value;
                }

                Console.WriteLine("Error: Please enter a valid non-negative integer.");
            }
        }

        public static bool ReadConfirmation(string prompt)
        {
            Console.Write(prompt);
            string response = Console.ReadLine()?.Trim().ToLower() ?? "";
            return response == "y" || response == "yes";
        }
    }
}
