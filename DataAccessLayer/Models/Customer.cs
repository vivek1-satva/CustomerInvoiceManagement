using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
	public class Customer
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? CustomerId { get; set; }

		[Required(ErrorMessage = "Mobile number is required.")]
		[RegularExpression(@"^(?!0)\d{10}$", ErrorMessage = "Mobile number must be 10 digits and not start with 0.")]
		public required string Mobile { get; set; }

		[Required(ErrorMessage = "Customer name is required.")]
		[StringLength(10, MinimumLength = 3)]
		[RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "Please enter a valid name with alphabetic characters only.")]
		public required string CustomerName { get; set; }

		[Required(ErrorMessage = "Email is required.")]
		[RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z]{2,})+$", ErrorMessage = "Please enter a valid email.")]
		[StringLength(255, MinimumLength = 10)]
		public required string Email { get; set; }
	}
}
