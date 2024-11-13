namespace LocationMessanger.Requests.ForChats
{
    public class GetBlockedUsersRequest
    {
        public string UserToken { get; set; }
        public int Page { get; set; }
        public int Count { get; set; }
    }
}
