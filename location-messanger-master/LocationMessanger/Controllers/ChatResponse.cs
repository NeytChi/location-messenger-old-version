using miniMessanger.Models;
using System;

namespace LocationMessanger.Controllers
{
    public class ChatResponse
    {
        public UserResponse user { get; set; }
        public ChatUnitResponse chat { get; set; }
        public ChatMessageResponse last_message { get; set; }
        public ChatResponse(User user, Chatroom chatroom)
        {
            this.user = new UserResponse(user);
            chat = new ChatUnitResponse(chatroom);
        }
        public void SetMessage(Message message, string awsPath)
        {
            if (message != null)
                last_message = new ChatMessageResponse(message, awsPath);
        }
    }
    public class ChatWithLikesResponse
    {
        public UserProfileResponse user { get; set; }
        public ChatUnitResponse chat { get; set; }
        public ChatMessageResponse last_message { get; set; }
        public bool liked_user { get; set; }
        public bool disliked_user { get; set; }

        public ChatWithLikesResponse(User userData, Chatroom chatroom, string awsPath)
        {
            user = new UserProfileResponse(userData, awsPath);
            chat = new ChatUnitResponse(chatroom);
        }
        public void SetMessage(Message message, string awsPath)
        {
            if (message != null)
                last_message = new ChatMessageResponse(message, awsPath);
        }
    }
    public class ChatUnitResponse
    {
        public int chat_id { get; set; }
        public string chat_token { get; set; }
        public DateTime created_at { get; set; }

        public ChatUnitResponse(Chatroom chat)
        {
            chat_id = chat.ChatId;
            chat_token = chat.ChatToken;
            created_at = chat.CreatedAt;
        }
    }
    public class ChatMessageResponse
    {
        public long message_id { get; set; }
        public int chat_id { get; set; }
        public int user_id { get; set; }
        public string message_type { get; set; }
        public string message_text { get; set; }
        public string url_file { get; set; }
        public bool message_viewed { get; set; }
        public DateTime created_at { get; set; }

        public ChatMessageResponse(Message message, string awsPath)
        {
            message_id = message.MessageId;
            chat_id = message.ChatId;
            user_id = message.UserId;
            message_type = message.MessageType;
            message_text = message.MessageText;
            url_file = string.IsNullOrEmpty(message.UrlFile) ? "" : awsPath + message.UrlFile;
            message_viewed = message.MessageViewed;
            created_at = message.CreatedAt;
        }
    }
}
