using Mendix.StudioPro.ExtensionsAPI.Model;
using System.Text.Json.Serialization;

namespace Odin
{
    public record EntityModel
    {
        [JsonConstructor]
        public EntityModel(
            string name,
            Location location,
            string documentation = "",
            bool isPersistent = true,
            bool hasChangedDate = false,
            bool hasCreatedDate = false,
            bool hasOwner = false,
            bool hasChangedBy = false)
        {
            Name = name;
            Location = location;
            Documentation = documentation;
            IsPersistent = isPersistent;
            HasChangedDate = hasChangedDate;
            HasCreatedDate = hasCreatedDate;
            HasOwner = hasOwner;
            HasChangedBy = hasChangedBy;
        }

        public string Name { get; set; }
        public Location Location { get; set; }
        public string Documentation { get; set; }
        public bool IsPersistent { get; set; }
        public bool HasChangedDate { get; set; }
        public bool HasCreatedDate { get; set; }
        public bool HasOwner { get; set; }
        public bool HasChangedBy { get; set; }

    }
}
