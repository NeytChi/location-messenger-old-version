using Common;
using Serilog;
using System.Linq;
using Serilog.Core;
using miniMessanger.Models;
using System.Collections.Generic;
using System;
using Z.EntityFramework.Plus;
using LocationMessanger.Responses;
using Microsoft.Extensions.Options;
using LocationMessanger.Settings;

namespace miniMessanger.Manage
{
    public class Users
    {
        public Context context;
        public string awsPath;
        public Validator validator;
        public Logger log;
        public Users(Context context, Validator validator, IOptions<ServerSettings> settings)
        {
            this.context = context;
            this.awsPath = settings.Value.AwsPath;
            this.validator = validator;
            log = new LoggerConfiguration()
            .WriteTo.File("./logs/log", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        }
        public LikeProfiles LikeUser(UserCache cache, ref string message)
        {
            User user = GetUserByToken(cache.user_token, ref message);
            User opposideUser = GetUserByPublicToken(cache.opposide_public_token, ref message);
            if (user != null && opposideUser != null)
            {
                if (user.UserId != opposideUser.UserId)
                {
                    return CreateLike(user.UserId, opposideUser.UserId);                    
                }
                else
                {
                    message = "User can't like himself.";
                }
            }
            return null;
        }
        public LikeProfiles CreateLike(int UserId, int OpposideUserId)
        {
            LikeProfiles like = GetLikeProfiles(UserId, OpposideUserId);
            like.Like = like.Like != true; 
            if (like.Like && like.Dislike)
            {
                like.Dislike = false;
            }
            context.LikeProfile.Update(like);
            context.SaveChanges();
            return like;
        }
        public LikeProfiles DislikeUser(UserCache cache, ref string message)
        {
            User user = GetUserByToken(cache.user_token, ref message);
            User opposideUser = GetUserByPublicToken(cache.opposide_public_token, ref message);
            if (user != null && opposideUser != null)
            {
                if (user.UserId != opposideUser.UserId)
                {
                    return CreateDislike(user.UserId, opposideUser.UserId);
                }
                else
                {
                    message = "User can't like himself.";
                }
            }
            return null;
        }
        public LikeProfiles CreateDislike(int UserId, int OpposideUserId)
        {
            LikeProfiles like = GetLikeProfiles(UserId, OpposideUserId);
            like.Dislike = like.Dislike != true;
            if (like.Dislike && like.Like)
            {
                like.Like = false;
            }
            context.LikeProfile.Update(like);
            context.SaveChanges();
            return like;
        }
        public LikeProfiles GetLikeProfiles(int userId, int toUserId)
        {
            LikeProfiles like = context.LikeProfile.Where(l 
            => l.UserId == userId 
            && l.ToUserId == toUserId).FirstOrDefault();
            if (like == null)
            {
                like = new LikeProfiles()
                {
                    UserId = userId,
                    ToUserId = toUserId,
                    Like = false,
                    Dislike = false
                };
                context.LikeProfile.Add(like);
                context.SaveChanges();
            }
            return like;
        }
        public User GetUserByToken(string userToken, ref string message)
        {
            if (!string.IsNullOrEmpty(userToken))
            {
                User user = context.User.Where(u 
                => u.UserToken == userToken).FirstOrDefault();
                if (user == null)
                {
                    message = "Server can't define user by token.";
                }
                return user;
            }
            return null;
        }
        public User GetUserByPublicToken(string token, ref string message)
        {
            var user = context.User.Where(u => u.UserPublicToken == token && u.Activate == 1 && !u.Deleted).FirstOrDefault();
            if (user == null)
            {
                message = "Server can't define user by token.";
            }
            return user;
        }
        public User GetUserWithProfile(string userToken, ref string message)
        {
            if (!string.IsNullOrEmpty(userToken))
            {
                User user = context.User.Where(u => 
                u.UserToken == userToken).FirstOrDefault();
                if (user == null)
                {
                    message = "Server can't define user by token.";
                }
                else
                {
                    user.Profile = context.Profile.Where(p => p.UserId == user.UserId).FirstOrDefault();
                }
                return user;
            }
            return null;
        }
        public dynamic GetUsers(int userid, int page = 0, int count = 30)
        {
            log.Information("Get users by user, id -> " + userid);
            var users = context.User
                .Where(u => u.UserId != userid && u.Activate == 1 && !u.Deleted)
                .OrderBy(u => u.UserId)
                .Select(u => new UserResponse(u)).Skip(page * count).Take(count).ToList();

            return GetNonBlockedUsers(users, userid);
        }
        public List<UserResponse> GetNonBlockedUsers(List<UserResponse> users, int userId)
        {
            var usersOutput = new List<UserResponse>();
            var blockes = context.BlockedUsers.Where(u => u.UserId == userId && u.BlockedDeleted == 0).ToList();
            
            foreach (var user in users)
            {
                if (!blockes.Any(b => b.BlockedUserId == user.user_id))
                    usersOutput.Add(user);
            }
            return usersOutput;
        }
        public List<UserByLocationResponse> GetNonBlockedUsers(List<UserByLocationResponse> users, int userId)
        {
            var usersOutput = new List<UserByLocationResponse>();
            var blockes = context.BlockedUsers.Where(u => u.UserId == userId && u.BlockedDeleted == 0).ToList();

            foreach (var user in users)
            {
                if (!blockes.Any(b => b.BlockedUserId == user.user_id))
                    usersOutput.Add(user);
            }
            return usersOutput;
        }
        public List<UserByLocationResponse> GetUsersLikes(List<UserByLocationResponse> users, int userId)
        {
            var likes = context.LikeProfile.Where(l => l.UserId == userId);

            foreach (var user in users)
            {
                var like = likes.FirstOrDefault(l => l.ToUserId == user.user_id);
                if (like != null)
                {
                    user.disliked_user = like.Dislike;
                    user.liked_user = like.Like;
                }
            }
            return users;
        }
        public dynamic GetUsersByLocation(int userid, Profile userProfile, int page = 0, int count = 30)
        {
            var users =  context.User
                .IncludeOptimized(u => u.Profile)
                .Where(u => u.UserId != userid && u.Activate == 1 && !u.Deleted)
                .OrderBy(u => Math.Abs(u.Profile.profileLatitude - userProfile.profileLatitude))
                .OrderBy(u => Math.Abs(u.Profile.profileLongitude - userProfile.profileLongitude))
                .Select(user => new UserByLocationResponse(user, awsPath))
                .Skip(page * count).Take(count).ToList();

            users = GetNonBlockedUsers(users, userid);

            log.Information("Get users by location, id -> " + userid);
            return GetUsersLikes(users, userid);
        }
        public dynamic GetUsersByProfile(int userid, Profile userProfile, UserCache cache)
        {
            var users = context.User
                .IncludeOptimized(u => u.Profile)
                .Where(u => u.UserId != userid
                && (u.Profile.weight >= cache.weight_from && u.Profile.weight <= cache.weight_to)
                && (u.Profile.height >= cache.height_from && u.Profile.height <= cache.height_to)
                && u.Profile.status.Contains(cache.status)
                && u.Activate == 1
                && !u.Deleted)
                .OrderBy(u => Math.Abs(u.Profile.profileLatitude - userProfile.profileLatitude))
                .OrderBy(u => Math.Abs(u.Profile.profileLongitude - userProfile.profileLongitude))
                .Select(user => new UserByLocationResponse(user, awsPath))
                .Skip(cache.page * cache.count).Take(cache.count).ToList();

            users = GetNonBlockedUsers(users, userid);
            log.Information("Get users by location, id -> " + userid);
            return GetUsersLikes(users, userid);
        }
        /// <summary>
        /// Select list of users with profile data, like and dislike keys.
        /// </summary>
        public dynamic GetUsersByGender(int userid, bool ProfileGender, int page = 0, int count = 30)
        {
            var users = context.User
                .IncludeOptimized(u => u.Profile)
                .Where(u => u.UserId != userid && u.Activate == 1 && !u.Deleted)
            .OrderBy(u => u.UserId)
            .Select(user => new UserByLocationResponse(user, awsPath))
            .Skip(page * count).Take(count).ToList();
            
            users = GetNonBlockedUsers(users, userid);
            log.Information("Get users by user and gender, id -> " + userid);
            return GetUsersLikes(users, userid);
        }
        public List<UserByLocationResponse> GetLikes(int userId, bool profileGender, int page, int count) => 
            context.LikeProfile
            .IncludeOptimized(l => l.ToUser)
            .IncludeOptimized(l => l.ToUser.Profile)
            .Where(l => l.UserId == userId && l.Like && !l.ToUser.Deleted)
            .OrderBy(u => u.UserId)
            .Select(l => new UserByLocationResponse(l.ToUser, awsPath)
            {
                liked_user = true,
                disliked_user = false
            })
            .Skip(page * count).Take(count).ToList();
        
        public dynamic GetLikedUsers(int userId, bool profileGender, int page, int count)
        {
            var likes = GetLikes(userId, profileGender, page, count);

            likes = GetNonBlockedUsers(likes, userId);
            log.Information("Get liked users by user, id -> " + userId);
            
            return likes;
        }   
    }
}