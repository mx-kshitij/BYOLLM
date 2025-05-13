using System.Text.Json.Serialization;

namespace Odin
{
    public record MessageListModel
    {
        [JsonConstructor]
        public MessageListModel(List<MessageModel> messages)
        {
            Messages = messages;
        }

        public List<MessageModel> Messages { get; }
    }
}
