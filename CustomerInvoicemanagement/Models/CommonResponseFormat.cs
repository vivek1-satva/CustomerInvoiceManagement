namespace CustomerInvoiceManagement.CommonModels
{
    public class CommonResponseFormat
    {
        public int ResponseStatus { get; set; }
        public string Message { get; set; }

        public dynamic Result { get; set; }
    }
}