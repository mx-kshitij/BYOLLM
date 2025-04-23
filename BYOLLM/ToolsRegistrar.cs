using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BYOLLM
{
    public class ToolsRegistrar
    {
        public ChatCompletionOptions registerTools()
        {
            ChatTool getCurrentWeatherTool = registerWeatherTool();
            ChatTool getModuleNames = registerModuleNamesTool();
            ChatTool getEntityNames = registerGetEntitiesTool();
            return new()
            {
                Tools = { getCurrentWeatherTool, getModuleNames, getEntityNames }
            };
        }

        private ChatTool registerWeatherTool()
        {
            return ChatTool.CreateFunctionTool(
            nameof(ModelTools.GetWeather),
            "Get weather information for the provided city",
            BinaryData.FromString(
                @"
                    {
                        ""type"": ""object"",
                        ""properties"": {
                        ""city"": {
                            ""type"": ""string"",
                            ""description"": ""The name of the city, e.g. Boston""
                        }
                        },
                        ""required"": [""city""]
                    }
                ")
             );
        }

        private ChatTool registerModuleNamesTool()
        {
            //return ChatTool.CreateFunctionTool(nameof(ModelTools.GetModules),"Get names of the modules available in the current Mendix app");
            return ChatTool.CreateFunctionTool(
            nameof(ModelTools.GetModules),
            "Get names of the modules available in the current Mendix app"
             );
        }

        private ChatTool registerGetEntitiesTool()
        {
            return ChatTool.CreateFunctionTool(
            nameof(ModelTools.GetEntities),
            "Get names of entities in the provided module name",
            BinaryData.FromString(
                @"
                    {
                        ""type"": ""object"",
                        ""properties"": {
                        ""module"": {
                            ""type"": ""string"",
                            ""description"": ""The name of the module, e.g. Administration, CommunityCommons""
                        }
                        },
                        ""required"": [""module""]
                    }
                ")
             );
        }
    }
}
