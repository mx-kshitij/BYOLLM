using System.Text.Json.Serialization;

namespace Odin
{
    public record FolderModel
    {
        public FolderModel(string name)
        {
            Name = name;
            Folders = [];
        }

        [JsonConstructor]
        public FolderModel(string name, List<FolderModel> folders)
        {
            Name = name;
            Folders = folders;
        }

        public string Name { get; set; }
        public List<FolderModel> Folders { get; set; }
    }
}
