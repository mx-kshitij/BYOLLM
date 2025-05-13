using Mendix.StudioPro.ExtensionsAPI;
using Mendix.StudioPro.ExtensionsAPI.Services;

namespace Odin
{
    public class ProjectTools
    {
        public static IConfiguration GetConfiguration(IConfigurationService ConfigurationService)
        {
            return ConfigurationService.Configuration;
        }
    }
}
