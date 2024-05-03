using CustomerInvoiceManagement.CommonModel;

namespace CustomerInvoiceManagement.Utility
{
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;

		public ExceptionMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext httpContext)
		{
			try
			{
				await _next(httpContext);
			}
			catch (Exception ex)
			{
				httpContext.Response.ContentType = "application/json";
				httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

				var response = new CommonJSONResponse
				{
					ResponseStatus = 0,
					Message = "An error occurred while processing your request.",
					Result = ex.Message // You can customize this part based on your needs
				};
				await httpContext.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(response));
			}
		}
	}
}
