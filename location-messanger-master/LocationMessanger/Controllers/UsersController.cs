using Common;
using Serilog;
using System.Linq;
using Serilog.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using miniMessanger;
using miniMessanger.Models;
using miniMessanger.Manage;
using LocationMessanger.Responses;
using Microsoft.Extensions.Options;
using LocationMessanger.Settings;


namespace LocationMessanger.Controllers
{
    [ApiController]
    [Route("v1.0/[controller]/[action]")]
    public class UsersController : ControllerBase
    {
        private readonly Context context;
        public Users users;
        public Chats chats;
        public Profiles profiles;
        public Authentication authentication;
        public Blocks blocks;
        public Validator Validator;

        public string AwsPath;
        
        public UsersController(Context context, IOptions<ServerSettings> settings)
        {
            this.AwsPath = settings.Value.AwsPath;
            this.context = context;
            this.Validator = new Validator();
            this.users = new Users(context, Validator, settings);
            this.chats = new Chats(context, users, Validator, settings);
            this.profiles = new Profiles(context, settings);
            this.blocks = new Blocks(users, context);
            this.authentication = new Authentication(context, Validator, settings);
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Registration(UserCache cache)
        {
            string message = string.Empty;
            User user = authentication.Registrate(cache, ref message);
            if (user != null)
            {
                return Ok( new DataMessangeResponse(true, message, new
                {
                    profile_token = user.ProfileToken
                }));
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult RegistrationEmail(UserCache cache)
        {
            string message = null;
            if (authentication.ConfirmEmail(cache.user_email, ref message))
            {
                return Ok(new 
                {   
                    success = true, 
                    message = "Send confirm email to user." 
                });
            }  
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Login(UserCache cache)
        {
            string message = null;
            User user = authentication.Login(cache.user_email, cache.user_password, ref message);
            if (user != null)
            {
                return Ok(new DataResponse(true, new UserProfileResponse(user, AwsPath)));
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult LogOut(UserCache cache)
        {
            string message = null;
            if (authentication.LogOut(cache.user_token, ref message))
            {
                return Ok(new 
                { 
                    success = true, 
                    message = "Log out is successfully." 
                });
            }
            return StatusCode(500, new DataResponse(false, message));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult RecoveryPassword(UserCache cache)
        {
            string message = null;
            if (authentication.RecoveryPassword(cache.user_email, ref message))
            {
                return Ok(new 
                { 
                    success = true, 
                    message = "Recovery password. Send message with code to email=" + cache.user_email + "." 
                });
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult CheckRecoveryCode(UserCache cache)
        {
            string message = null;
            string RecoveryToken = authentication.CheckRecoveryCode(cache.user_email, cache.recovery_code, ref message);
            if (!string.IsNullOrEmpty(RecoveryToken))
            {
                return Ok(new 
                { 
                    success = true, 
                    data = new 
                    { 
                        recovery_token = RecoveryToken 
                    }
                });
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult ChangePassword(UserCache cache)
        {
            string message = null;
            if (authentication.ChangePassword(
                cache.recovery_token, cache.user_password, 
                cache.user_confirm_password, ref message))
            {
                return Ok(new { success = true, message = "Change user password." });
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Activate([FromQuery] string hash)
        {
            string message = null;
            if (authentication.Activate(hash, ref message))
            {
                return Ok(new { success = true, message = "User account is successfully active." });
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Delete(UserCache cache)
        { 
            string message = null;
            if (authentication.Delete(cache.user_token, ref message))
            {
                return Ok(new { success = true, message = "Account was successfully deleted." });
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult UpdateProfile(IFormFile profile_photo)
        {
            string message = null;
            string userToken = Request.Form["user_token"];
            User user = users.GetUserByToken(userToken, ref message);
            if (user != null) {
                user.Profile = profiles.UpdateProfile(
                    user.UserId, 
                    ref message, 
                    profile_photo, 
                    Request.Form["profile_gender"],
                    Request.Form["profile_city"], 
                    Request.Form["profile_age"],
                    ConvertHelper.ConvertDouble(Request.Form["profile_latitude"]),
                    ConvertHelper.ConvertDouble(Request.Form["profile_longitude"]),
                    ConvertHelper.ConvertInt(Request.Form["height"]),
                    ConvertHelper.ConvertInt(Request.Form["weight"]),
                    Request.Form["status"]);
                if (user.Profile != null) 
                    return Ok(new DataResponse(true, new ProfileResponse(user.Profile, AwsPath)));
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult RegistrateProfile(IFormFile profile_photo)
        {
            string message = null;
            string profileToken = Request.Form["profile_token"];
            User user = context.User.Where(u => u.ProfileToken == profileToken.ToString()).FirstOrDefault();
            if (user != null) {
                user.Profile = profiles.UpdateProfile(user.UserId, ref message, 
                    profile_photo, 
                    Request.Form["profile_gender"],
                    Request.Form["profile_city"], 
                    Request.Form["profile_age"],
                    ConvertHelper.ConvertDouble(Request.Form["profile_latitude"]),
                    ConvertHelper.ConvertDouble(Request.Form["profile_longitude"]),
                    ConvertHelper.ConvertInt(Request.Form["height"]),
                    ConvertHelper.ConvertInt(Request.Form["weight"]),
                    Request.Form["status"]
                    );
                if (user.Profile != null)
                    return Ok(new DataMessangeResponse(true,
                        "User account was successfully registered. See your email to activate account by link.",
                        new ProfileResponse(user.Profile, AwsPath)));
            }
            else 
                message = "No user with that profile_token."; 
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Profile(UserCache cache)
        {
            string message = null;
            User user = users.GetUserByToken(cache.user_token, ref message);
            if (user != null) {
                user.Profile = authentication.CreateIfNotExistProfile(user.UserId);
                return Ok(new DataResponse(true, new ProfileResponse(user.Profile, AwsPath)));
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult ProfileLocation(UserCache cache)
        {
            string message = null;
            User user = users.GetUserByToken(cache.user_token, ref message);
            if (user != null) {
                Profile profile = authentication.CreateIfNotExistProfile(user.UserId);
                profiles.UpdateLocation(ref profile, cache.profile_latitude, cache.profile_longitude);
                return Ok(new DataResponse(true, new ProfileResponse(user.Profile, AwsPath)));
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult GetUsersList(UserCache cache)
        {
            string message = null;
            cache.count = cache.count == 0 ? 30 : cache.count;
            User user = users.GetUserByToken(cache.user_token, ref message);
            if (user != null)
            {
                return Ok(new 
                { 
                    success = true, 
                    data = users.GetUsers(user.UserId, cache.page, cache.count) 
                });
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult SelectChats(UserCache cache)
        {
            string message = null;
            cache.count = cache.count == 0 ? 30 : cache.count;
            User user = users.GetUserByToken(cache.user_token, ref message);
            if (user != null)
            {
                return Ok(new 
                { 
                    success = true,
                    data = chats.GetChats(user.UserId, cache.page, cache.count) 
                });
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult SelectMessages(ChatCache cache)
        {
            string message = null;
            cache.count = cache.count == 0 ? 50 : cache.count;
            User user = users.GetUserByToken(cache.user_token, ref message);
            if (user != null)
            {
                dynamic messages = chats.GetMessages(
                    user.UserId, cache.chat_token, cache.page, cache.count, ref message);
                if (messages != null)
                {
                    return Ok(new DataResponse(true, messages));
                }

            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult CreateChat(UserCache cache)
        {
            string message = null;
            Chatroom room = chats.CreateChat(cache.user_token, cache.opposide_public_token, ref message);
            if (room != null)
            {
                return Ok(new DataResponse(true, 
                    new 
                    {
                        chat_id = room.ChatId,
                        chat_token = room.ChatToken,
                        created_at = room.CreatedAt 
                    } 
                ));
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult SendMessage(ChatCache cache)
        {
            string answer = null;
            Message message = chats.CreateMessage(
                cache.message_text, cache.user_token, cache.chat_token, ref answer);
            if (message != null)
            {
                return Ok(new 
                { 
                    success = true, 
                    data = new ChatMessageResponse(message, AwsPath) 
                });
            }
            return StatusCode(500, new MessageResponse(false, answer));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult MessagePhoto(IFormFile photo)
        {
            string message = null;
            ChatCache cache  = new()
            {
                user_token = Request.Form["user_token"],
                chat_token = Request.Form["chat_token"]
            };
            Message result = chats.UploadMessagePhoto(photo, cache, ref message);
            if (result != null)
            {
                return Ok(new { success = true, data = new ChatMessageResponse(result, AwsPath) });
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult BlockUser(UserCache cache)
        {
            string message = null;
            if (blocks.BlockUser(cache.user_token, cache.opposide_public_token, cache.blocked_reason, ref message))
            {
                return Ok(new { success = true, message = "Block user - successed." });
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult GetBlockedUsers(UserCache cache)
        {
            string message = null;
            cache.count = cache.count == 0 ? 50 : cache.count;
            User user = users.GetUserByToken(cache.user_token, ref message);
            if (user != null)
            {
                var blockedUsers = blocks.GetBlockedUsers(user.UserId, cache.page, cache.count);
                return Ok(new 
                { 
                    success = true, 
                    data = blockedUsers 
                });
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult UnblockUser(UserCache cache)
        {
            string message = null;
            if (blocks.UnblockUser(cache.user_token, cache.opposide_public_token, ref message))
            {
                return Ok(new MessageResponse(true, "Unblock user - successed."));
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult ComplaintContent(UserCache cache)
        {
            string message = null;   
            if (blocks.Complaint(cache.user_token, cache.message_id, cache.complaint, ref message))
            {
                return Ok(new MessageResponse (true, "Complain content - successed."));
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult GetUsersByGender(UserCache cache)
        {
            string message = null;
            cache.count = cache.count == 0 ? 30 : cache.count;
            User user = users.GetUserWithProfile(cache.user_token, ref message);
            if (user != null)
            {
                var data = users.GetUsersByGender(user.UserId, user.Profile.ProfileGender, cache.page, cache.count);
                return Ok(new DataResponse(true, data));
            }
            return StatusCode(500, new DataResponse(false, message));
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult GetUsersByLocation(UserCache cache)
        {
            string message = null;
            cache.count = cache.count == 0 ? 30 : cache.count;
            User user = users.GetUserWithProfile(cache.user_token, ref message);
            if (user != null)
                return Ok(new { success = true, 
                    data = users.GetUsersByLocation(user.UserId, user.Profile, cache.page, cache.count) });
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult GetUsersByProfile(UserCache cache)
        {
            string message = null;
            cache.count = cache.count == 0 ? 30 : cache.count;
            User user = users.GetUserWithProfile(cache.user_token, ref message);
            if (user != null)
                return Ok(new { success = true, 
                    data = users.GetUsersByProfile(user.UserId, user.Profile, cache) });
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult SelectChatsByGender(UserCache cache)
        {
            string message = null;
            cache.count = cache.count == 0 ? 30 : cache.count;
            var user = users.GetUserWithProfile(cache.user_token, ref message);
            if (user != null)
            {
                var data = chats.GetChatsByGender(user.UserId, user.Profile.ProfileGender, cache.page, cache.count);
                return Ok(new { success = true, data });
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult ReciprocalUsers(UserCache cache)
        {
            string message = null;
            cache.count = cache.count == 0 ? 30 : cache.count;
            User user = users.GetUserWithProfile(cache.user_token, ref message);
            if (user != null)
            {
                dynamic data = users.GetLikedUsers(user.UserId, user.Profile.ProfileGender, cache.page, cache.count);
                return Ok(new { success = true, data });
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult LikeUsers(UserCache cache)
        {
            string message = null;
            LikeProfiles like = users.LikeUser(cache, ref message);
            if (like != null)
            {
                return Ok(new 
                { 
                    success = true,
                    data = new 
                    {
                        disliked_user = like.Dislike,
                        liked_user = like.Like
                    }
                });
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DislikeUsers(UserCache cache)
        {
            string message = null;
            LikeProfiles dislike = users.DislikeUser(cache, ref message);
            if (dislike != null)
            {
                return Ok(new 
                { 
                    success = true,
                    data = new 
                    {
                        disliked_user = dislike.Dislike,
                        liked_user = dislike.Like
                    }
                });
            } 
            return StatusCode(500, new MessageResponse(false, message));
        }
    }
}