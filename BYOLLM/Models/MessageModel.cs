using System.Text.Json.Serialization;

namespace Odin
{
    public record MessageModel
    {
        [JsonConstructor]
        public MessageModel( string text, string attachment)
        {
            Id = Guid.NewGuid().ToString();
            Text = text;
            Attachment = attachment;
        }


        public string Id { get; }
        public string Text { get; set; }
        public string Attachment { get; set; }
    }
}
