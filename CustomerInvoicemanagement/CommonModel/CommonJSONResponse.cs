namespace CustomerInvoiceManagement.CommonModel
{
	public class CommonJSONResponse
	{
		public int? ResponseStatus { get; set; }
		public string? Message { get; set; }
		public dynamic? Result { get; set; }
	}
}
