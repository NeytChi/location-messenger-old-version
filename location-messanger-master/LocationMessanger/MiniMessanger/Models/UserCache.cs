namespace miniMessanger.Models
{
    public struct UserCache
    {
        public string user_login { get; set; }
        public string user_email { get; set; }
        public string user_password { get; set; }
        public string user_confirm_password { get; set; }
        public string user_token { get; set; }
        public int recovery_code { get; set; }
        public string recovery_token { get; set; }
        public int page { get; set; }
        public int count { get; set; }
        public long message_id { get; set; }
        public string complaint { get; set; }
        public string opposide_public_token { get; set; }
        public string blocked_reason { get; set; }
        public double profile_latitude { get; set; }
        public double profile_longitude { get; set; }
        public int weight_from { get; set; }
        public int weight_to { get; set; }
        public int height_from { get; set; }
        public int height_to { get; set; }
        public string status { get; set; }
    }
}