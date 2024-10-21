namespace LocationMessanger.Responses
{
    public class DataResponse
    {
        public bool success { get; set; }
        public dynamic data { get; set; }
        public DataResponse(bool Success, dynamic Data)
        {
            success = Success;
            data = Data;
        }
    }
}
