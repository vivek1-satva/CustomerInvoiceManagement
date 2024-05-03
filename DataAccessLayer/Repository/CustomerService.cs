using DataAccessLayer.Models;
using DataAccessLayer.Services;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
	public class CustomerService : ICustomerInterface
	{
		public IMongoCollection<Customer> customerCollection;

		public CustomerService(string connectionString, string databaseName)
		{
			var mongoClient = new MongoClient(connectionString);
			var mongoDatabase = mongoClient.GetDatabase(databaseName);
			customerCollection = mongoDatabase.GetCollection<Customer>("Customers"); //collectionName
		}

		public async Task CreateAsync(Customer customer)
		{
			await customerCollection.InsertOneAsync(customer);
		}

		public async Task<List<Customer>> ListAsync()
		{
			return await customerCollection.Find(_ => true).ToListAsync();
		}

		public Customer? GetById(string id)
		{
			return customerCollection.Find(x => x.CustomerId == id).FirstOrDefault();
		}

		public async Task UpdateAsync(string id, Customer updatedCustomer)
		{
			await customerCollection.ReplaceOneAsync(x => x.CustomerId == id, updatedCustomer);
		}
	}
}
