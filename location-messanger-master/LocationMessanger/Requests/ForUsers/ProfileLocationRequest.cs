namespace LocationMessanger.Requests.ForUsers
{
    public class ProfileLocationRequest
    {
        public string UserToken { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
