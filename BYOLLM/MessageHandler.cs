using System.Text.Json;

namespace Odin
{
    public class MessageHandler
    {
        public MessageModel HandleNewUserMessage(string messageJSON)
        {
            MessageModel message = JsonSerializer.Deserialize<MessageModel>(messageJSON)!;
            return message;
        }
    }

}
