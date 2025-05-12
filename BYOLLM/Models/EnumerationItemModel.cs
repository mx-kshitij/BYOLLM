using Mendix.StudioPro.ExtensionsAPI.Model;
using Mendix.StudioPro.ExtensionsAPI.Model.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BYOLLM
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
