namespace LocationMessanger.Requests.ForUsers
{
    public class CheckRecoveryCodeRequest
    {
        public string UserEmail { get; set; }
        public int RecoveryCode { get; set; }
    }
}
