using System.Collections.Generic;

namespace ConsoleStoreApp
{
    public interface ICustomerService
    {
        void AddCustomer(string name, string email, string phone);
        IEnumerable<Customer> GetAllCustomers();
        Customer GetCustomerById(int id);
    }
}
