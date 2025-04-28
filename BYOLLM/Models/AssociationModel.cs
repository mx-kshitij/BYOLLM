using Mendix.StudioPro.ExtensionsAPI.Model;
using Mendix.StudioPro.ExtensionsAPI.Model.DomainModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BYOLLM
{
    public record AssociationModel
    {        
        public AssociationModel(IAssociation association)
        {
            Name = association.Name;
            Type = association.Type.ToString();
            Origin = association.Owner.ToString();
        }

        [JsonConstructor]
        public AssociationModel(string name, string? type, string? origin)
        {
            Name = name;
            Type = type ?? string.Empty;
            Origin = origin ?? string.Empty;
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public string Origin { get; set; }
    }

}
