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
            ChatTool sendMessageTool = registerSendMessageTool();
            ChatTool getModuleNamesTool = registerModuleNamesTool();
            ChatTool getEntityNamesTool = registerGetEntitiesTool();
            ChatTool getEntityAttributesTool = registerGetEntityAttributesTool();
            ChatTool createEntityTool = registerCreateEntityTool();
            ChatTool createAttributeTool = registerCreateAttributeTool();
            ChatTool createAttributesTool = registerCreateAttributesTool();
            return new()
            {
                Tools = {
                    getCurrentWeatherTool,
                    sendMessageTool,
                    getModuleNamesTool,
                    getEntityNamesTool,
                    getEntityAttributesTool,
                    createEntityTool,
                    createAttributeTool,
                    createAttributesTool
                }
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

        private ChatTool registerSendMessageTool()
        {
            return ChatTool.CreateFunctionTool(
            nameof(ChatTools.SendMessage),
            "Send a message to the user to response to their query or ask a question",
            BinaryData.FromString(
                @"
                    {
                        ""type"": ""object"",
                        ""properties"": {
                        ""message"": {
                            ""type"": ""string"",
                            ""description"": ""The message for the user. This can be a response to their previous question or a question for the user for further processing.""
                        }
                        },
                        ""required"": [""message""]
                    }
                ")
             );
        }

        private ChatTool registerModuleNamesTool()
        {
            //return ChatTool.CreateFunctionTool(nameof(ModelTools.GetModules),"Get names of the modules available in the current Mendix app");
            return ChatTool.CreateFunctionTool(
            nameof(ModelTools.GetModules),
            "Get names of the modules available in the current Mendix app. The output is a mix of information and JSON."
             );
        }

        private ChatTool registerGetEntitiesTool()
        {
            return ChatTool.CreateFunctionTool(
            nameof(ModelTools.GetEntities),
            "Get names of entities in the provided module name. The output is a mix of information and JSON.",
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

        private ChatTool registerGetEntityAttributesTool()
        {
            return ChatTool.CreateFunctionTool(
            nameof(EntityTools.GetAttributes),
            "Accepts the name of a module and an entity to return existing attributes in the entity",
            BinaryData.FromString(
                @"  
                   {  
                       ""type"": ""object"",  
                       ""properties"": {  
                       ""module"": {  
                           ""type"": ""string"",  
                           ""description"": ""The name of the module, e.g. Administration, CommunityCommons""  
                       },  
                       ""entity"": {  
                           ""type"": ""string"",  
                           ""description"": ""The name of the entity within the module, e.g. Account, User""  
                       }
                       },  
                       ""required"": [""module"", ""entity""]  
                   }  
               ")
             );
        }

        private ChatTool registerCreateEntityTool()
        {
            return ChatTool.CreateFunctionTool(
            nameof(ModelTools.CreateEntity),
            "Accepts the name of a module, an entity and it's location coordinate x & y to create the entity in the module's domain model at a location",
            BinaryData.FromString(
                @"  
                   {  
                       ""type"": ""object"",  
                       ""properties"": {  
                       ""module"": {  
                           ""type"": ""string"",  
                           ""description"": ""The name of the module, e.g. Administration, CommunityCommons""  
                       },  
                       ""entity"": {  
                           ""type"": ""string"",  
                           ""description"": ""The name of the entity within the module, e.g. Account, User""  
                       },  
                       ""locationX"": {  
                           ""type"": ""integer"",  
                           ""description"": ""The x coordinate of the location for entity creation""  
                       },  
                       ""locationY"": {  
                           ""type"": ""integer"",  
                           ""description"": ""The y coordinate of the location for entity creation""  
                       } 
                       },
                       ""required"": [""module"", ""entity"",""locationX"",""locationY""]
                   }  
               ")
             );
        }

        private ChatTool registerCreateAttributeTool()
        {
            return ChatTool.CreateFunctionTool(
            nameof(EntityTools.CreateAttribute),
            "Accepts the name of a module, an entity and a new attribute with name and type to create a new attribute in the entity mentioned",
            BinaryData.FromString(
                @"  
                   {  
                       ""type"": ""object"",  
                       ""properties"": {  
                       ""module"": {  
                           ""type"": ""string"",  
                           ""description"": ""The name of the module, e.g. Administration, CommunityCommons""  
                       },  
                       ""entity"": {  
                           ""type"": ""string"",  
                           ""description"": ""The name of the entity within the module, e.g. Account, User""  
                       },  
                       ""attributeName"": {  
                           ""type"": ""string"",  
                           ""description"": ""The name of the attribute to be created. Name should start with a letter or an underscore and can only contain letter, digits and underscore.""  
                       },  
                       ""attributeType"": {  
                           ""type"": ""string"",  
                           ""description"": ""The type of attribute to be created. Type can be string, integer, long, boolean, decimal, datetime, binary, enumeration, autonumber or hashedstring. An enumeration object needs to be created before it is used as an attribute type.""  
                       } 
                       },
                       ""required"": [""module"", ""entity"",""attributeName"",""attributeType""]
                   }  
               ")
             );
        }

        private ChatTool registerCreateAttributesTool()
        {
            return ChatTool.CreateFunctionTool(
            nameof(EntityTools.CreateAttributes),
            "Accepts the name of a module, an entity and a list of attributes with name and type to create attributes in the entity mentioned. This tool should have preferance over the tool which creates 1 attribute at a time.",
            BinaryData.FromString(
                @"  
                   {  
                       ""type"": ""object"",  
                       ""properties"": {  
                       ""module"": {  
                           ""type"": ""string"",  
                           ""description"": ""The name of the module, e.g. Administration, CommunityCommons""  
                       },  
                       ""entity"": {  
                           ""type"": ""string"",  
                           ""description"": ""The name of the entity within the module, e.g. Account, User""  
                       },
                       ""attributes"": {  
                          ""type"": ""array"",  
                          ""description"": ""A list of attributes to be created, each with a name and type"",  
                          ""items"": {  
                              ""type"": ""object"",  
                              ""properties"": {  
                                  ""name"": {  
                                      ""type"": ""string"",  
                                      ""description"": ""The name of the attribute to be created. Name should start with a letter or an underscore and can only contain letter, digits and underscore.""  
                                  },  
                                  ""type"": {  
                                      ""type"": ""string"",  
                                      ""description"": ""The type of the attribute to be created. Type can be string, integer, long, boolean, decimal, datetime, binary, enumeration, autonumber or hashedstring. An enumeration object needs to be created before it is used as an attribute type.""
                                  }  
                              },  
                              ""required"": [""name"", ""type""]  
                          }  
                      }
                       },
                       ""required"": [""module"", ""entity"",""attributes""]
                   }  
               ")
             );
        }
    }
}
