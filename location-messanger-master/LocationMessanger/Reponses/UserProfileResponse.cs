using miniMessanger.Models;

namespace LocationMessanger.Reponses
{

    public class UserProfileResponse
    {
        public int user_id { get; set; }
        public string user_token { get; set; }
        public string user_email { get; set; }
        public string user_login { get; set; }
        public int created_at { get; set; }
        public int? last_login_at { get; set; }
        public string user_public_token { get; set; }
        public ProfileResponse profile { get; set; }

        public UserProfileResponse(User user, string awsPath)
        {
            user_id = user.UserId;
            user_token = user.UserToken;
            user_email = user.UserEmail;
            user_login = user.UserLogin;
            created_at = user.CreatedAt;
            last_login_at = user.LastLoginAt;
            user_public_token = user.UserPublicToken;
            if (user.Profile != null)
            {
                profile = new ProfileResponse(user.Profile, awsPath);
            }
        }
    }
}
