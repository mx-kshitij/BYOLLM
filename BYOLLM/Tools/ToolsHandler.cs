using Mendix.StudioPro.ExtensionsAPI.Model;
using Mendix.StudioPro.ExtensionsAPI.Services;
using Mendix.StudioPro.ExtensionsAPI.UI.WebView;
using OpenAI.Chat;
using System.Text.Json;

namespace Odin
{
    public class ToolsHandler
    {
        private IModel currentApp;
        //private ModelTools modelTools;
        private IWebView webView;
        private IDomainModelService domainModelService;

        public ToolsHandler(IModel _currentApp, IWebView _webView, IDomainModelService _domainModelService)
        {
            currentApp = _currentApp;
            webView = _webView;
            //modelTools = new ModelTools();
            domainModelService = _domainModelService;
        }
        public int HandleTool(ChatToolCall toolCall, ref string response)
        {
            switch (toolCall.FunctionName)
            {
                case nameof(ModelTools.GetWeather):
                    return HandleGetWeather(toolCall, ref response);

                //case nameof(ChatTools.SendMessage):
                //    return HandleSendMessage(toolCall, ref response);

                case nameof(ModelTools.GetModules):
                    return HandleGetModules(toolCall, ref response);

                case nameof(ModelTools.GetEntities):
                    return HandleGetEntities(toolCall, ref response);

                case nameof(EntityTools.GetAttributes):
                    return HandleGetEntityAttributes(toolCall, ref response);

                case nameof(EntityTools.GetAssociations):
                    return HandleGetEntityAssociations(toolCall, ref response);

                case nameof(ModelTools.CreateEntity):
                    return HandleCreateEntity(toolCall, ref response);

                case nameof(ModelTools.MoveEntity):
                    return HandleMoveEntity(toolCall, ref response);

                case nameof(ModelTools.RemoveEntity):
                    return HandleRemoveEntity(toolCall, ref response);

                case nameof(ModelTools.GeneralizeEntity):
                    return HandleGeneralizeEntity(toolCall, ref response);

                case nameof(ModelTools.RemoveEntityGeneralization):
                    return HandleRemoveEntityGeneralization(toolCall, ref response);

                case nameof(EntityTools.CreateAttribute):
                    return HandleCreateAttribute(toolCall, ref response);

                case nameof(EntityTools.CreateAttributes):
                    return HandleCreateAttributes(toolCall, ref response);

                case nameof(EntityTools.CreateAssociation):
                    return HandleCreateAssociation(toolCall, ref response);

                case nameof(EntityTools.RemoveAttribute):
                    return HandleRemoveAttribute(toolCall, ref response);

                case nameof(EntityTools.RemoveAttributes):
                    return HandleRemoveAttributes(toolCall, ref response);

                case nameof(EntityTools.RemoveAssociation):
                    return HandleRemoveAssociation(toolCall, ref response);

                case nameof(ModelTools.CreateEnumeration):
                    return HandleCreateEnumeration(toolCall, ref response);

                case nameof(ModelTools.AddEnumerationItems):
                    return HandleAddEnumerationItems(toolCall, ref response);

                case nameof(ModelTools.RemoveEnumerationItems):
                    return HandleRemoveEnumerationItems(toolCall, ref response);

                case nameof(ModelTools.RemoveEnumeration):
                    return HandleRemoveEnumeration(toolCall, ref response);

                case nameof(ModelTools.GetEnumerationValues):
                    return HandleGetEnumerationValues(toolCall, ref response);

                case nameof(ModelTools.UpdateEnumerationItems):
                    return HandleUpdateEnumerationItems(toolCall, ref response);

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

        //public int HandleSendMessage(ChatToolCall toolCall, ref string response)
        //{
        //    try
        //    {
        //        using JsonDocument argumentsDocument =
        //            JsonDocument.Parse(toolCall.FunctionArguments);

        //        if (argumentsDocument.RootElement.TryGetProperty("message", out JsonElement messageElement)
        //            && !string.IsNullOrEmpty(messageElement.GetString()))
        //        {
        //            ChatTools.SendMessage(webView, messageElement.GetString()!);
        //            return 0;
        //        }
        //        response = "Invalid or missing 'message' argument.";
        //        return 0;
        //    }
        //    catch (JsonException ex)
        //    {
        //        response = $"Error parsing JSON: {ex.Message}";
        //        return 0;
        //    }
        //}

        private int HandleGetModules(ChatToolCall toolCall, ref string response)
        {
            try
            {
                response = ModelTools.GetModules(currentApp);
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
                using JsonDocument argumentsDocument = JsonDocument.Parse(toolCall.FunctionArguments);

                argumentsDocument.RootElement.TryGetProperty("module", out JsonElement moduleElement);

                if (!string.IsNullOrEmpty(moduleElement.GetString()))
                {
                    response = ModelTools.GetEntities(currentApp, moduleElement.GetString()!);
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
                using JsonDocument argumentsDocument = JsonDocument.Parse(toolCall.FunctionArguments);

                argumentsDocument.RootElement.TryGetProperty("module", out JsonElement moduleElement);
                argumentsDocument.RootElement.TryGetProperty("entity", out JsonElement entityElement);

                if (!string.IsNullOrEmpty(moduleElement.GetString()) &&
                   !string.IsNullOrEmpty(entityElement.GetString()))
                {
                    response = EntityTools.GetAttributes(currentApp, moduleElement.GetString()!, entityElement.GetString()!);
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

        private int HandleGetEntityAssociations(ChatToolCall toolCall, ref string response)
        {
            try
            {
                using JsonDocument argumentsDocument = JsonDocument.Parse(toolCall.FunctionArguments);

                argumentsDocument.RootElement.TryGetProperty("module", out JsonElement moduleElement);
                argumentsDocument.RootElement.TryGetProperty("entity", out JsonElement entityElement);

                if (!string.IsNullOrEmpty(moduleElement.GetString()) &&
                   !string.IsNullOrEmpty(entityElement.GetString()))
                {
                    response = new EntityTools(domainModelService).GetAssociations(currentApp, moduleElement.GetString()!, entityElement.GetString()!);
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
                using JsonDocument argumentsDocument = JsonDocument.Parse(toolCall.FunctionArguments);

                argumentsDocument.RootElement.TryGetProperty("module", out JsonElement moduleElement);
                argumentsDocument.RootElement.TryGetProperty("entity", out JsonElement entityElement);
                argumentsDocument.RootElement.TryGetProperty("locationX", out JsonElement locationXElement);
                argumentsDocument.RootElement.TryGetProperty("locationY", out JsonElement locationYElement);
                argumentsDocument.RootElement.TryGetProperty("isPersistent", out JsonElement isPersistentElement);
                argumentsDocument.RootElement.TryGetProperty("documentation", out JsonElement documentationElement);
                argumentsDocument.RootElement.TryGetProperty("hasChangedDate", out JsonElement hasChangedDateElement);
                argumentsDocument.RootElement.TryGetProperty("hasCreatedDate", out JsonElement hasCreatedDateElement);
                argumentsDocument.RootElement.TryGetProperty("hasOwner", out JsonElement hasOwnerElement);
                argumentsDocument.RootElement.TryGetProperty("hasChangedBy", out JsonElement hasChangedByElement);

                if (!string.IsNullOrEmpty(moduleElement.GetString()) &&
                   !string.IsNullOrEmpty(entityElement.GetString()) &&
                   locationXElement.ValueKind == JsonValueKind.Number && int.IsPositive(locationXElement.GetInt32()) &&
                   locationYElement.ValueKind == JsonValueKind.Number && int.IsPositive(locationYElement.GetInt32()))
                {
                    // Set default to false if value is empty or undefined
                    bool isPersistent = isPersistentElement.ValueKind == JsonValueKind.True ? true : isPersistentElement.ValueKind == JsonValueKind.False ? false : false;
                    bool hasChangedDate = hasChangedDateElement.ValueKind == JsonValueKind.True ? true : hasChangedDateElement.ValueKind == JsonValueKind.False ? false : false;
                    bool hasCreatedDate = hasCreatedDateElement.ValueKind == JsonValueKind.True ? true : hasCreatedDateElement.ValueKind == JsonValueKind.False ? false : false;
                    bool hasOwner = hasOwnerElement.ValueKind == JsonValueKind.True ? true :  hasOwnerElement.ValueKind == JsonValueKind.False ? false : false;
                    bool hasChangedBy = hasChangedByElement.ValueKind == JsonValueKind.True ? true : hasChangedByElement.ValueKind == JsonValueKind.False ? false : false;

                    response = ModelTools.CreateEntity(
                        currentApp,
                        moduleElement.GetString()!,
                        new EntityModel(
                            entityElement.GetString()!,
                            new Location(locationXElement.GetInt32()!, locationYElement.GetInt32()!),
                            documentationElement.GetString() ?? "",
                            isPersistent,
                            hasChangedDate,
                            hasCreatedDate,
                            hasOwner,
                            hasChangedBy));
                    return 1;
                }
                response = "Invalid or missing 'module', 'entity' or 'other entity' argument.";
                return 0;
            }
            catch (JsonException ex)
            {
                response = $"Error parsing JSON: {ex.Message}";
                return 0;
            }
        }

        private int HandleMoveEntity(ChatToolCall toolCall, ref string response)
        {
            try
            {
                using JsonDocument argumentsDocument = JsonDocument.Parse(toolCall.FunctionArguments);

                argumentsDocument.RootElement.TryGetProperty("module", out JsonElement moduleElement);
                argumentsDocument.RootElement.TryGetProperty("entity", out JsonElement entityElement);
                argumentsDocument.RootElement.TryGetProperty("newLocationX", out JsonElement locationXElement);
                argumentsDocument.RootElement.TryGetProperty("newLocationY", out JsonElement locationYElement);

                if (!string.IsNullOrEmpty(moduleElement.GetString()) &&
                   !string.IsNullOrEmpty(entityElement.GetString()) &&
                   locationXElement.ValueKind == JsonValueKind.Number && int.IsPositive(locationXElement.GetInt32()) &&
                   locationYElement.ValueKind == JsonValueKind.Number && int.IsPositive(locationYElement.GetInt32()))
                {
                    response = ModelTools.MoveEntity(currentApp, moduleElement.GetString()!, entityElement.GetString()!, locationXElement.GetInt32()!, locationYElement.GetInt32()!);
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

        private int HandleRemoveEntity(ChatToolCall toolCall, ref string response)
        {
            try
            {
                using JsonDocument argumentsDocument = JsonDocument.Parse(toolCall.FunctionArguments);

                argumentsDocument.RootElement.TryGetProperty("module", out JsonElement moduleElement);
                argumentsDocument.RootElement.TryGetProperty("entity", out JsonElement entityElement);

                if (!string.IsNullOrEmpty(moduleElement.GetString()) &&
                   !string.IsNullOrEmpty(entityElement.GetString()))
                {
                    response = ModelTools.RemoveEntity(currentApp, moduleElement.GetString()!, entityElement.GetString()!);
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

        private int HandleGeneralizeEntity(ChatToolCall toolCall, ref string response)
        {
            try
            {
                using JsonDocument argumentsDocument = JsonDocument.Parse(toolCall.FunctionArguments);

                argumentsDocument.RootElement.TryGetProperty("parentModule", out JsonElement parentModuleElement);
                argumentsDocument.RootElement.TryGetProperty("parentEntity", out JsonElement parentEntityElement);
                argumentsDocument.RootElement.TryGetProperty("targetModule", out JsonElement targetModuleElement);
                argumentsDocument.RootElement.TryGetProperty("targetEntity", out JsonElement targetEntityElement);

                if (!string.IsNullOrEmpty(parentModuleElement.GetString()) &&
                   !string.IsNullOrEmpty(parentEntityElement.GetString()) &&
                   !string.IsNullOrEmpty(targetModuleElement.GetString()) &&
                   !string.IsNullOrEmpty(targetEntityElement.GetString()))
                {
                    response = ModelTools.GeneralizeEntity(currentApp, parentModuleElement.GetString()!, parentEntityElement.GetString()!, targetModuleElement.GetString()!, targetEntityElement.GetString()!);
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

        private int HandleRemoveEntityGeneralization(ChatToolCall toolCall, ref string response)
        {
            try
            {
                using JsonDocument argumentsDocument = JsonDocument.Parse(toolCall.FunctionArguments);

                argumentsDocument.RootElement.TryGetProperty("module", out JsonElement moduleElement);
                argumentsDocument.RootElement.TryGetProperty("entity", out JsonElement entityElement);

                if (!string.IsNullOrEmpty(moduleElement.GetString()) &&
                   !string.IsNullOrEmpty(entityElement.GetString()))
                {
                    response = ModelTools.RemoveEntityGeneralization(currentApp, moduleElement.GetString()!, entityElement.GetString()!);
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

        private int HandleCreateAttribute(ChatToolCall toolCall, ref string response)
        {
            try
            {
                using JsonDocument argumentsDocument = JsonDocument.Parse(toolCall.FunctionArguments);

                argumentsDocument.RootElement.TryGetProperty("module", out JsonElement moduleElement);
                argumentsDocument.RootElement.TryGetProperty("entity", out JsonElement entityElement);
                argumentsDocument.RootElement.TryGetProperty("attributeName", out JsonElement attributeNameElement);
                argumentsDocument.RootElement.TryGetProperty("attributeType", out JsonElement attributeTypeElement);
                argumentsDocument.RootElement.TryGetProperty("attributeEnumerationName", out JsonElement attributeEnumerationElement);

                if (!string.IsNullOrEmpty(moduleElement.GetString()) &&
                   !string.IsNullOrEmpty(entityElement.GetString()) &&
                   !string.IsNullOrEmpty(attributeNameElement.GetString()) &&
                   !string.IsNullOrEmpty(attributeTypeElement.GetString()))
                {
                    response = new EntityTools(domainModelService).CreateAttribute(currentApp, moduleElement.GetString()!, entityElement.GetString()!, attributeNameElement.GetString()!, attributeTypeElement.GetString()!, attributeEnumerationElement.GetString()!);
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
                using JsonDocument argumentsDocument = JsonDocument.Parse(toolCall.FunctionArguments);

                argumentsDocument.RootElement.TryGetProperty("module", out JsonElement moduleElement);
                argumentsDocument.RootElement.TryGetProperty("entity", out JsonElement entityElement);
                argumentsDocument.RootElement.TryGetProperty("attributes", out JsonElement attributesElement);

                if (!string.IsNullOrEmpty(moduleElement.GetString()) &&
                   !string.IsNullOrEmpty(entityElement.GetString()) &&
                   attributesElement.ValueKind == JsonValueKind.Array && attributesElement.GetArrayLength() > 0)
                {
                    var attributes = new List<AttributeModel>();
                    foreach (var attribute in attributesElement.EnumerateArray())
                    {
                        attribute.TryGetProperty("name", out JsonElement nameElement);
                        attribute.TryGetProperty("type", out JsonElement typeElement);
                        attribute.TryGetProperty("enumerationName", out JsonElement enumerationElement);
                        if (!string.IsNullOrEmpty(nameElement.GetString()) &&
                            !string.IsNullOrEmpty(typeElement.GetString()))
                        {
                            attributes.Add(new AttributeModel(nameElement.GetString()!, typeElement.GetString()!, enumerationElement.GetString()!));
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
                using JsonDocument argumentsDocument = JsonDocument.Parse(toolCall.FunctionArguments);

                argumentsDocument.RootElement.TryGetProperty("originModule", out JsonElement originModuleElement);
                argumentsDocument.RootElement.TryGetProperty("originEntity", out JsonElement originEntityElement);
                argumentsDocument.RootElement.TryGetProperty("destinationModule", out JsonElement destinationModuleElement);
                argumentsDocument.RootElement.TryGetProperty("destinationEntity", out JsonElement destinationEntityElement);
                argumentsDocument.RootElement.TryGetProperty("associationType", out JsonElement associationTypeElement);

                if (!string.IsNullOrEmpty(originModuleElement.GetString()) &&
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

        private int HandleRemoveAttribute(ChatToolCall toolCall, ref string response)
        {
            try
            {
                using JsonDocument argumentsDocument = JsonDocument.Parse(toolCall.FunctionArguments);

                argumentsDocument.RootElement.TryGetProperty("module", out JsonElement moduleElement);
                argumentsDocument.RootElement.TryGetProperty("entity", out JsonElement entityElement);
                argumentsDocument.RootElement.TryGetProperty("attributeName", out JsonElement attributeNameElement);

                if (!string.IsNullOrEmpty(moduleElement.GetString()) &&
                   !string.IsNullOrEmpty(entityElement.GetString()) &&
                   !string.IsNullOrEmpty(attributeNameElement.GetString()))
                {
                    response = EntityTools.RemoveAttribute(currentApp, moduleElement.GetString()!, entityElement.GetString()!, attributeNameElement.GetString()!);
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

        private int HandleRemoveAttributes(ChatToolCall toolCall, ref string response)
        {
            try
            {
                using JsonDocument argumentsDocument = JsonDocument.Parse(toolCall.FunctionArguments);

                argumentsDocument.RootElement.TryGetProperty("module", out JsonElement moduleElement);
                argumentsDocument.RootElement.TryGetProperty("entity", out JsonElement entityElement);
                argumentsDocument.RootElement.TryGetProperty("attributes", out JsonElement attributesElement);

                if (!string.IsNullOrEmpty(moduleElement.GetString()) &&
                   !string.IsNullOrEmpty(entityElement.GetString()) &&
                   attributesElement.ValueKind == JsonValueKind.Array && attributesElement.GetArrayLength() > 0)
                {
                    var attributes = new List<AttributeModel>();
                    foreach (var attribute in attributesElement.EnumerateArray())
                    {
                        if (attribute.TryGetProperty("name", out JsonElement nameElement) &&
                            attribute.TryGetProperty("type", out JsonElement typeElement) &&
                            !string.IsNullOrEmpty(nameElement.GetString()))
                        {
                            attributes.Add(new AttributeModel(nameElement.GetString()!, string.Empty, String.Empty));
                        }
                        else
                        {
                            response = "Invalid attribute object in 'attributes' array.";
                            return 0;
                        }
                    }

                    response = EntityTools.RemoveAttributes(currentApp, moduleElement.GetString()!, entityElement.GetString()!, attributes);
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

        private int HandleRemoveAssociation(ChatToolCall toolCall, ref string response)
        {
            try
            {
                using JsonDocument argumentsDocument = JsonDocument.Parse(toolCall.FunctionArguments);

                argumentsDocument.RootElement.TryGetProperty("module", out JsonElement moduleElement);
                argumentsDocument.RootElement.TryGetProperty("entity", out JsonElement entityElement);
                argumentsDocument.RootElement.TryGetProperty("association", out JsonElement associationElement);

                if (!string.IsNullOrEmpty(moduleElement.GetString()) &&
                   !string.IsNullOrEmpty(entityElement.GetString()) &&
                   !string.IsNullOrEmpty(associationElement.GetString()))
                {
                    response = new EntityTools(domainModelService).RemoveAssociation(currentApp, moduleElement.GetString()!, entityElement.GetString()!, associationElement.GetString()!);
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

        private int HandleCreateEnumeration(ChatToolCall toolCall, ref string response)
        {
            try
            {
                using JsonDocument argumentsDocument = JsonDocument.Parse(toolCall.FunctionArguments);

                argumentsDocument.RootElement.TryGetProperty("module", out JsonElement moduleElement);
                argumentsDocument.RootElement.TryGetProperty("enumeration", out JsonElement enumerationElement);
                argumentsDocument.RootElement.TryGetProperty("values", out JsonElement valuesElement);
                argumentsDocument.RootElement.TryGetProperty("language", out JsonElement languageElement);

                if (!string.IsNullOrEmpty(moduleElement.GetString()) &&
                   !string.IsNullOrEmpty(enumerationElement.GetString()) &&
                   valuesElement.ValueKind == JsonValueKind.Array && valuesElement.GetArrayLength() > 0)
                {
                    var enumItems = new List<EnumerationItemModel>();
                    valuesElement.EnumerateArray().ToList().ForEach(value => {
                        value.TryGetProperty("value", out JsonElement valueElement);
                        value.TryGetProperty("name", out JsonElement nameElement);
                        if (!string.IsNullOrEmpty(valueElement.GetString()))
                        {
                            enumItems.Add(new EnumerationItemModel(valueElement.ToString()!, nameElement.ToString()!, null));
                        }
                    });
                    response = ModelTools.CreateEnumeration(currentApp, moduleElement.GetString()!, enumerationElement.GetString()!, enumItems, languageElement.GetString()!);
                    return 1;
                }
                response = "Invalid or missing 'module', 'enumeration' or 'values' argument.";
                return 0;
            }
            catch (JsonException ex)
            {
                response = $"Error parsing JSON: {ex.Message}";
                return 0;
            }
        }

        private int HandleAddEnumerationItems(ChatToolCall toolCall, ref string response)
        {
            try
            {
                using JsonDocument argumentsDocument = JsonDocument.Parse(toolCall.FunctionArguments);

                argumentsDocument.RootElement.TryGetProperty("module", out JsonElement moduleElement);
                argumentsDocument.RootElement.TryGetProperty("enumeration", out JsonElement enumerationElement);
                argumentsDocument.RootElement.TryGetProperty("values", out JsonElement valuesElement);
                argumentsDocument.RootElement.TryGetProperty("language", out JsonElement languageElement);

                if (!string.IsNullOrEmpty(moduleElement.GetString()) &&
                   !string.IsNullOrEmpty(enumerationElement.GetString()) &&
                   valuesElement.ValueKind == JsonValueKind.Array && valuesElement.GetArrayLength() > 0)
                {
                    var enumItems = new List<EnumerationItemModel>();
                    valuesElement.EnumerateArray().ToList().ForEach(value => {
                        value.TryGetProperty("value", out JsonElement valueElement);
                        value.TryGetProperty("name", out JsonElement nameElement);
                        if (!string.IsNullOrEmpty(valueElement.GetString()))
                        {
                            enumItems.Add(new EnumerationItemModel(valueElement.ToString()!, nameElement.ToString()!, null));
                        }
                    });
                    response = ModelTools.AddEnumerationItems(currentApp, moduleElement.GetString()!, enumerationElement.GetString()!, enumItems, languageElement.GetString()!);
                    return 1;
                }
                response = "Invalid or missing 'module', 'enumeration' or 'values' argument.";
                return 0;
            }
            catch (JsonException ex)
            {
                response = $"Error parsing JSON: {ex.Message}";
                return 0;
            }
        }

        private int HandleRemoveEnumerationItems(ChatToolCall toolCall, ref string response)
        {
            try
            {
                using JsonDocument argumentsDocument = JsonDocument.Parse(toolCall.FunctionArguments);

                argumentsDocument.RootElement.TryGetProperty("module", out JsonElement moduleElement);
                argumentsDocument.RootElement.TryGetProperty("enumeration", out JsonElement enumerationElement);
                argumentsDocument.RootElement.TryGetProperty("values", out JsonElement valuesElement);

                if (!string.IsNullOrEmpty(moduleElement.GetString()) &&
                   !string.IsNullOrEmpty(enumerationElement.GetString()) &&
                   valuesElement.ValueKind == JsonValueKind.Array && valuesElement.GetArrayLength() > 0)
                {
                    var enumItems = new List<EnumerationItemModel>();
                    valuesElement.EnumerateArray().ToList().ForEach(value => {
                        value.TryGetProperty("name", out JsonElement nameElement);
                        if (!string.IsNullOrEmpty(nameElement.GetString()))
                        {
                            enumItems.Add(new EnumerationItemModel("", nameElement.ToString()!, null));
                        }
                    });
                    response = ModelTools.RemoveEnumerationItems(currentApp, moduleElement.GetString()!, enumerationElement.GetString()!, enumItems);
                    return 1;
                }
                response = "Invalid or missing 'module', 'enumeration' or 'values' argument.";
                return 0;
            }
            catch (JsonException ex)
            {
                response = $"Error parsing JSON: {ex.Message}";
                return 0;
            }
        }

        private int HandleRemoveEnumeration(ChatToolCall toolCall, ref string response)
        {
            try
            {
                using JsonDocument argumentsDocument = JsonDocument.Parse(toolCall.FunctionArguments);

                argumentsDocument.RootElement.TryGetProperty("module", out JsonElement moduleElement);
                argumentsDocument.RootElement.TryGetProperty("enumeration", out JsonElement enumerationElement);

                if (!string.IsNullOrEmpty(moduleElement.GetString()) &&
                   !string.IsNullOrEmpty(enumerationElement.GetString()))
                {
                    response = ModelTools.RemoveEnumeration(currentApp, moduleElement.GetString()!, enumerationElement.GetString()!);
                    return 1;
                }
                response = "Invalid or missing 'module' or 'enumeration' argument.";
                return 0;
            }
            catch (JsonException ex)
            {
                response = $"Error parsing JSON: {ex.Message}";
                return 0;
            }
        }

        private int HandleGetEnumerationValues(ChatToolCall toolCall, ref string response)
        {
            try
            {
                using JsonDocument argumentsDocument = JsonDocument.Parse(toolCall.FunctionArguments);

                argumentsDocument.RootElement.TryGetProperty("module", out JsonElement moduleElement);
                argumentsDocument.RootElement.TryGetProperty("enumeration", out JsonElement enumerationElement);
                argumentsDocument.RootElement.TryGetProperty("language", out JsonElement languageElement);

                if (!string.IsNullOrEmpty(moduleElement.GetString()) &&
                   !string.IsNullOrEmpty(enumerationElement.GetString()))
                {
                    response = ModelTools.GetEnumerationValues(currentApp, moduleElement.GetString()!, enumerationElement.GetString()!, languageElement.GetString()!);
                    return 1;
                }
                response = "Invalid or missing 'module' or 'enumeration' argument.";
                return 0;
            }
            catch (JsonException ex)
            {
                response = $"Error parsing JSON: {ex.Message}";
                return 0;
            }
        }

        private int HandleUpdateEnumerationItems(ChatToolCall toolCall, ref string response)
        {
            try
            {
                using JsonDocument argumentsDocument = JsonDocument.Parse(toolCall.FunctionArguments);

                argumentsDocument.RootElement.TryGetProperty("module", out JsonElement moduleElement);
                argumentsDocument.RootElement.TryGetProperty("enumeration", out JsonElement enumerationElement);
                argumentsDocument.RootElement.TryGetProperty("values", out JsonElement valuesElement);
                argumentsDocument.RootElement.TryGetProperty("language", out JsonElement languageElement);

                if (!string.IsNullOrEmpty(moduleElement.GetString()) &&
                   !string.IsNullOrEmpty(enumerationElement.GetString()) &&
                   valuesElement.ValueKind == JsonValueKind.Array && valuesElement.GetArrayLength() > 0)
                {
                    var enumItems = new List<EnumerationItemModel>();
                    valuesElement.EnumerateArray().ToList().ForEach(value => {
                        value.TryGetProperty("value", out JsonElement valueElement);
                        value.TryGetProperty("name", out JsonElement nameElement);
                        value.TryGetProperty("originalName", out JsonElement originalNameElement);
                        if (!string.IsNullOrEmpty(valueElement.GetString()))
                        {
                            enumItems.Add(new EnumerationItemModel(valueElement.ToString()!, nameElement.ToString()!, originalNameElement.ToString()!));
                        }
                    });
                    response = ModelTools.UpdateEnumerationItems(currentApp, moduleElement.GetString()!, enumerationElement.GetString()!, enumItems, languageElement.GetString()!);
                    return 1;
                }
                response = "Invalid or missing 'module', 'enumeration' or 'values' argument.";
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
