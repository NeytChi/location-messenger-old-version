using miniMessanger.Models;

namespace LocationMessanger.Responses
{
    public class UserResponse
    {
        public int user_id { get; set; }
        public string user_email { get; set; }
        public string user_login { get; set; }
        public int created_at { get; set; }
        public int? last_login_at { get; set; }
        public string user_public_token { get; set; }

        public UserResponse(User user)
        {
            user_id = user.UserId;
            user_email = user.UserEmail;
            user_login = user.UserLogin;
            created_at = user.CreatedAt;
            last_login_at = user.LastLoginAt;
            user_public_token = user.UserPublicToken;
        }
    }
}
