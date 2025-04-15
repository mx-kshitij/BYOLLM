using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BYOLLM
{
    public record MessageModel
    {
        [JsonConstructor]
        public MessageModel(string id, string text)
        {
            Id = id;
            Text = text;
        }

        public MessageModel(string text, bool isDone)
            : this(Guid.NewGuid().ToString(), text)
        {
        }

        public string Id { get; set; }
        public string Text { get; set; }
    }
}
