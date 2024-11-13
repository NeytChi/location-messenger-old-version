namespace LocationMessanger.Requests.ForChats
{
    public class BlockUserRequest
    {
        public string UserToken { get; set; }
        public string OpposidePublicToken { get; set; }
        public string BlockedReason { get; set; }
    }
}
