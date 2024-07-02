namespace miniMessanger.Models
{
    public struct ChatCache
    {
        public string chat_token { get; set; }
        public string message_text { get; set; }
        public string user_token { get; set; }
        public int page { get; set; }
        public int count { get; set; }
    }
}