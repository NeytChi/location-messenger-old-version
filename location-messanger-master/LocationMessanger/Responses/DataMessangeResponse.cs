namespace LocationMessanger.Responses
{
    public class DataMessangeResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public dynamic data { get; set; }
        public DataMessangeResponse(bool Success, string Message, dynamic Data)
        {
            success = Success;
            message = Message;
            data = Data;
        }
    }
}
