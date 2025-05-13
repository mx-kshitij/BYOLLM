using System.Text.Json.Serialization;

namespace Odin
{
    public record EnumerationItemModel
    {
        [JsonConstructor]
        public EnumerationItemModel(string value, string name, string originalName)
        {
            Name = name;
            Value = value;
            OriginalName = originalName;
        }

        public string Name { get; set; }
        public string Value { get; set; }
        public string OriginalName { get; set; }
    }

}
