using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleStoreApp
{
    public class CustomerService : ICustomerService
    {
        private readonly List<Customer> _customers;
        private int _nextCustomerId = 1;

        public CustomerService(List<Customer> customers)
        {
            _customers = customers ?? new List<Customer>();
            if (_customers.Any())
            {
                _nextCustomerId = _customers.Max(c => c.Id) + 1;
            }
        }

        public void AddCustomer(string name, string email, string phone)
        {
            // Duplicate Email Check
            if (_customers.Any(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new DuplicateEntityException($"A customer with email '{email}' already exists.");
            }

            // Duplicate Phone Check
            if (_customers.Any(c => c.PhoneNumber == phone))
            {
                throw new DuplicateEntityException($"A customer with phone number '{phone}' already exists.");
            }

            var customer = new Customer(_nextCustomerId++, name, email, phone);
            _customers.Add(customer);
            DataManager.SaveData("customers.json", _customers);
        }

        public IEnumerable<Customer> GetAllCustomers() => _customers;

        public Customer GetCustomerById(int id)
        {
            return _customers.FirstOrDefault(c => c.Id == id)
                ?? throw new EntityNotFoundException($"Customer with ID {id} was not found.");
        }
    }
}