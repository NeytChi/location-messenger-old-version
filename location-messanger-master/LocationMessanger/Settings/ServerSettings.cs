namespace LocationMessanger.Settings
{
    public class ServerSettings
    {
        public string IP = "127.0.0.1";
        public string Domen = "(none)";
        public int Port = 8023;
        public string AwsPath = "";
        public string savePath = "";
        public string urlCheck = "";
        public string urlRedirect = "";
        public string issuer = "MiniMessanger";
        public string audience = "http://localhost:8000/";
        public string auth_key = "mysql.connection";
        public int auth_lifetime = 1;
        public string mail_address = "test657483921@gmail.com";
        public string mail_password = "GmailPassword1234";
        public string smtp_server = "smtp.gmail.com";
        public int smtp_port = 587;
        public string url_check = "http://a.trackmyapptwlv.club/?app_id=26061&gaid=*GAID*&banner_id=*BANNER_ID*";
        public string url_redirect = "https://google.com";
        public bool email_enable = true;
    }
}
