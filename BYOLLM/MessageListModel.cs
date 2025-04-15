using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BYOLLM
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
