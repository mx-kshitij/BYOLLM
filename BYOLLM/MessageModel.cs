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
        public MessageModel( string text)
        {
            Id = Guid.NewGuid().ToString();
            Text = text;
        }


        public string Id { get; }
        public string Text { get; set; }
    }
}
