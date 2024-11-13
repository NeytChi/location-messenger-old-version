namespace LocationMessanger.Requests.ForUsers
{
    public class ChangePasswordRequest
    {
        public string RecoveryToken { get; set; }
        public string UserPassword { get; set; }
        public string UserConfirmPassword { get; set; }
    }
}
