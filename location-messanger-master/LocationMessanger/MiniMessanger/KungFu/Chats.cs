using System;
using Common;
using Serilog;
using System.Net;
using System.Linq;
using Serilog.Core;
using miniMessanger.Manage;
using miniMessanger.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Z.EntityFramework.Plus;
using LocationMessanger.Controllers;

namespace miniMessanger
{
    public class Chats
    {
        public Context context;
        public Users users;
        public Validator validator;
        public FileSaver system;
        public Logger log;
        public string savePath;
        public string awsPath;
        public Chats(Context context, Users users, Validator validator)
        {
            Config config = new Config();
            this.context = context;
            this.savePath = config.savePath;
            this.users = users;
            this.awsPath = config.AwsPath;
            this.validator = validator;
            this.system = new FileSaver();
            log = new LoggerConfiguration()
            .WriteTo.File("./logs/log", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        }
        public Chatroom CreateChat(string userToken, string publicToken, ref string message)
        {
            User user = users.GetUserByToken(userToken, ref message);
            if (user != null)
            {
                User interlocutor = users.GetUserByPublicToken(publicToken, ref message);
                if (interlocutor != null)
                {
                    return CreateChatIfNotExist(user, interlocutor);
                }
            } 
            return null;
        }
        public Chatroom CreateChatIfNotExist(User user, User interlocutor)
        {
            Chatroom room;
            Participants participant = context.Participants.Where(p 
            => p.UserId == user.UserId 
            && p.OpposideId == interlocutor.UserId).FirstOrDefault();
            if (participant == null)
            {
                room = SaveChat();
                SaveParticipants(room.ChatId,  user.UserId, interlocutor.UserId);
                SaveParticipants(room.ChatId, interlocutor.UserId, user.UserId);
                log.Information("Create new chat by user, id -> " + user.UserId);
            }
            else
            {
                room = context.Chatroom.Where(ch => ch.ChatId == participant.ChatId).First();
                log.Information("Get chat for user, id -> " + user.UserId);
            }
            return room;
        }
        public Chatroom SaveChat()
        {
            Chatroom room = new Chatroom();
            room.ChatToken = users.validator.GenerateHash(20);
            room.CreatedAt = DateTime.Now;
            context.Chatroom.Add(room);
            context.SaveChanges();
            log.Information("Save new chat");
            return room;
        }
        public void SaveParticipants(int chatId, int userId, int opposideUserId)
        {
            Participants participant = new Participants();
            participant.ChatId = chatId;
            participant.UserId = userId;
            participant.OpposideId = opposideUserId;
            context.Participants.Add(participant);
            context.SaveChanges();
            log.Information("Create and save new participants");
        }
        public Message CreateMessage(string messageText, string userToken, string chatToken , ref string answer)
        {
            if (CheckMessageText(ref messageText, ref answer))
            {
                User user = users.GetUserByToken(userToken, ref answer);
                if (user != null)
                {
                    Chatroom room = GetChatroom(chatToken, ref answer);
                    if (room != null)
                    {
                        Message message = SaveTextMessage(room.ChatId, user.UserId, messageText);
                        log.Information("Send new message, id -> " + message.MessageId);
                        return message;
                    }
                } 
            }
            return null;
        }
        public Message SaveTextMessage(int ChatId, int UserId, string messageText)
        {
            Message message = new Message();
            message.ChatId = ChatId;
            message.UserId = UserId;
            message.MessageType = "text";
            message.MessageText = messageText;
            message.MessageViewed = false;
            message.UrlFile = "";
            message.CreatedAt = DateTime.Now;
            context.Messages.Add(message);
            context.SaveChanges();
            log.Information("Save new text message");
            return message;
        }
        public Chatroom GetChatroom(string ChatToken, ref string message)
        {
            Chatroom room = context.Chatroom.Where(ch 
            => ch.ChatToken == ChatToken).FirstOrDefault();
            if (room == null)
            {
                message = "Server can't define chat by chat_token."; 
            } 
            return room;
        }
        public bool CheckMessageText(ref string messageText, ref string answer)
        {
            if (!string.IsNullOrEmpty(messageText))
            {
                if (messageText.Length < 500)
                {
                    messageText = WebUtility.UrlDecode(messageText);
                    return true;
                }
                messageText = "Message can't be more that 500 characters";
            }
            else
            {
                answer = "Message is empty. Server woundn't upload this message."; 
            }
            return false;
        }
        public Message UploadMessagePhoto(IFormFile photo, ChatCache cache, ref string message)
        {
            User user = users.GetUserByToken(cache.user_token, ref message);
            if (user != null)
            {
                Chatroom room = context.Chatroom.Where(ch 
                => ch.ChatToken == cache.chat_token).FirstOrDefault();
                if (room != null)
                {
                    return MessagePhoto(photo, user.UserId, room.ChatId, ref message);
                } 
                else 
                { 
                    message = "Server can't define chat by 'chat_token' key."; 
                }
            }
            return null;
        }
        public Message MessagePhoto(IFormFile photo, int userId, int chatId, ref string answer)
        {
            if (photo != null)
            {
                if (photo.ContentType.Contains("image"))
                {
                    Message message = new Message()
                    {
                        ChatId = chatId,
                        UserId = userId,
                        MessageType = "photo",
                        MessageText = "",
                        MessageViewed = false,
                        UrlFile = system.CreateFile(photo, "/MessagePhoto/"),
                        CreatedAt = DateTime.Now
                    };
                    context.Messages.Add(message);
                    context.SaveChanges();
                    log.Information("Create photo message, id -> " + message.MessageId);
                    return message;
                }
                answer = "Incorrect file type of message's foto.";
            }
            else
            {
                answer = "Input file is null.";
            } 
            return null;
        }
        public dynamic GetMessages(int UserId, string ChatToken, int Page, int Count, ref string answer)
        {
            Chatroom room = GetChatroom(ChatToken, ref answer);
            if (room != null)
            {
                var messages = context.Messages.Where(m 
                => m.ChatId == room.ChatId).OrderByDescending(m => m.MessageId)
                .Skip(Page * Count).Take(Count).Select(m 
                => new 
                { 
                    message_id = m.MessageId,
                    chat_id = m.ChatId,
                    user_id = m.UserId,
                    message_type = m.MessageType,
                    message_text = m.MessageText,
                    url_file = string.IsNullOrEmpty(m.UrlFile) ? "" : awsPath + m.UrlFile,
                    message_viewed = m.MessageViewed,
                    created_at = m.CreatedAt
                }).ToList(); 
                UpdateMessagesToViewed(room.ChatId, UserId);
                log.Information("Get messages by chat, id -> " + room.ChatId); 
                return messages;
            }
            return null;
        }
        public void UpdateMessagesToViewed(int ChatId, int UserId)
        {
            var data = context.Messages.Where(m => m.ChatId == ChatId && m.UserId != UserId).ToList();
            data.ForEach(m => m.MessageViewed = true);
            context.SaveChanges();
        }
        public dynamic GetChats(int UserId, int page = 0, int count = 30)
        {
            log.Information("Get chats by user, id -> " + UserId);
            
            var chats = context.Participants
                .IncludeOptimized(p => p.Chat)
                .IncludeOptimized(p => p.Opposite)
                .Where(p => p.UserId == UserId && !p.Opposite.Deleted)
                .Select(p => new ChatResponse(p.Opposite, p.Chat))
                .Skip(page * count).Take(count).ToList();
            foreach (var chat in chats)
            {
                chat.SetMessage(context.Messages.Where(m => m.ChatId == chat.chat.chat_id).OrderByDescending(m => m.MessageId).FirstOrDefault(), awsPath);
            }
            return chats;
        }
        public dynamic GetChatsByGender(int UserId, bool ProfileGender, int page = 0, int count = 30)
        {
            log.Information("Get chats by user with gender, id -> " + UserId);
            var chats = context.Participants
                .IncludeOptimized(p => p.Chat)
                .IncludeOptimized(p => p.Opposite)
                .IncludeOptimized(p => p.Opposite.Profile)
                .Where(p => p.UserId == UserId && p.Opposite.Profile.ProfileGender != ProfileGender && !p.Opposite.Deleted)
                .OrderByDescending(p => p.Opposite.UserId)
                .Select(p => new ChatWithLikesResponse(p.Opposite, p.Chat, awsPath)).Skip(page * count).Take(count).ToList();

            foreach (var chat in chats)
            {
                var like = context.LikeProfile.FirstOrDefault(l => l.UserId == UserId && l.ToUserId == chat.user.user_id);
                chat.SetMessage(context.Messages.FirstOrDefault(m => m.ChatId == chat.chat.chat_id), awsPath);
                chat.liked_user = like != null ? like.Like : false;
                chat.disliked_user = like != null ? like.Dislike : false;
            }

            return chats;
        }
    }
}