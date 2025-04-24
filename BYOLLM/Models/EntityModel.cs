using Mendix.StudioPro.ExtensionsAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BYOLLM
{
    public record EntityModel
    {
        [JsonConstructor]
        public EntityModel( string name, Location location, string documentation)
        {
            Name = name;
            Location = location;
            Documentation = documentation;
        }

        public string Name { get; set; }
        public Location Location { get; set; }
        public string Documentation { get; set; }
    }
}
