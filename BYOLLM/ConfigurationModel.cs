using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BYOLLM
{
    public record ConfigurationModel
    {
        [JsonConstructor]
        public ConfigurationModel(string endpoint, string deployment, string version, bool useEntraId, string apikey, string systemPrompt)
        {
            Endpoint = endpoint;
            Deployment = deployment;
            Version = version;
            UseEntraId = useEntraId;
            Apikey = apikey;
            SystemPrompt = systemPrompt;
        }

        public string Endpoint { get; set; }
        public string Deployment { get; set; }
        public string Version { get; set; }
        public bool UseEntraId { get; set; }
        public string Apikey { get; set; }
        public string SystemPrompt { get; set; }
    }
}
