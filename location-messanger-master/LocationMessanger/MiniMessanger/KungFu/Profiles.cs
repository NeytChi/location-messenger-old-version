using System;
using System.Web;
using Serilog;
using System.Linq;
using Serilog.Core;
using miniMessanger.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using LocationMessanger.Settings;

namespace miniMessanger
{
    public class Profiles
    {
        public Context context;
        public FileSaver fileSystem;
        public Logger log;
        public Profiles(Context context, IOptions<ServerSettings> settings)
        {
            this.context = context;
            fileSystem = new FileSaver(settings);
            log = new LoggerConfiguration()
                .WriteTo.File("./logs/log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
        public Profile UpdateProfile (
            int userId,
            ref string message, 
            IFormFile photo = null, 
            string profileGender = null,
            string profileCity = null,
            string profileAge = null,
            double? profileLatitude = null,
            double? profileLongitude = null,
            int height = 0,
            int weight = 0,
            string status = null)
        {
            Profile profile = CreateIfNotExistProfile(userId);
            if (UpdateGender(ref profile, profileGender, ref message)) 
                if (UpdateAge(ref profile, profileAge, ref message)) 
                    if (UpdateCity(ref profile, profileCity, ref message)) 
                        if (UpdatePhoto(photo,ref profile, ref message)) 
                            if (UpdateLocation(ref profile, profileLatitude, profileLongitude))
                                if (UpdateHeight(ref profile, height, ref message)) 
                                    if (UpdateWeight(ref profile, weight, ref message)) 
                                        if (UpdateStatus(ref profile, status, ref message)) {
                                            log.Information("Update profile, id -> " + userId);
                                            return profile;
                                        }
            return null;
        }
        public bool UpdateGender(ref Profile profile, string profileGender, ref string message)
        {
            if (profileGender != null) {
                if (profileGender == "1")
                    profile.ProfileGender = true;
                else if (profileGender == "0")
                    profile.ProfileGender = false;
                else {
                    message ="Incorrect value in variable profile gender, id -> " + profile.UserId;
                    log.Warning(message);
                    return false;
                }
                context.Profile.Update(profile);
                context.SaveChanges();
                log.Information("Update profile gender, id -> ", profile.ProfileId);
            }
            return true;
        }
        public bool UpdateAge(ref Profile profile, string profileAge, ref string message)
        {
            if (profileAge != null)
            {
                short ProfileAge = 0;
                if (Int16.TryParse(profileAge, out ProfileAge)) {
                    if (ProfileAge > 0 && ProfileAge < 200) {
                        profile.ProfileAge = (sbyte)ProfileAge;
                        context.Profile.Update(profile);
                        context.SaveChanges();
                        log.Information("Update profile age, id -> ", profile.UserId);
                        return true;
                    }
                    message = "Profile age can't be more that 200 and less that 0.";
                }
                else
                    message = "Server can't convert profile age to short type.";
                return false;
            }
            return true;
        }
        public bool UpdateCity(ref Profile profile, string profileCity, ref string message)
        {
            if (profileCity != null)
            {
                if (profileCity.Length > 3 && profileCity.Length < 50)
                {
                    profile.ProfileCity = profileCity;
                    context.Profile.Update(profile);
                    context.SaveChanges();
                    log.Information("Update profile city, id -> ", profile.UserId);
                    return true;
                }
                message = "Parameter 'profile_city' can't has more that 50 charaters and less that 3.";
                return false;
            }
            return true;
        }
        public bool UpdatePhoto(IFormFile photo, ref Profile profile, ref string message)
        {
            if (photo != null)
            {
                if (photo.ContentType.Contains("image"))
                {
                    fileSystem.DeleteFile(profile.UrlPhoto);
                    profile.UrlPhoto = fileSystem.CreateFile(photo, "/ProfilePhoto/");
                    context.Profile.Update(profile);
                    context.SaveChanges();
                    log.Information("Update profile photo, id -> " + profile.UserId);
                    return true;
                }
                else
                {
                    message = "Wrong type of file. Required type of file is image.";
                }
                return false;
            }
            return true;
        }
        public bool UpdateLocation(ref Profile profile, double? profileLatitude, double? profileLongitude)
        {
            if (profileLatitude != null && profileLongitude != null) {
                profile.profileLatitude = (double)profileLatitude;
                profile.profileLongitude = (double)profileLongitude;
                context.Profile.Update(profile);
                context.SaveChanges();
                log.Information("Update profile location, id -> " + profile.UserId);
                return true;
            }
            return true;
        }
        
        public Profile CreateIfNotExistProfile(int UserId)
        {
            Profile profile = context.Profile.Where(p => p.UserId == UserId).FirstOrDefault();
            if (profile == null) {
                profile = new Profile();
                profile.UserId = UserId;
                profile.ProfileGender = true;
                profile.weight = 60;
                profile.height = 160;
                context.Add(profile);
                context.SaveChanges();
            }
            return profile;
        }
        public bool UpdateHeight(ref Profile profile, int height, ref string message)
        {
            if (height != 0) {
                if (height >= 100 && height <= 300) {
                    profile.height = height;
                    context.Profile.Update(profile);
                    context.SaveChanges();
                    log.Information("Update height profile, id -> " + profile.ProfileId);
                    return true;
                }
                else
                    message = "Height can't be more 300 & less 100";
                return false;
            }
            return true;
        }

        public bool UpdateWeight(ref Profile profile, int weight, ref string message)
        {
            if (weight != 0) {
                if (weight >= 40 && weight <= 400) {
                    profile.weight = weight;
                    context.Profile.Update(profile);
                    context.SaveChanges();
                    log.Information("Update weight profile, id -> " + profile.ProfileId);
                    return true;
                }
                else
                    message = "Weight can't be more 400 & less 40";
                return false;
            }
            return true;
        }
        public bool UpdateStatus(ref Profile profile, string status, ref string message)
        {
            if (!string.IsNullOrEmpty(status)) {
                status = HttpUtility.UrlDecode(status);
                if (status.Length <= 300) {
                    profile.status = status;
                    context.Profile.Update(profile);
                    context.SaveChanges();
                    log.Information("Update status profile, id -> " + profile.ProfileId);
                    return true;
                }
                else {
                    message = "Status can't be more that 300 characters.";
                    return false;
                }
            }
            return true;
        }
    }
}