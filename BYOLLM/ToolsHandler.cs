using Mendix.StudioPro.ExtensionsAPI.Model;
using Mendix.StudioPro.ExtensionsAPI.Model.DomainModels;
using Mendix.StudioPro.ExtensionsAPI.Services;
using Mendix.StudioPro.ExtensionsAPI.UI.WebView;
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
        private ModelTools modelTools;
        private IWebView webView;
        private IDomainModelService domainModelService;

        public ToolsHandler(IModel _currentApp, IWebView _webView, IDomainModelService _domainModelService)
        {
            currentApp = _currentApp;
            webView = _webView;
            modelTools = new ModelTools();
            domainModelService = _domainModelService;
        }
        public int HandleTool(ChatToolCall toolCall, ref string response)
        {
            switch (toolCall.FunctionName)
            {
                case nameof(ModelTools.GetWeather):
                    return HandleGetWeather(toolCall, ref response);
                case nameof(ChatTools.SendMessage):
                    return HandleSendMessage(toolCall, ref response);
                case nameof(ModelTools.GetModules):
                    return HandleGetModules(toolCall, ref response);
                case nameof(ModelTools.GetEntities):
                    return HandleGetEntities(toolCall, ref response);
                case nameof(EntityTools.GetAttributes):
                    return HandleGetEntityAttributes(toolCall, ref response);
                case nameof(ModelTools.CreateEntity):
                    return HandleCreateEntity(toolCall, ref response);
                case nameof(EntityTools.CreateAttribute):
                    return HandleCreateAttribute(toolCall, ref response);
                case nameof(EntityTools.CreateAttributes):
                    return HandleCreateAttributes(toolCall, ref response);
                case nameof(EntityTools.CreateAssociation):
                    return HandleCreateAssociation(toolCall, ref response);
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

                if (argumentsDocument.RootElement.TryGetProperty("city", out JsonElement cityElement)
                    && !string.IsNullOrEmpty(cityElement.GetString()))
                {
                    response = ModelTools.GetWeather(cityElement.GetString()!);
                    return 1;
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

        public int HandleSendMessage(ChatToolCall toolCall, ref string response)
        {
            try
            {
                using JsonDocument argumentsDocument =
                    JsonDocument.Parse(toolCall.FunctionArguments);

                if (argumentsDocument.RootElement.TryGetProperty("message", out JsonElement messageElement)
                    && !string.IsNullOrEmpty(messageElement.GetString()))
                {
                    ChatTools.SendMessage(webView, messageElement.GetString()!);
                    return 0;
                }
                response = "Invalid or missing 'message' argument.";
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

                if (argumentsDocument.RootElement.TryGetProperty("module", out JsonElement moduleElement)
                    && !string.IsNullOrEmpty(moduleElement.GetString()))
                {
                    response = modelTools.GetEntities(currentApp, moduleElement.GetString()!);
                    return 1;
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

        private int HandleGetEntityAttributes(ChatToolCall toolCall, ref string response)
        {
            try
            {
                using JsonDocument argumentsDocument =
                    JsonDocument.Parse(toolCall.FunctionArguments);


                if (argumentsDocument.RootElement.TryGetProperty("module", out JsonElement moduleElement) &&
                   argumentsDocument.RootElement.TryGetProperty("entity", out JsonElement entityElement) &&
                   !string.IsNullOrEmpty(moduleElement.GetString()) &&
                   !string.IsNullOrEmpty(entityElement.GetString()))
                {
                    response = new EntityTools(domainModelService).GetAttributes(currentApp, moduleElement.GetString()!, entityElement.GetString()!);
                    return 1;
                }
                response = "Invalid or missing 'module' or 'entity' argument.";
                return 0;
            }
            catch (JsonException ex)
            {
                response = $"Error parsing JSON: {ex.Message}";
                return 0;
            }
        }

        private int HandleCreateEntity(ChatToolCall toolCall, ref string response)
        {
            try
            {
                using JsonDocument argumentsDocument =
                    JsonDocument.Parse(toolCall.FunctionArguments);


                if (argumentsDocument.RootElement.TryGetProperty("module", out JsonElement moduleElement) &&
                   argumentsDocument.RootElement.TryGetProperty("entity", out JsonElement entityElement) &&
                   argumentsDocument.RootElement.TryGetProperty("locationX", out JsonElement locationXElement) &&
                   argumentsDocument.RootElement.TryGetProperty("locationY", out JsonElement locationYElement) &&
                   !string.IsNullOrEmpty(moduleElement.GetString()) &&
                   !string.IsNullOrEmpty(entityElement.GetString()) &&
                   locationXElement.ValueKind == JsonValueKind.Number && int.IsPositive(locationXElement.GetInt32()) &&
                   locationYElement.ValueKind == JsonValueKind.Number && int.IsPositive(locationYElement.GetInt32()))
                {
                    response = new ModelTools().CreateEntity(currentApp, moduleElement.GetString()!, entityElement.GetString()!, locationXElement.GetInt32()!, locationYElement.GetInt32()!);
                    return 1;
                }
                response = "Invalid or missing 'module', 'entity' or 'location' argument.";
                return 0;
            }
            catch (JsonException ex)
            {
                response = $"Error parsing JSON: {ex.Message}";
                return 0;
            }
        }

        private int HandleCreateAttribute(ChatToolCall toolCall, ref string response)
        {
            try
            {
                using JsonDocument argumentsDocument =
                    JsonDocument.Parse(toolCall.FunctionArguments);


                if (argumentsDocument.RootElement.TryGetProperty("module", out JsonElement moduleElement) &&
                   argumentsDocument.RootElement.TryGetProperty("entity", out JsonElement entityElement) &&
                   argumentsDocument.RootElement.TryGetProperty("attributeName", out JsonElement attributeNameElement) &&
                   argumentsDocument.RootElement.TryGetProperty("attributeType", out JsonElement attributeTypeElement) &&
                   !string.IsNullOrEmpty(moduleElement.GetString()) &&
                   !string.IsNullOrEmpty(entityElement.GetString()) &&
                   !string.IsNullOrEmpty(attributeNameElement.GetString()) &&
                   !string.IsNullOrEmpty(attributeTypeElement.GetString()))
                {
                    response = new EntityTools(domainModelService).CreateAttribute(currentApp, moduleElement.GetString()!, entityElement.GetString()!, attributeNameElement.GetString()!, attributeTypeElement.GetString()!);
                    return 1;
                }
                response = "Invalid or missing 'module', 'entity' or 'attribute' argument.";
                return 0;
            }
            catch (JsonException ex)
            {
                response = $"Error parsing JSON: {ex.Message}";
                return 0;
            }
        }

        private int HandleCreateAttributes(ChatToolCall toolCall, ref string response)
        {
            try
            {
                using JsonDocument argumentsDocument =
                    JsonDocument.Parse(toolCall.FunctionArguments);

                if (argumentsDocument.RootElement.TryGetProperty("module", out JsonElement moduleElement) &&
                   argumentsDocument.RootElement.TryGetProperty("entity", out JsonElement entityElement) &&
                   argumentsDocument.RootElement.TryGetProperty("attributes", out JsonElement attributesElement) &&
                   !string.IsNullOrEmpty(moduleElement.GetString()) &&
                   !string.IsNullOrEmpty(entityElement.GetString()) &&
                   attributesElement.ValueKind == JsonValueKind.Array && attributesElement.GetArrayLength() > 0)
                {
                    var attributes = new List<AttributeModel>();
                    foreach (var attribute in attributesElement.EnumerateArray())
                    {
                        if (attribute.TryGetProperty("name", out JsonElement nameElement) &&
                            attribute.TryGetProperty("type", out JsonElement typeElement) &&
                            !string.IsNullOrEmpty(nameElement.GetString()) &&
                            !string.IsNullOrEmpty(typeElement.GetString()))
                        {
                            attributes.Add(new AttributeModel(nameElement.GetString()!, typeElement.GetString()!));
                        }
                        else
                        {
                            response = "Invalid attribute object in 'attributes' array.";
                            return 0;
                        }
                    }

                    response = new EntityTools(domainModelService).CreateAttributes(currentApp, moduleElement.GetString()!, entityElement.GetString()!, attributes);
                    return 1;
                }
                response = "Invalid or missing 'module', 'entity' or 'attributes' argument.";
                return 0;
            }
            catch (JsonException ex)
            {
                response = $"Error parsing JSON: {ex.Message}";
                return 0;
            }
        }

        private int HandleCreateAssociation(ChatToolCall toolCall, ref string response)
        {
            try
            {
                using JsonDocument argumentsDocument =
                    JsonDocument.Parse(toolCall.FunctionArguments);


                if (argumentsDocument.RootElement.TryGetProperty("originModule", out JsonElement originModuleElement) &&
                   argumentsDocument.RootElement.TryGetProperty("originEntity", out JsonElement originEntityElement) &&
                   argumentsDocument.RootElement.TryGetProperty("destinationModule", out JsonElement destinationModuleElement) &&
                   argumentsDocument.RootElement.TryGetProperty("destinationEntity", out JsonElement destinationEntityElement) &&
                   argumentsDocument.RootElement.TryGetProperty("associationType", out JsonElement associationTypeElement) &&
                   !string.IsNullOrEmpty(originModuleElement.GetString()) &&
                   !string.IsNullOrEmpty(originEntityElement.GetString()) &&
                   !string.IsNullOrEmpty(destinationModuleElement.GetString()) &&
                   !string.IsNullOrEmpty(destinationEntityElement.GetString()) &&
                   !string.IsNullOrEmpty(associationTypeElement.GetString()))
                {
                    response = new EntityTools(domainModelService).CreateAssociation(currentApp, originModuleElement.GetString()!, originEntityElement.GetString()!, destinationModuleElement.GetString()!, destinationEntityElement.GetString()!, associationTypeElement.GetString()!);
                    return 1;
                }
                response = "Invalid or missing 'module', 'entity' or 'association type' argument.";
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
