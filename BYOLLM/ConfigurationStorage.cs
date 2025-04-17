using Mendix.StudioPro.ExtensionsAPI.Model;
using Mendix.StudioPro.ExtensionsAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BYOLLM
{
    public class ConfigurationStorage
    {
        private readonly ILogService _logService;
        private readonly string _configFilePath;

        public ConfigurationStorage(IModel currentApp, ILogService logService)
        {
            _logService = logService;
            _configFilePath = Path.Join(currentApp.Root.DirectoryPath, "byollm.json");
        }

        public ConfigurationModel LoadConfiguration()
        {
            ConfigurationModel? configuration = null;

            try
            {
                configuration = JsonSerializer.Deserialize<ConfigurationModel>(File.ReadAllText(_configFilePath, Encoding.UTF8));
            }
            catch (Exception exception)
            {
                _logService.Error($"Error while loading To Dos from {_configFilePath}", exception);
            }

            return configuration;
        }

        public void SaveConfiguration(ConfigurationModel configuration)
        {
            var jsonText = JsonSerializer.Serialize(configuration, new JsonSerializerOptions() { WriteIndented = true });
            File.WriteAllText(_configFilePath, jsonText, Encoding.UTF8);
        }
    }
}
