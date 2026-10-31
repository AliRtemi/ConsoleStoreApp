using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace ConsoleStoreApp
{
    public class CustomerServiceAdo : ICustomerService
    {
        private readonly string _connectionString;

        public CustomerServiceAdo(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddCustomer(string name, string email, string phone)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            // 1. Check for duplicate Email
            string checkEmailSql = "SELECT COUNT(1) FROM Customers WHERE Email = @Email";
            using var emailCmd = new SqlCommand(checkEmailSql, connection);
            emailCmd.Parameters.AddWithValue("@Email", email);
            int emailCount = (int)emailCmd.ExecuteScalar();

            if (emailCount > 0)
            {
                throw new DuplicateEntityException($"A customer with email '{email}' already exists.");
            }

            // 2. Check for duplicate Phone Number
            string checkPhoneSql = "SELECT COUNT(1) FROM Customers WHERE PhoneNumber = @Phone";
            using var phoneCmd = new SqlCommand(checkPhoneSql, connection);
            phoneCmd.Parameters.AddWithValue("@Phone", phone);
            int phoneCount = (int)phoneCmd.ExecuteScalar();

            if (phoneCount > 0)
            {
                throw new DuplicateEntityException($"A customer with phone number '{phone}' already exists.");
            }

            // 3. Insert new Customer
            string insertSql = @"INSERT INTO Customers (Name, Email, PhoneNumber) 
                                 VALUES (@Name, @Email, @Phone)";
            using var insertCmd = new SqlCommand(insertSql, connection);
            insertCmd.Parameters.AddWithValue("@Name", name);
            insertCmd.Parameters.AddWithValue("@Email", email);
            insertCmd.Parameters.AddWithValue("@Phone", phone);

            insertCmd.ExecuteNonQuery();
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            var customers = new List<Customer>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            string sql = "SELECT Id, Name, Email, PhoneNumber FROM Customers";
            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                customers.Add(new Customer(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3)
                ));
            }

            return customers;
        }

        public Customer GetCustomerById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            string sql = "SELECT Id, Name, Email, PhoneNumber FROM Customers WHERE Id = @Id";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Customer(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3)
                );
            }

            throw new EntityNotFoundException($"Customer with ID {id} was not found.");
        }
    }
}