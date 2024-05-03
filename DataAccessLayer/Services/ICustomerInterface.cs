using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Services
{
	public interface ICustomerInterface
	{
		Customer? GetById(string id);
		Task CreateAsync(Customer customers);
		Task<List<Customer>> ListAsync();
		Task UpdateAsync(string id, Customer updatedCustomer);
	}
}
