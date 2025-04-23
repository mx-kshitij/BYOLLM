using Azure;
using Mendix.StudioPro.ExtensionsAPI.Model;
using Mendix.StudioPro.ExtensionsAPI.Model.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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

        public string GetModules(IModel currentApp)
        {
            var modules = currentApp.Root.GetModules();
            string response = "";
            foreach (var item in modules)
            {
                if (response != "")
                {
                    response += "," + item.Name;
                }
                else
                {
                    response = item.Name;
                }
            }
            return "Modules available : " + response;
        }

        public string GetEntities(IModel currentApp, string moduleName)
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
                foreach (var item in entities)
                {
                    if (response != "")
                    {
                        response += "," + item.Name;
                    }
                    else
                    {
                        response = item.Name;
                    }
                }
                return $"The module {module.Name} has entities : {response}";
            }
            return $"A module with name {module.Name} was not found";
        }
    }
}
