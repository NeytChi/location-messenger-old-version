using miniMessanger.Models;
using System;

namespace LocationMessanger.Reponses
{
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
}
