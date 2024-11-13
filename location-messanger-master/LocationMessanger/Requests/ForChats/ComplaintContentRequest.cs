namespace LocationMessanger.Requests.ForChats
{
    public class ComplaintContentRequest
    {
        public string UserToken { get; set; }
        public int MessageId { get;set; }
        public string Complaint { get; set; }
    }
}
