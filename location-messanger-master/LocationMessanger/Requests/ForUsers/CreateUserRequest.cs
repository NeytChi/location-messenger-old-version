namespace LocationMessanger.Requests.ForUsers
{
    public class CreateUserRequest
    {
        public string UserEmail { get; set; }
        public string UserLogin { get; set; }
        public string Password { get; set; }
    }
}
