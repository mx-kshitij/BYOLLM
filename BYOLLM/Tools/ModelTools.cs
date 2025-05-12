using Azure;
using BYOLLM.Models;
using Mendix.StudioPro.ExtensionsAPI.Model;
using Mendix.StudioPro.ExtensionsAPI.Model.DomainModels;
using Mendix.StudioPro.ExtensionsAPI.Model.Enumerations;
using Mendix.StudioPro.ExtensionsAPI.Model.Projects;
using Mendix.StudioPro.ExtensionsAPI.Model.Texts;
using Mendix.StudioPro.ExtensionsAPI.Services;
using Mendix.StudioPro.ExtensionsAPI.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BYOLLM
{
    public class ModelTools
    {
        public static string GetWeather(string city)
        {
            List<string> weatherConditions =
            [
                "sunny",
                "cloudy",
                "rainy",
                "stormy",
                "snowy"
            ];
            Random rnd = new();
            int r = rnd.Next(0, weatherConditions.Count);
            string weather = weatherConditions[r];
            return "The weather in " + city + " is " + weather;
        }

        public static string GetModules(IModel currentApp)
        {
            var modules = currentApp.Root.GetModules();
            string response = "";
            List<ModuleModel> moduleModels = new List<ModuleModel>();
            foreach (var item in modules)
            {
                ModuleModel module = new ModuleModel(item.Name, item.AppStoreVersion, item.FromAppStore);
                moduleModels.Add(module);
            }
            response = JsonSerializer.Serialize(moduleModels);
            return "Modules available : " + response;
        }

        public static string GetEntities(IModel currentApp, string moduleName)
        {
            var module = currentApp.Root.GetModules().FirstOrDefault(m => m.Name.Equals(moduleName, StringComparison.OrdinalIgnoreCase));
            string response = "";
            if (module != null)
            {
                var entities = module.DomainModel.GetEntities();
                if (entities == null || entities.Count == 0)
                {
                    return $"The module {module.Name} has no entities";
                }
                List<EntityModel> entitiesModel = new List<EntityModel>();
                foreach (var item in entities)
                {
                    EntityModel entity = new EntityModel(item.Name, item.Location, item.Documentation);
                    entitiesModel.Add(entity);
                }
                response = JsonSerializer.Serialize(entitiesModel);
                return $"The module {module.Name} has entities : {response}";
            }
            return $"A module with name {module.Name} was not found";
        }

        public static string CreateEntity(IModel currentApp, string moduleName, EntityModel entity)
        {
            var module = currentApp.Root.GetModules().FirstOrDefault(m => string.Equals(m.Name, moduleName, StringComparison.OrdinalIgnoreCase));
            if (module == null)
            {
                return $"A module with name {moduleName} was not found";
            }
            using (var transaction = currentApp.StartTransaction("Create new Entity"))
            {
                IEntity newEntity = currentApp.Create<IEntity>();
                newEntity.Name = entity.Name;
                newEntity.Location = entity.Location;
                newEntity.Documentation = entity.Documentation;
                INoGeneralization generalization = currentApp.Create<INoGeneralization>();
                generalization.Persistable = entity.IsPersistent;
                generalization.HasCreatedDate = entity.HasCreatedDate;
                generalization.HasChangedDate = entity.HasChangedDate;
                generalization.HasOwner = entity.HasOwner;
                generalization.HasChangedBy = entity.HasChangedBy;
                newEntity.Generalization = generalization;
                module.DomainModel.AddEntity(newEntity);
                transaction.Commit();
            }
            return $"An entity with name {entity.Name} was created in module {moduleName}";
        }

        public static string MoveEntity(IModel currentApp, string moduleName, string entityName, int newLocationX, int newLocationY)
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
            using (var transaction = currentApp.StartTransaction("Move Entity"))
            {
                entity.Location = new Location(newLocationX, newLocationY);
                transaction.Commit();
            }
            return $"The entity {entityName} was moved to location ({newLocationX}, {newLocationY})";
        }

        public static string RemoveEntity(IModel currentApp, string moduleName, string entityName)
        {
            var module = currentApp.Root.GetModules().FirstOrDefault(m => string.Equals(m.Name, moduleName, StringComparison.OrdinalIgnoreCase));
            if (module == null)
            {
                return $"A module with name {moduleName} was not found";
            }
            using (var transaction = currentApp.StartTransaction("Create new Entity"))
            {
                var entity = module.DomainModel.GetEntities().FirstOrDefault(e => string.Equals(e.Name, entityName, StringComparison.OrdinalIgnoreCase));
                if (entity == null)
                {
                    return $"An entity with name {entityName} was not found in module {moduleName}";
                }
                module.DomainModel.RemoveEntity(entity);
                transaction.Commit();
            }
            return $"An entity with name {entityName} was removed from module {moduleName}";
        }

        public static string GeneralizeEntity(IModel currentApp, string parentModuleName, string parentEntityName, string targetModuleName, string targetEntityName)
        {
            var parentModule = currentApp.Root.GetModules().FirstOrDefault(m => string.Equals(m.Name, parentModuleName, StringComparison.OrdinalIgnoreCase));
            if (parentModule == null)
            {
                return $"A module with name {parentModuleName} was not found";
            }
            var targetModule = currentApp.Root.GetModules().FirstOrDefault(m => string.Equals(m.Name, targetModuleName, StringComparison.OrdinalIgnoreCase));
            if (targetModule == null)
            {
                return $"A module with name {targetModuleName} was not found";
            }
            using (var transaction = currentApp.StartTransaction("Create new Entity"))
            {
                var parentEntity = parentModule.DomainModel.GetEntities().FirstOrDefault(e => string.Equals(e.Name, parentEntityName, StringComparison.OrdinalIgnoreCase));
                if (parentEntity == null)
                {
                    return $"An entity with name {parentEntityName} was not found in module {parentModuleName}";
                }
                var targetEntity = targetModule.DomainModel.GetEntities().FirstOrDefault(e => string.Equals(e.Name, targetEntityName, StringComparison.OrdinalIgnoreCase));
                if (targetEntity == null)
                {
                    return $"An entity with name {targetEntityName} was not found in module {targetModuleName}";
                }
                IGeneralization generalization = currentApp.Create<IGeneralization>();
                generalization.Generalization = parentEntity.QualifiedName;
                targetEntity.Generalization = generalization;
                transaction.Commit();
            }
            return $"Entity {targetEntityName} now has a generalization of {parentEntityName}";
        }

        public static string RemoveEntityGeneralization(IModel currentApp, string moduleName, string entityName)
        {
            var module = currentApp.Root.GetModules().FirstOrDefault(m => string.Equals(m.Name, moduleName, StringComparison.OrdinalIgnoreCase));
            if (module == null)
            {
                return $"A module with name {moduleName} was not found";
            }
            using (var transaction = currentApp.StartTransaction("Remove Entity Generalization"))
            {
                var entity = module.DomainModel.GetEntities().FirstOrDefault(e => string.Equals(e.Name, entityName, StringComparison.OrdinalIgnoreCase));
                if (entity == null)
                {
                    return $"An entity with name {entityName} was not found in module {moduleName}";
                }
                INoGeneralization generalization = currentApp.Create<INoGeneralization>();
                entity.Generalization = generalization;

                transaction.Commit();
            }
            return $"The generalization of entity {entityName} was removed in module {moduleName}";
        }

        public static string CreateEnumeration(IModel currentApp, string moduleName, string enumerationName, List<EnumerationItemModel> enumItems, string language = "en_US")
        {
            var module = currentApp.Root.GetModules().FirstOrDefault(m => string.Equals(m.Name, moduleName, StringComparison.OrdinalIgnoreCase));
            if (module == null)
            {
                return $"A module with name {moduleName} was not found";
            }
            using (var transaction = currentApp.StartTransaction("Create new Enumeration"))
            {
                IEnumeration enumeration = currentApp.Create<IEnumeration>();
                enumeration.Name = enumerationName;
                if (enumItems.Count > 0)
                {
                    foreach (EnumerationItemModel enumItem in enumItems)
                    {
                        IEnumerationValue enumerationValue = currentApp.Create<IEnumerationValue>();
                        IText text = currentApp.Create<IText>();
                        text.AddOrUpdateTranslation(language, enumItem.Value);
                        enumerationValue.Caption = text;
                        enumerationValue.Name = enumItem.Name;
                        enumeration.AddValue(enumerationValue);
                    }
                    module.AddDocument(enumeration);
                    transaction.Commit();
                }
                return $"An enumeration with name {enumerationName} was created in module {moduleName}";
            }
        }

        public static string AddEnumerationItems(IModel currentApp, string moduleName, string enumerationName, List<EnumerationItemModel> enumItems, string language = "en_US")
        {
            var module = currentApp.Root.GetModules().FirstOrDefault(m => string.Equals(m.Name, moduleName, StringComparison.OrdinalIgnoreCase));
            if (module == null)
            {
                return $"A module with name {moduleName} was not found";
            }
            IEnumeration? enumeration = module.GetDocuments().FirstOrDefault(e => e.Name == enumerationName) as IEnumeration;
            if (enumeration == null)
            {
                return $"An enumeration with name {enumerationName} was not found in module {moduleName}";
            }
            using (var transaction = currentApp.StartTransaction("Create new enumeration items"))
            {
                if (enumItems.Count > 0)
                {
                    foreach (EnumerationItemModel enumItem in enumItems)
                    {
                        IEnumerationValue enumerationValue = currentApp.Create<IEnumerationValue>();
                        IText text = currentApp.Create<IText>();
                        text.AddOrUpdateTranslation(language, enumItem.Value);
                        enumerationValue.Caption = text;
                        enumerationValue.Name = enumItem.Name;
                        enumeration.AddValue(enumerationValue);
                    }
                    transaction.Commit();
                }
                return $"New values were added to enumeration {enumerationName} in module {moduleName}";
            }
        }

        public static string RemoveEnumerationItems(IModel currentApp, string moduleName, string enumerationName, List<EnumerationItemModel> enumItems)
        {
            var module = currentApp.Root.GetModules().FirstOrDefault(m => string.Equals(m.Name, moduleName, StringComparison.OrdinalIgnoreCase));
            if (module == null)
            {
                return $"A module with name {moduleName} was not found";
            }
            IEnumeration? enumeration = module.GetDocuments().FirstOrDefault(e => e.Name == enumerationName) as IEnumeration;
            if (enumeration == null)
            {
                return $"An enumeration with name {enumerationName} was not found in module {moduleName}";
            }
            using (var transaction = currentApp.StartTransaction("Remove enumeration items"))
            {
                if (enumItems.Count > 0)
                {
                    IReadOnlyList<IEnumerationValue>? enumerationValueList = enumeration.GetValues();
                    if(enumerationValueList == null || enumerationValueList.Count == 0)
                    {
                        return $"The enumeration {enumerationName} has no values";
                    }
                    foreach (EnumerationItemModel enumItem in enumItems)
                    {
                        IEnumerationValue? enumerationValue = enumerationValueList.FirstOrDefault(e => e.Name == enumItem.Name);
                        if(enumerationValue == null)
                        {
                            return $"A value with name {enumItem.Name} was not found in {enumerationName} in module {moduleName}";
                        }
                        enumeration.RemoveValue(enumerationValue);
                    }
                    transaction.Commit();
                }
                return $"Values were removed from enumeration {enumerationName} in module {moduleName}";
            }
        }

        public static string RemoveEnumeration(IModel currentApp, string moduleName, string enumerationName)
        {
            var module = currentApp.Root.GetModules().FirstOrDefault(m => string.Equals(m.Name, moduleName, StringComparison.OrdinalIgnoreCase));
            if (module == null)
            {
                return $"A module with name {moduleName} was not found";
            }
            IEnumeration? enumeration = module.GetDocuments().FirstOrDefault(e => e.Name == enumerationName) as IEnumeration;
            if (enumeration == null)
            {
                return $"An enumeration with name {enumerationName} was not found in module {moduleName}";
            }
            using (var transaction = currentApp.StartTransaction("Remove enumeration items"))
            {
                module.RemoveDocument(enumeration);
                transaction.Commit();
                
                return $"Enumeration {enumerationName} was deleted in module {moduleName}";
            }
        }

        public static string GetEnumerationValues(IModel currentApp, string moduleName, string enumerationName, string language = "en_US")
        {
            var module = currentApp.Root.GetModules().FirstOrDefault(m => string.Equals(m.Name, moduleName, StringComparison.OrdinalIgnoreCase));
            if (module == null)
            {
                return $"A module with name {moduleName} was not found";
            }
            IEnumeration? enumeration = module.GetDocuments().FirstOrDefault(e => e.Name == enumerationName) as IEnumeration;
            if (enumeration == null)
            {
                return $"An enumeration with name {enumerationName} was not found in module {moduleName}";
            }
            using (var transaction = currentApp.StartTransaction("Remove enumeration items"))
            {
                IReadOnlyList<IEnumerationValue>? enumerationValueList = enumeration.GetValues();
                if (enumerationValueList == null || enumerationValueList.Count == 0)
                {
                    return $"The enumeration {enumerationName} has no values";
                }
                List<EnumerationItemModel> enumValuesList = new List<EnumerationItemModel>();
                foreach (var item in enumerationValueList)
                {
                    string enumValue = item.Caption.GetTranslations()?.FirstOrDefault(i => i.LanguageCode == language)?.Text ?? "";
                    EnumerationItemModel enumValueItem = new EnumerationItemModel(enumValue, item.Name, null);
                    enumValuesList.Add(enumValueItem);
                }
                string response = JsonSerializer.Serialize(enumValuesList);
                return $"Enumeration {enumerationName} in module {moduleName} for language {language} has values : {response}";
            }
        }

        public static string UpdateEnumerationItems(IModel currentApp, string moduleName, string enumerationName, List<EnumerationItemModel> enumItems, string language = "en_US")
        {
            var module = currentApp.Root.GetModules().FirstOrDefault(m => string.Equals(m.Name, moduleName, StringComparison.OrdinalIgnoreCase));
            if (module == null)
            {
                return $"A module with name {moduleName} was not found";
            }
            IEnumeration? enumeration = module.GetDocuments().FirstOrDefault(e => e.Name == enumerationName) as IEnumeration;
            if (enumeration == null)
            {
                return $"An enumeration with name {enumerationName} was not found in module {moduleName}";
            }
            using (var transaction = currentApp.StartTransaction("Create new enumeration items"))
            {
                if (enumItems.Count > 0)
                {
                    foreach (EnumerationItemModel enumItem in enumItems)
                    {
                        IReadOnlyList<IEnumerationValue>? enumerationValueList = enumeration.GetValues();
                        if (enumerationValueList == null || enumerationValueList.Count == 0)
                        {
                            return $"The enumeration {enumerationName} has no values";
                        }
                        IEnumerationValue? enumerationValueItem = enumerationValueList.FirstOrDefault(e => e.Name == enumItem.OriginalName);
                        if (enumerationValueItem == null)
                        {
                            return $"An enumeration value with name {enumItem.Name} was not found in enumeration {enumerationName}";
                        }
                        enumerationValueItem.Caption.AddOrUpdateTranslation(language, enumItem.Value);
                        enumerationValueItem.Name = enumItem.Name;
                    }
                    transaction.Commit();
                }
                return $"New translations were added to enumeration {enumerationName} in module {moduleName}";
            }
        }
    }
}
