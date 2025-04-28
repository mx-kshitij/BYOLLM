using Azure;
using BYOLLM.Models;
using Mendix.StudioPro.ExtensionsAPI.Model;
using Mendix.StudioPro.ExtensionsAPI.Model.DomainModels;
using Mendix.StudioPro.ExtensionsAPI.Model.Projects;
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

        public static string CreateEntity(IModel currentApp, string moduleName, string entityName, int locationX = 0, int locationY = 0)
        {
            var module = currentApp.Root.GetModules().FirstOrDefault(m => string.Equals(m.Name, moduleName, StringComparison.OrdinalIgnoreCase));
            if (module == null)
            {
                return $"A module with name {moduleName} was not found";
            }
            using (var transaction = currentApp.StartTransaction("Create new Entity"))
            {
                IEntity newEntity = currentApp.Create<IEntity>();
                newEntity.Name = entityName;
                newEntity.Location = new Location(locationX, locationY);
                module.DomainModel.AddEntity(newEntity);
                transaction.Commit();
            }
            return $"An entity with name {entityName} was created in module {moduleName}";
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
    }
}
