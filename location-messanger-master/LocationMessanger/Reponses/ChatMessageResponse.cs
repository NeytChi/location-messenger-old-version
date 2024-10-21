using miniMessanger.Models;
using System;

namespace LocationMessanger.Reponses
{
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
