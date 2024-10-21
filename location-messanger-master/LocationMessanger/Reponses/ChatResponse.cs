using miniMessanger.Models;

namespace LocationMessanger.Reponses
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
}
