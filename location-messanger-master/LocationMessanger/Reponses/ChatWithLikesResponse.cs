using miniMessanger.Models;

namespace LocationMessanger.Reponses
{
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
}
