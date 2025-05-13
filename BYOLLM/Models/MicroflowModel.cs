using System.Text.Json.Serialization;

namespace Odin
{
    public record MicroflowModel
    {
        [JsonConstructor]
        public MicroflowModel(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }
}
