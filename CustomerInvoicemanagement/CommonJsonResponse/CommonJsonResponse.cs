namespace APICRUD_mongodb.CommonjsonResponse
{
    public class CommonResponse
    {
        public int ResponseStatus {  get; set; }
        public string Message { get; set; }

        public dynamic Result { get; set; }

    }

}
