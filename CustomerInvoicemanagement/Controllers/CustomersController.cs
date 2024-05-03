using CustomerInvoiceManagement.CommonModel;
using DataAccessLayer.Models;
using DataAccessLayer.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CustomerInvoiceManagement.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CustomersController : ControllerBase
	{
		private readonly ICustomerInterface _customerService;
		public CustomersController(ICustomerInterface customerService)
		{
			_customerService = customerService;
		}

		[HttpGet]
		[Route("GetCustomers")]
		//[Authorize(Roles = "Admin,Employee")]
		public async Task<ActionResult> Get()
		{
			var objCommonJson = new CommonJSONResponse();
			try
			{
				var objInvoices = await _customerService.ListAsync();
				if (objInvoices is null || !objInvoices.Any())
				{
					objCommonJson.ResponseStatus = -1;
					objCommonJson.Message = "No records available in the database.";
				}
				else
				{
					objCommonJson.ResponseStatus = 1;
					objCommonJson.Message = "Records found successfully.";
				}
				objCommonJson.Result = objInvoices;
			}
			catch (Exception ex)
			{
				objCommonJson.ResponseStatus = 0;
				objCommonJson.Message = ex.Message;
				if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
				{
					objCommonJson.Message = ex.InnerException.Message;
				}
			}

			return Ok(objCommonJson);
		}

		[HttpGet]
		[Route("GetById")]
		//[Authorize(Roles = "Admin,Employee")]
		public async Task<CommonJSONResponse> GetById(string id)
		{
			var objCustomer = _customerService.GetById(id)
;
			if (objCustomer is null)
			{
				return new CommonJSONResponse
				{
					ResponseStatus = -1,
					Message = "No record found as per request search value.",
					Result = null
				};
			}
			else
			{
				return new CommonJSONResponse
				{
					ResponseStatus = 1,
					Message = "Record found successfully.",
					Result = objCustomer
				};
			}
		}


		[HttpPost]
		[Route("InsertCustomer")]
		//[Authorize(Roles = "Admin")]
		public async Task<ActionResult> InsertOne(Customer customer)
		{

			var objCommonJson = new CommonJSONResponse();
			try
			{
				await _customerService.CreateAsync(customer);
				objCommonJson.ResponseStatus = 1;
				objCommonJson.Message = "Record added successfully.";
				objCommonJson.Result = customer;
			}
			catch (Exception ex)
			{
				objCommonJson.ResponseStatus = 0;
				objCommonJson.Message = ex.Message;
				if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
				{
					objCommonJson.Message = ex.InnerException.Message;
				}
			}
			return Ok(objCommonJson);
		}

		[HttpPut("{id}")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Update(string id, Customer customer)
		{
			var objCustomer = _customerService.GetById(id)
;

			if (objCustomer is null)
			{
				return NotFound();
			}

			customer.CustomerId = objCustomer.CustomerId;

			_customerService.UpdateAsync(id, customer);

			return Ok();
		}
	}
}
