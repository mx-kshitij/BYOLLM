using System.Text.Json.Serialization;

namespace Odin
{
    public record ModuleModel
    {
        [JsonConstructor]
        public ModuleModel(string name, string appStoreVersion, bool isAppStoreModule)
        {
            Name = name;
            AppStoreVersion = appStoreVersion;
            IsAppStoreModule = isAppStoreModule;
        }


        public string Name { get; set; }
        public string AppStoreVersion { get; set; }
        public bool IsAppStoreModule { get; set; }
    }
}
