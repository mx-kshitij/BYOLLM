using Mendix.StudioPro.ExtensionsAPI.Model;
using Mendix.StudioPro.ExtensionsAPI.Services;
using System.Text;
using System.Text.Json;

namespace Odin
{
    public class ConfigurationStorage
    {
        private readonly ILogService _logService;
        private readonly string _configFilePath;

        public ConfigurationStorage(IModel currentApp, ILogService logService)
        {
            _logService = logService;
            _configFilePath = Path.Join(currentApp.Root.DirectoryPath, Defaults.extensionPath);
        }

        public ConfigurationModel LoadConfiguration()
        {
            ConfigurationModel? configuration = null;
            string fullPath = Path.Join(_configFilePath, Defaults.configFileName);
            try
            {
                configuration = JsonSerializer.Deserialize<ConfigurationModel>(File.ReadAllText(fullPath, Encoding.UTF8));
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
            if (!Directory.Exists(_configFilePath))
            {
                Directory.CreateDirectory(_configFilePath);
            }
            File.WriteAllText(Path.Join(_configFilePath, Defaults.configFileName), jsonText, Encoding.UTF8);
        }
    }
}
