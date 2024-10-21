using miniMessanger.Models;

namespace LocationMessanger.Reponses
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
}
