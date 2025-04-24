using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BYOLLM.Models
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
