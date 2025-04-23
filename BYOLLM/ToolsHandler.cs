using Mendix.StudioPro.ExtensionsAPI.Model;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BYOLLM
{
    public class ToolsHandler
    {
        private IModel currentApp;
        public ModelTools modelTools;
        public ToolsHandler(IModel _currentApp)
        {
            currentApp = _currentApp;
            modelTools = new ModelTools();
        }
        public int HandleTool(ChatToolCall toolCall, ref string response)
        {
            switch (toolCall.FunctionName)
            {
                case nameof(ModelTools.GetWeather):
                    return HandleGetWeather(toolCall, ref response);
                case nameof(ModelTools.GetModules):
                    return HandleGetModules(toolCall, ref response);
                case nameof(ModelTools.GetEntities):
                    return HandleGetEntities(toolCall, ref response);
                default:
                    response = "Unknown function call.";
                    return 0;
            }
        }

        private int HandleGetWeather(ChatToolCall toolCall, ref string response)
        {
            try
            {
                using JsonDocument argumentsDocument =
                    JsonDocument.Parse(toolCall.FunctionArguments);

                if (argumentsDocument.RootElement.TryGetProperty("city",
                    out JsonElement cityElement) &&
                    !string.IsNullOrEmpty(cityElement.GetString()))
                {
                    response = ModelTools.GetWeather(cityElement.GetString()!);
                    return 0;
                }
                response = "Invalid or missing 'city' argument.";
                return 0;
            }
            catch (JsonException ex)
            {
                response = $"Error parsing JSON: {ex.Message}";
                return 0;
            }
        }

        private int HandleGetModules(ChatToolCall toolCall, ref string response)
        {
            try
            {
                response = modelTools.GetModules(currentApp);
                return 1;
            }
            catch (JsonException ex)
            {
                response = $"Error in getting module names: {ex.Message}";
                return 0;
            }
        }

        private int HandleGetEntities(ChatToolCall toolCall, ref string response)
        {
            try
            {
                using JsonDocument argumentsDocument =
                    JsonDocument.Parse(toolCall.FunctionArguments);

                if (argumentsDocument.RootElement.TryGetProperty("module",
                    out JsonElement moduleElement) &&
                    !string.IsNullOrEmpty(moduleElement.GetString()))
                {
                    response = modelTools.GetEntities(currentApp, moduleElement.GetString()!);
                    return 0;
                }
                response = "Invalid or missing 'module' argument.";
                return 0;
            }
            catch (JsonException ex)
            {
                response = $"Error parsing JSON: {ex.Message}";
                return 0;
            }
        }
    }
}
