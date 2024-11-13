using Common;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using miniMessanger;
using miniMessanger.Models;
using miniMessanger.Manage;
using LocationMessanger.Responses;
using Microsoft.Extensions.Options;
using LocationMessanger.Settings;
using LocationMessanger.Requests.ForUsers;
using LocationMessanger.Requests.ForChats;


namespace LocationMessanger.Controllers
{
    [ApiController]
    [Route("/[controller]/[action]")]
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
            AwsPath = settings.Value.AwsPath;
            this.context = context;
            Validator = new Validator();
            users = new Users(context, Validator, settings);
            chats = new Chats(context, users, Validator, settings);
            profiles = new Profiles(context, settings);
            blocks = new Blocks(users, context);
            authentication = new Authentication(context, Validator, settings);
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Registration(CreateUserRequest request)
        {
            string message = string.Empty;
            User user = authentication.Registrate(request, ref message);
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
        public ActionResult RegistrationEmail(RegistrationEmailRequest request)
        {
            string message = null;
            if (authentication.ConfirmEmail(request.Email, ref message))
            {
                return Ok(new MessageResponse(true, "Send confirm email to user."));
            }  
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Login(LoginRequest request)
        {
            string message = null;
            User user = authentication.Login(request.UserEmail, request.Password, ref message);
            if (user != null)
            {
                return Ok(new DataResponse(true, new UserProfileResponse(user, AwsPath)));
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult LogOut(LogOutRequest request)
        {
            string message = null;
            if (authentication.LogOut(request.UserToken, ref message))
            {
                return Ok(new MessageResponse(true, "Log out is successfully."));
            }
            return StatusCode(500, new DataResponse(false, message));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult RecoveryPassword(RecoveryPasswordRequest request)
        {
            string message = null;
            if (authentication.RecoveryPassword(request.UserEmail, ref message))
            {
                return Ok(new MessageResponse(true, $"Send message with code to email address -> {request.UserEmail}"));
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult CheckRecoveryCode(CheckRecoveryCodeRequest request)
        {
            string message = null;
            string RecoveryToken = authentication.CheckRecoveryCode(request.UserEmail, request.RecoveryCode, ref message);
            if (!string.IsNullOrEmpty(RecoveryToken))
            {
                return Ok(new { success = true, data = new { recovery_token = RecoveryToken }});
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult ChangePassword(ChangePasswordRequest request)
        {
            string message = null;
            if (authentication.ChangePassword(request.RecoveryToken, request.UserPassword, request.UserConfirmPassword, ref message))
            {
                return Ok(new MessageResponse(true, "Change user password."));
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
                return Ok(new MessageResponse(true, "User account is successfully active."));
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Delete(DeleteRequest request)
        { 
            string message = null;
            if (authentication.Delete(request.UserToken, ref message))
            {
                return Ok(new MessageResponse(true, "Account was successfully deleted." ));
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
        public ActionResult Profile(ProfileRequest request)
        {
            string message = null;
            User user = users.GetUserByToken(request.UserToken, ref message);
            if (user != null) {
                user.Profile = authentication.CreateIfNotExistProfile(user.UserId);
                return Ok(new DataResponse(true, new ProfileResponse(user.Profile, AwsPath)));
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult ProfileLocation(ProfileLocationRequest request)
        {
            string message = null;
            User user = users.GetUserByToken(request.UserToken, ref message);
            if (user != null) {
                Profile profile = authentication.CreateIfNotExistProfile(user.UserId);
                profiles.UpdateLocation(ref profile, request.Latitude, request.Longitude);
                return Ok(new DataResponse(true, new ProfileResponse(user.Profile, AwsPath)));
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult GetUsersList(GetUsersRequest request)
        {
            string message = null;
            request.Count = request.Count == 0 ? 30 : request.Count;
            var user = users.GetUserByToken(request.UserToken, ref message);
            if (user != null)
            {
                return Ok(new 
                { 
                    success = true, 
                    data = users.GetUsers(user.UserId, request.Page, request.Count) 
                });
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult SelectChats(GetUsersRequest request)
        {
            string message = null;
            request.Count = request.Count == 0 ? 30 : request.Count;
            var user = users.GetUserByToken(request.UserToken, ref message);
            if (user != null)
            {
                return Ok(new 
                { 
                    success = true,
                    data = chats.GetChats(user.UserId, request.Page, request.Count) 
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
        public ActionResult CreateChat(CreateChatRequest request)
        {
            string message = null;
            var room = chats.CreateChat(request.UserToken, request.OpposidePublicToken, ref message);
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
        public ActionResult BlockUser(BlockUserRequest request)
        {
            string message = null;
            if (blocks.BlockUser(request.UserToken, request.OpposidePublicToken, request.BlockedReason, ref message))
            {
                return Ok(new { success = true, message = "Block user - successed." });
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult GetBlockedUsers(GetBlockedUsersRequest request)
        {
            string message = null;
            request.Count = request.Count == 0 ? 50 : request.Count;
            User user = users.GetUserByToken(request.UserToken, ref message);
            if (user != null)
            {
                var blockedUsers = blocks.GetBlockedUsers(user.UserId, request.Page, request.Count);
                return Ok(new { success = true, data = blockedUsers });
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult UnblockUser(BlockUserRequest request)
        {
            string message = null;
            if (blocks.UnblockUser(request.UserToken, request.OpposidePublicToken, ref message))
            {
                return Ok(new MessageResponse(true, "Unblock user - successed."));
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult ComplaintContent(ComplaintContentRequest cache)
        {
            string message = null;   
            if (blocks.Complaint(cache.UserToken, cache.MessageId, cache.Complaint, ref message))
            {
                return Ok(new MessageResponse (true, "Complain content - successed."));
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult GetUsersByGender(GetUsersRequest request)
        {
            string message = null;
            request.Count = request.Count == 0 ? 30 : request.Count;
            User user = users.GetUserWithProfile(request.UserToken, ref message);
            if (user != null)
            {
                var data = users.GetUsersByGender(user.UserId, user.Profile.ProfileGender, request.Page, request.Count);
                return Ok(new DataResponse(true, data));
            }
            return StatusCode(500, new DataResponse(false, message));
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult GetUsersByLocation(GetUsersRequest cache)
        {
            string message = null;
            cache.Count = cache.Count == 0 ? 30 : cache.Count;
            User user = users.GetUserWithProfile(cache.UserToken, ref message);
            if (user != null)
                return Ok(new { success = true, 
                    data = users.GetUsersByLocation(user.UserId, user.Profile, cache.Page, cache.Count) });
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult GetUsersByProfile(UserCache cache)
        {
            string message = null;
            cache.Count = cache.Count == 0 ? 30 : cache.Count;
            User user = users.GetUserWithProfile(cache.UserToken, ref message);
            if (user != null)
                return Ok(new { success = true, 
                    data = users.GetUsersByProfile(user.UserId, user.Profile, cache) });
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult SelectChatsByGender(GetUsersRequest cache)
        {
            string message = null;
            cache.Count = cache.Count == 0 ? 30 : cache.Count;
            var user = users.GetUserWithProfile(cache.UserToken, ref message);
            if (user != null)
            {
                var data = chats.GetChatsByGender(user.UserId, user.Profile.ProfileGender, cache.Page, cache.Count);
                return Ok(new { success = true, data });
            }
            return StatusCode(500, new MessageResponse(false, message));
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult ReciprocalUsers(GetUsersRequest cache)
        {
            string message = null;
            cache.Count = cache.Count == 0 ? 30 : cache.Count;
            User user = users.GetUserWithProfile(cache.UserToken, ref message);
            if (user != null)
            {
                dynamic data = users.GetLikedUsers(user.UserId, user.Profile.ProfileGender, cache.Page, cache.Count);
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