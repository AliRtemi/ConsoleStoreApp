using System;

namespace ConsoleStoreApp
{
    // Thrown when an entity (Product, Customer, Order) cannot be found by ID
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string message) : base(message) { }
    }

    // Thrown when trying to buy more items than available in inventory
    public class InsufficientStockException : Exception
    {
        public InsufficientStockException(string message) : base(message) { }
    }

    // Thrown when a duplicate email or phone number is detected
    public class DuplicateEntityException : Exception
    {
        public DuplicateEntityException(string message) : base(message) { }
    }
}