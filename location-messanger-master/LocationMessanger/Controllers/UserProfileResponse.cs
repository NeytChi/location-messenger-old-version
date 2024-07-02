using miniMessanger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocationMessanger.Controllers
{
    public class ProfileResponse
    {
        public string url_photo { get; set; }
        public int profile_age { get; set; }
        public bool profile_gender { get; set; }
        public string profile_city { get; set; }
        public double profile_latitude { get; set; }
        public double profile_longitude { get; set; }
        public int weight { get; set; }
        public int height { get; set; }
        public string status { get; set; }
        public ProfileResponse(Profile profile, string awsPath)
        {
            url_photo = profile.UrlPhoto == null ? "" : awsPath + profile.UrlPhoto;
            profile_age = profile.ProfileAge == null ? -1 : profile.ProfileAge.Value;
            profile_gender = profile.ProfileGender;
            profile_city = profile.ProfileCity ?? "";
            profile_latitude = profile.profileLatitude;
            profile_longitude = profile.profileLongitude;
            weight = profile.weight;
            height = profile.height;
            status = profile.status ?? "";
        }
    }
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
