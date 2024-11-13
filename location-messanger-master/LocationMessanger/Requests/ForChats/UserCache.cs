namespace LocationMessanger.Requests.ForChats
{
    public class UserCache
    {
        public int Page { get; set; }
        public int Count { get; set; }
        public int WeightFrom { get; set; }
        public int HeightFrom { get; set; }
        public int WeightTo { get; set; }
        public int HeightTo { get; set; }
        public string Status { get; set; }
        public string UserToken { get;set; }
        public string OpposidePublicToken { get;set; }
    }
}
