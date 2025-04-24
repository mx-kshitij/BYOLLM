using Azure;
using BYOLLM.Models;
using Mendix.StudioPro.ExtensionsAPI.Model;
using Mendix.StudioPro.ExtensionsAPI.Model.DomainModels;
using Mendix.StudioPro.ExtensionsAPI.Model.Projects;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BYOLLM
{
    public class EntityTools
    {
        public string GetAttributes(IModel currentApp, string moduleName, string entityName)
        {
            var module = currentApp.Root.GetModules().FirstOrDefault(m => string.Equals(m.Name, moduleName, StringComparison.OrdinalIgnoreCase));
            if (module == null)
            {
                return $"A module with name {moduleName} was not found";
            }

            var entity = module.DomainModel.GetEntities().FirstOrDefault(e => string.Equals(e.Name, entityName, StringComparison.OrdinalIgnoreCase));
            if (entity == null)
            {
                return $"An entity with name {entityName} was not found in module {moduleName}";
            }

            var attributes = entity.GetAttributes()?.Select(attr => new AttributeModel(attr)).ToList();
            if (attributes == null || !attributes.Any())
            {
                return $"No attributes found in entity {entityName} of module {moduleName}";
            }

            var response = JsonSerializer.Serialize(attributes);
            return $"The attributes in entity {entityName} of module {moduleName} are: {response}";

        }

        public string CreateAttribute(IModel currentApp, string moduleName, string entityName, string attributeName, string attributeType)
        {
            var module = currentApp.Root.GetModules().FirstOrDefault(m => string.Equals(m.Name, moduleName, StringComparison.OrdinalIgnoreCase));
            if (module == null)
            {
                return $"A module with name {moduleName} was not found";
            }
            var entity = module.DomainModel.GetEntities().FirstOrDefault(e => string.Equals(e.Name, entityName, StringComparison.OrdinalIgnoreCase));
            if (entity == null)
            {
                return $"An entity with name {entityName} was not found in module {moduleName}";
            }
            using (var transaction = currentApp.StartTransaction("Create new Attribute"))
            {
                IAttribute newAttribute = currentApp.Create<IAttribute>();
                var attributeTypeInstance = GetAttributeType(currentApp, attributeType);
                if (attributeTypeInstance == null)
                {
                    return $"Invalid attribute type: {attributeType}";
                }
                newAttribute.Name = attributeName;
                newAttribute.Type = attributeTypeInstance;
                entity.AddAttribute(newAttribute);
                transaction.Commit();
            }

            return $"Attribute {attributeName} of type {attributeType} created successfully in entity {entityName} of module {moduleName}";
        }

        public string CreateAttributes(IModel currentApp, string moduleName, string entityName, List<AttributeModel> attributes)
        {
            var module = currentApp.Root.GetModules().FirstOrDefault(m => string.Equals(m.Name, moduleName, StringComparison.OrdinalIgnoreCase));
            if (module == null)
            {
                return $"A module with name {moduleName} was not found";
            }
            var entity = module.DomainModel.GetEntities().FirstOrDefault(e => string.Equals(e.Name, entityName, StringComparison.OrdinalIgnoreCase));
            if (entity == null)
            {
                return $"An entity with name {entityName} was not found in module {moduleName}";
            }
            using (var transaction = currentApp.StartTransaction("Create attributes"))
            {
                foreach(var attribute in attributes)
                {
                    IAttribute newAttribute = currentApp.Create<IAttribute>();
                    var attributeTypeInstance = GetAttributeType(currentApp, attribute.Type);
                    if (attributeTypeInstance == null)
                    {
                        return $"Invalid attribute type: {attribute.Type}";
                    }
                    newAttribute.Name = attribute.Name;
                    newAttribute.Type = attributeTypeInstance;
                    entity.AddAttribute(newAttribute);
                }
                transaction.Commit();
            }

            return $"{attributes.Count} attributes created successfully in entity {entityName} of module {moduleName}";
        }

        public IAttributeType? GetAttributeType(IModel currentApp, string attributeType)
        {
            return attributeType switch
            {
                "string" => currentApp.Create<IStringAttributeType>(),
                "integer" => currentApp.Create<IIntegerAttributeType>(),
                "long" => currentApp.Create<ILongAttributeType>(),
                "boolean" => currentApp.Create<IBooleanAttributeType>(),
                "decimal" => currentApp.Create<IDecimalAttributeType>(),
                "datetime" => currentApp.Create<IDateTimeAttributeType>(),
                "binary" => currentApp.Create<IBinaryAttributeType>(),
                "enumeration" => currentApp.Create<IEnumerationAttributeType>(),
                "autonumber" => currentApp.Create<IAutoNumberAttributeType>(),
                "hashedstring" => currentApp.Create<IHashedStringAttributeType>(),
                _ => null
            };
        }

    }
}
