using System.Text.Json.Serialization;

namespace Odin
{
    public record FolderPathModel
    {
        
        [JsonConstructor]
        public FolderPathModel(string name, FolderPathModel folder)
        {
            Name = name;
            Folder = folder;
        }

        public string Name { get; set; }
        public FolderPathModel Folder { get; set; }
    }
}
