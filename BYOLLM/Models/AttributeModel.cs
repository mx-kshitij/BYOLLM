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
    public record AttributeModel
    {
        public AttributeModel(IAttribute attribute)
        {
            Name = attribute.Name;
            Type = GetAttributeTypeString(attribute);
            EnumerationName = attribute is IEnumerationAttributeType enumAttr ? enumAttr.Enumeration.Name : String.Empty;
        }

        [JsonConstructor]
        public AttributeModel(string name, string type, string? enumerationName)
        {
            Name = name;
            Type = type;
            EnumerationName = enumerationName ?? String.Empty;
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public string EnumerationName { get; set; }


        private static string GetAttributeTypeString(IAttribute attribute)
        {
            return attribute.Type switch
            {
                IStringAttributeType => "string",
                IIntegerAttributeType => "integer",
                ILongAttributeType => "long",
                IBooleanAttributeType => "boolean",
                IDecimalAttributeType => "decimal",
                IDateTimeAttributeType => "datetime",
                IBinaryAttributeType => "binary",
                IEnumerationAttributeType => "enumeration",
                IAutoNumberAttributeType => "autonumber",
                IHashedStringAttributeType => "hashedstring",
                _ => "Unknown"
            };
        }
    }

}
