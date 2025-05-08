using Mendix.StudioPro.ExtensionsAPI;
using Mendix.StudioPro.ExtensionsAPI.Model;
using Mendix.StudioPro.ExtensionsAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BYOLLM.Tools
{
    public class ProjectTools
    {
        public static IConfiguration GetConfiguration(IConfigurationService ConfigurationService)
        {
            return ConfigurationService.Configuration;
        }
    }
}
