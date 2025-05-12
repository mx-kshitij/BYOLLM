using Azure;
using BYOLLM.Models;
using Mendix.StudioPro.ExtensionsAPI.Model;
using Mendix.StudioPro.ExtensionsAPI.Model.DomainModels;
using Mendix.StudioPro.ExtensionsAPI.Model.Enumerations;
using Mendix.StudioPro.ExtensionsAPI.Model.Projects;
using Mendix.StudioPro.ExtensionsAPI.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
        private readonly IDomainModelService domainModelService;

        public EntityTools(IDomainModelService _domainModelService)
        {
            domainModelService = _domainModelService;
        }
        public static string GetAttributes(IModel currentApp, string moduleName, string entityName)
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

        public string GetAssociations(IModel currentApp, string moduleName, string entityName)
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
            var associations = domainModelService.GetAssociationsOfEntity(currentApp, entity, AssociationDirection.Parent);
            if (associations == null || !associations.Any())
            {
                return $"No associations found in entity {entityName} of module {moduleName}";
            }
            var response = JsonSerializer.Serialize(associations.Select(a => new AssociationModel(a.Association)));
            return $"The associations in entity {entityName} of module {moduleName} are: {response}";
        }

        public string CreateAttribute(IModel currentApp, string moduleName, string entityName, string attributeName, string attributeType, string? enumerationName)
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
                IAttributeType attributeTypeInstance;
                if (attributeType == "enumeration")
                {
                    attributeTypeInstance = GetEnumerationAttributeType(currentApp, module, enumerationName);
                }
                else
                {
                   attributeTypeInstance = GetAttributeType(currentApp, attributeType);
                }
                if (attributeTypeInstance == null)
                {
                    return $"Invalid attribute type: {attributeType} or doesn't exist";
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
                foreach (var attribute in attributes)
                {
                    IAttribute newAttribute = currentApp.Create<IAttribute>();
                    IAttributeType attributeTypeInstance;
                    if (attribute.Type == "enumeration")
                    {
                        attributeTypeInstance = GetEnumerationAttributeType(currentApp, module, attribute.EnumerationName);
                    }
                    else
                    {
                        attributeTypeInstance = GetAttributeType(currentApp, attribute.Type);
                    }
                    if (attributeTypeInstance == null)
                    {
                        return $"Invalid attribute type: {attribute.Type} or doesn't exist";
                    }
                    newAttribute.Name = attribute.Name;
                    newAttribute.Type = attributeTypeInstance;
                    entity.AddAttribute(newAttribute);
                }
                transaction.Commit();
            }

            return $"{attributes.Count} attributes created successfully in entity {entityName} of module {moduleName}";
        }

        public string CreateAssociation(IModel currentApp, string originModuleName, string originEntityName, string destinationModuleName, string destinationEntityName, string associationType)
        {
            var originModule = currentApp.Root.GetModules().FirstOrDefault(m => string.Equals(m.Name, originModuleName, StringComparison.OrdinalIgnoreCase));
            if (originModule == null)
            {
                return $"A module with name {originModuleName} was not found";
            }
            var originEntity = originModule.DomainModel.GetEntities().FirstOrDefault(e => string.Equals(e.Name, originEntityName, StringComparison.OrdinalIgnoreCase));
            if (originEntity == null)
            {
                return $"An entity with name {originEntityName} was not found in module {originModuleName}";
            }
            var destinationModule = currentApp.Root.GetModules().FirstOrDefault(m => string.Equals(m.Name, destinationModuleName, StringComparison.OrdinalIgnoreCase));
            if (destinationModule == null)
            {
                return $"A module with name {destinationModuleName} was not found";
            }
            var destinationEntity = destinationModule.DomainModel.GetEntities().FirstOrDefault(e => string.Equals(e.Name, destinationEntityName, StringComparison.OrdinalIgnoreCase));
            if (destinationEntity == null)
            {
                return $"An entity with name {destinationEntityName} was not found in module {destinationModuleName}";
            }
            using (var transaction = currentApp.StartTransaction("Create association"))
            {
                string associationName = GetValidAssociationName(currentApp, originModule, originEntity, destinationModule, destinationEntity);
                IAssociation newAssociation = originEntity.AddAssociation(destinationEntity);
                if (associationType == "1to1")
                {
                    newAssociation.Type = AssociationType.Reference;
                    newAssociation.Owner = AssociationOwner.Both;
                }
                else if (associationType == "Nto1")
                {
                    newAssociation.Type = AssociationType.Reference;
                    newAssociation.Owner = AssociationOwner.Default;
                }
                else if (associationType == "NtoN")
                {
                    newAssociation.Type = AssociationType.ReferenceSet;
                    newAssociation.Owner = AssociationOwner.Default;
                }
                else
                {
                    return $"Invalid association type: {associationType}";
                }
                newAssociation.Name = associationName;
                transaction.Commit();
            }
            return $"Association created successfully between entity {originEntityName} and {destinationEntityName}";
        }

        public static string RemoveAttribute(IModel currentApp, string moduleName, string entityName, string attributeName)
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
            using (var transaction = currentApp.StartTransaction("Remove attribute"))
            {
                var attribute = entity.GetAttributes().FirstOrDefault(a => string.Equals(a.Name, attributeName, StringComparison.OrdinalIgnoreCase));
                if (attribute == null)
                {
                    return $"An attribute with name {attributeName} was not found in entity {entityName} of module {moduleName}";
                }
                entity.RemoveAttribute(attribute);
                transaction.Commit();
            }
            return $"Attribute {attributeName} removed successfully from entity {entityName} of module {moduleName}";
        }

        public static string RemoveAttributes(IModel currentApp, string moduleName, string entityName, List<AttributeModel> attributes)
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
            using (var transaction = currentApp.StartTransaction("Remove attribute"))
            {
                foreach (var attributeItem in attributes)
                {
                    var attribute = entity.GetAttributes().FirstOrDefault(a => string.Equals(a.Name, attributeItem.Name, StringComparison.OrdinalIgnoreCase));
                    if (attribute == null)
                    {
                        return $"An attribute with name {attributeItem.Name} was not found in entity {entityName} of module {moduleName}";
                    }
                    entity.RemoveAttribute(attribute);
                }
                transaction.Commit();
            }
            return $"{attributes.Count} Attributes removed successfully from entity {entityName} of module {moduleName}";
        }

        public string RemoveAssociation(IModel currentApp, string moduleName, string entityName, string associationName)
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
            using (var transaction = currentApp.StartTransaction("Remove association"))
            {

                var association = domainModelService.GetAssociationsOfEntity(currentApp, entity, AssociationDirection.Parent).FirstOrDefault(a => string.Equals(a.Association.Name, associationName, StringComparison.OrdinalIgnoreCase));
                if (association == null)
                {
                    return $"An association with name {associationName} was not found in entity {entityName} of module {moduleName}";
                }
                entity.DeleteAssociation(association.Association);
                transaction.Commit();
            }
            return $"Association {associationName} removed successfully from entity {entityName} of module {moduleName}";
        }

        private static IAttributeType? GetAttributeType(IModel currentApp, string attributeType)
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
                //"enumeration" => currentApp.Create<IEnumerationAttributeType>(),
                "autonumber" => currentApp.Create<IAutoNumberAttributeType>(),
                "hashedstring" => currentApp.Create<IHashedStringAttributeType>(),
                _ => null
            };
        }

        private static IAttributeType? GetEnumerationAttributeType(IModel currentApp, IModule module,  string enumerationName)
        {
            var attrType = currentApp.Create<IEnumerationAttributeType>();
            IEnumeration? enumeration = module.GetDocuments().FirstOrDefault(e => e.Name == enumerationName) as IEnumeration;
            if(enumeration == null)
            {
                return null;
            }
            attrType.Enumeration = enumeration.QualifiedName;
            return attrType;
        }

        private string GetValidAssociationName(IModel currentApp, IModule originModule, IEntity originEntity, IModule destinationModule, IEntity destinationEntity)
        {
            string baseName = $"{originEntity.Name}_{destinationEntity.Name}";
            string associationName = baseName;
            int counter = 1;

            var associationsToCheck = domainModelService.GetAllAssociations(currentApp, [originModule, destinationModule]).ToList();
            while (associationsToCheck.Any(a => string.Equals(a.Association.Name, associationName, StringComparison.OrdinalIgnoreCase)))
            {
                associationName = $"{baseName}_{counter}";
                counter++;
            }

            return associationName;
        }

    }
}
