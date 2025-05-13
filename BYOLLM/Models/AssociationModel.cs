using Mendix.StudioPro.ExtensionsAPI.Model.DomainModels;
using System.Text.Json.Serialization;

namespace Odin
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
