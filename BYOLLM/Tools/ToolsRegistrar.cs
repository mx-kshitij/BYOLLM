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
            //ChatTool sendMessageTool = registerSendMessageTool();
            ChatTool getModuleNamesTool = registerModuleNamesTool();
            ChatTool getEntityNamesTool = registerGetEntitiesTool();
            ChatTool getEntityAttributesTool = registerGetEntityAttributesTool();
            ChatTool getEntityAssociationsTool = registerGetEntityAssociationsTool();
            ChatTool createEntityTool = registerCreateEntityTool();
            ChatTool moveEntityTool = registerMoveEntityTool();
            ChatTool removeEntityTool = registerRemoveEntityTool();
            ChatTool generalizeEntityTool = registerGeneralizeEntityTool();
            ChatTool removeEntityGeneralizationTool = registerRemoveEntityGeneralizationTool();
            ChatTool createAttributeTool = registerCreateAttributeTool();
            ChatTool createAttributesTool = registerCreateAttributesTool();
            ChatTool createAssociationTool = registerCreateAssociationTool();
            ChatTool removeAttributeTool = registerRemoveAttributeTool();
            ChatTool removeAttributesTool = registerRemoveAttributesTool();
            ChatTool removeAssociationTool = registerRemoveAssociationTool();
            ChatTool createEnumerationTool = registerCreateEnumeration();
            ChatTool addEnumerationItemsTool = registerAddEnumerationItems();
            ChatTool removeEnumerationItemsTool = registerRemoveEnumerationItems();
            ChatTool removeEnumerationTool = registerRemoveEnumeration();
            ChatTool getEnumerationValuesTool = registerGetEnumerationValues();
            ChatTool updateEnumerationItemTranslationsTool = registerUpdateEnumerationItemTranslations();
            return new ChatCompletionOptions()
            {
                Tools = {
                    getCurrentWeatherTool,
                    //sendMessageTool,
                    getModuleNamesTool,
                    getEntityNamesTool,
                    getEntityAttributesTool,
                    getEntityAssociationsTool,
                    createEntityTool,
                    moveEntityTool,
                    removeEntityTool,
                    generalizeEntityTool,
                    removeEntityGeneralizationTool,
                    createAttributeTool,
                    createAttributesTool,
                    createAssociationTool,
                    removeAttributeTool,
                    removeAttributesTool,
                    removeAssociationTool,
                    createEnumerationTool,
                    addEnumerationItemsTool,
                    removeEnumerationItemsTool,
                    removeEnumerationTool,
                    getEnumerationValuesTool,
                    updateEnumerationItemTranslationsTool
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

        private ChatTool registerGetEntityAssociationsTool()
        {
            return ChatTool.CreateFunctionTool(
            nameof(EntityTools.GetAssociations),
            "Get names of associations of an entity in the provided module name. The output is a mix of information and JSON.",
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
            "Accepts the name of a module, an entity (persistent or non-persistent) and it's location coordinate x & y to create the entity in the module's domain model at a location",
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
                       },  
                       ""isPersistent"": {  
                           ""type"": ""boolean"",  
                           ""description"": ""Whether the entity should be persistent(true) or non-persistent (false)""
                       },  
                       ""documentation"": {  
                           ""type"": ""string"",  
                           ""description"": ""Brief text explaining what the entity is for""
                       },  
                       ""hasChangedDate"": {  
                           ""type"": ""boolean"",  
                           ""description"": ""Whether the system attribute cchangedDate should be enabled. It tracks the last changed timestamp of the record without need of user programming.""
                       }, 
                       ""hasCreatedDate"": {  
                           ""type"": ""boolean"",  
                           ""description"": ""Whether the system attribute createdDate should be enabled. It tracks the created timestamp of the record without need of user programming.""
                       },  
                       ""hasOwner"": {  
                           ""type"": ""boolean"",  
                           ""description"": ""Whether the system attribute owner should be enabled. It tracks the user who created the record without need of user programming.""
                       },  
                       ""hasChangedBy"": {  
                           ""type"": ""boolean"",  
                           ""description"": ""Whether the system attribute changedBy should be enabled. It tracks the user who last changed the record without need of user programming.""
                       }
                       },
                       ""required"": [""module"", ""entity"",""locationX"",""locationY""]
                   }  
               ")
             );
        }

        private ChatTool registerMoveEntityTool()
        {
            return ChatTool.CreateFunctionTool(
            nameof(ModelTools.MoveEntity),
            "Accepts the name of a module, an entity and it's new location coordinate x & y to move the entity in the module's domain model at a new location",
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
                       ""newLocationX"": {  
                           ""type"": ""integer"",  
                           ""description"": ""The x coordinate of the location for entity relocation""  
                       },  
                       ""newLocationY"": {  
                           ""type"": ""integer"",  
                           ""description"": ""The y coordinate of the location for entity relocation""  
                       } 
                       },
                       ""required"": [""module"", ""entity"",""newLocationX"",""newLocationY""]
                   }  
               ")
             );
        }

        private ChatTool registerRemoveEntityTool()
        {
            return ChatTool.CreateFunctionTool(
            nameof(ModelTools.RemoveEntity),
            "Accepts the name of a module, an entity to remove the entity in the module's domain model",
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

        private ChatTool registerGeneralizeEntityTool()
        {
            return ChatTool.CreateFunctionTool(
            nameof(ModelTools.GeneralizeEntity),
            "Accepts the name of the module which has target entity, an entity to target, the module name of the parent entity and the parent entity which the target should inherit. Generalization is Mendix's way of creating inheritance. For e.g. Banana and Apple are both Fruits, so in this case Fruit is the parent entity while Banana and Apple are target entities.",
            BinaryData.FromString(
                @"  
                   {  
                       ""type"": ""object"",  
                       ""properties"": {  
                       ""parentModule"": {  
                           ""type"": ""string"",  
                           ""description"": ""The name of the module which has the parent entity, e.g. Administration, CommunityCommons""  
                       },  
                       ""parentEntity"": {  
                           ""type"": ""string"",  
                           ""description"": ""The name of the entity within the parent module, e.g. Account, User""  
                       },
                       ""targetModule"": {  
                           ""type"": ""string"",  
                           ""description"": ""The name of the module which has the target entity, e.g. Administration, CommunityCommons""  
                       },  
                       ""targetEntity"": {  
                           ""type"": ""string"",  
                           ""description"": ""The name of the entity which should inherit from the parent entity, e.g. Account, User""  
                       }
                       },
                       ""required"": [""parentModule"", ""parentEntity"", ""targetModule"", ""targetEntity""]
                   }  
               ")
             );
        }

        private ChatTool registerRemoveEntityGeneralizationTool()
        {
            return ChatTool.CreateFunctionTool(
            nameof(ModelTools.RemoveEntityGeneralization),
            "Accepts the name of a module, an entity to remove the generalization (inheritance) of the entity in the module's domain model",
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

        private ChatTool registerCreateAssociationTool()
        {
            return ChatTool.CreateFunctionTool(
            nameof(EntityTools.CreateAssociation),
            "Accepts the name of the module of the origin entity, the origin entity, the module name of the destination entity, the destination entity and a type of association between the 2 entities. Key Rule: In a Nto1 association, the origin is the entity that has many. The association is created from the origin to the destination. Example: A Team has many Players. So Player has a Nto1 relation with Team. Player is the origin (N). Team is the destination (1).",
            BinaryData.FromString(
                @"  
                   {  
                       ""type"": ""object"",  
                       ""properties"": {  
                       ""originModule"": {  
                           ""type"": ""string"",  
                           ""description"": ""The name of the module which as the origin entity, e.g. Administration, CommunityCommons""  
                       },  
                       ""originEntity"": {  
                           ""type"": ""string"",  
                           ""description"": ""The name of the origin entity within the origin module, e.g. Account, User""  
                       },  
                       ""destinationModule"": {  
                           ""type"": ""string"",  
                           ""description"": ""The name of the module which as the destination entity, e.g. Administration, CommunityCommons""  
                       },  
                       ""destinationEntity"": {  
                           ""type"": ""string"",  
                           ""description"": ""The name of the destination entity within the destination module, e.g. Account, User""  
                       },  
                       ""associationType"": {  
                           ""type"": ""string"",  
                           ""description"": ""The type of the association from the origin to the destination. Possible values are '1to1', 'Nto1' or 'NtoN'""
                       }
                       },
                       ""required"": [""originModule"", ""originEntity"",""destinationModule"",""destinationEntity"",""associationType""]
                   }  
               ")
             );
        }

        private ChatTool registerRemoveAttributeTool()
        {
            return ChatTool.CreateFunctionTool(
            nameof(EntityTools.RemoveAttribute),
            "Accepts the name of a module, an entity and an attribute name to delete the attribute in the entity mentioned",
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
                           ""description"": ""The name of the attribute to be deleted.""  
                       }
                       },
                       ""required"": [""module"", ""entity"",""attributeName""]
                   }  
               ")
             );
        }

        private ChatTool registerRemoveAttributesTool()
        {
            return ChatTool.CreateFunctionTool(
            nameof(EntityTools.RemoveAttributes),
            "Accepts the name of a module, an entity and a list of attributes to delete the attribute in the entity mentioned. This tool should take priority if multiple attributes need to be removed from the same entity.",
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
                                      ""description"": ""The name of the attribute to be deleted.""  
                                  } 
                              },  
                              ""required"": [""name""]  
                          }  
                      }
                       },
                       ""required"": [""module"", ""entity"",""attributes""]
                   }  
               ")
             );
        }

        private ChatTool registerRemoveAssociationTool()
        {
            return ChatTool.CreateFunctionTool(
            nameof(EntityTools.RemoveAssociation),
            "Accepts the name of a module, an entity and an association to delete the association of the entity mentioned",
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
                       ""association"": {  
                           ""type"": ""string"",
                           ""description"": ""The name of the association to be removed.""  
                       }
                       },
                       ""required"": [""module"", ""entity"",""association""]
                   }  
               ")
             );
        }

        private ChatTool registerCreateEnumeration()
        {
            return ChatTool.CreateFunctionTool(
            nameof(ModelTools.CreateEnumeration),
            "Accepts the name of a module, an enumeration, language code and a list of values to create an enumeration with values. ",
            BinaryData.FromString(
                @"  
                   {  
                       ""type"": ""object"",
                       ""properties"": {  
                       ""module"": {  
                           ""type"": ""string"",  
                           ""description"": ""The name of the module, e.g. Administration, CommunityCommons""  
                       },  
                       ""enumeration"": {  
                           ""type"": ""string"",  
                           ""description"": ""The name of the enumeration within the module, e.g. ENUM_StatusOptions, ENUM_YesNo, ENUM_TypesOfCars""  
                       },  
                       ""language"": {  
                           ""type"": ""string"",  
                           ""description"": ""The language code for the app, e.g. en_US, nl_NL, etc. Default is en_US""  
                       },
                       ""values"": {  
                          ""type"": ""array"", 
                          ""description"": ""A list of values to be created, each with a name and value"",  
                          ""items"": {  
                              ""type"": ""object"",  
                              ""properties"": {  
                                  ""value"": {  
                                      ""type"": ""string"",  
                                      ""description"": ""The value to be created. This is the translation for the language specified.""  
                                  },
                                 ""name"": {  
                                      ""type"": ""string"",  
                                      ""description"": ""The name of the item. It must start with a letter or underscore and can only contain letters, digits and underscores. This must be unique for the enumeration and not language dependent.""  
                                  }
                              },  
                              ""required"": [""value"",""name""]  
                          }  
                      }
                       },
                       ""required"": [""module"", ""enumeration"",""values"", ""language""]
                   }  
               ")
             );
        }

        private ChatTool registerAddEnumerationItems()
        {
            return ChatTool.CreateFunctionTool(
            nameof(ModelTools.AddEnumerationItems),
            "Accepts the name of a module, an enumeration, language code and a list of values to add to an existing enumeration. ",
            BinaryData.FromString(
                @"{  
                       ""type"": ""object"",
                       ""properties"": {  
                       ""module"": {  
                           ""type"": ""string"",  
                           ""description"": ""The name of the module, e.g. Administration, CommunityCommons""  
                       },  
                       ""enumeration"": {  
                           ""type"": ""string"",
                           ""description"": ""The name of the enumeration within the module, e.g. ENUM_StatusOptions, ENUM_YesNo, ENUM_TypesOfCars""  
                       },  
                       ""language"": {  
                           ""type"": ""string"",  
                           ""description"": ""The language code for the app, e.g. en_US, nl_NL, etc. Default is en_US""  
                       },
                       ""values"": {  
                          ""type"": ""array"", 
                          ""description"": ""A list of values to be created, each with a name and value"",  
                          ""items"": {  
                              ""type"": ""object"",  
                              ""properties"": {  
                                  ""value"": {  
                                      ""type"": ""string"",  
                                      ""description"": ""The value to be created. This is the translation for the language specified.""  
                                  },
                                 ""name"": {  
                                      ""type"": ""string"",  
                                      ""description"": ""The name of the item. It must start with a letter or underscore and can only contain letters, digits and underscores. This must be unique for the enumeration and not language dependent.""  
                                  }
                              },  
                              ""required"": [""value"",""name""]  
                          }  
                      }
                       },
                       ""required"": [""module"", ""enumeration"",""values"", ""language""]
                   }  
               ")
             );
        }

        private ChatTool registerRemoveEnumerationItems()
        {
            return ChatTool.CreateFunctionTool(
            nameof(ModelTools.RemoveEnumerationItems),
            "Accepts the name of a module, an enumeration and a list of values to remove from the existing enumeration. ",
            BinaryData.FromString(
                @"  
                   {  
                       ""type"": ""object"",
                       ""properties"": {  
                       ""module"": {  
                           ""type"": ""string"",  
                           ""description"": ""The name of the module, e.g. Administration, CommunityCommons""  
                       },  
                       ""enumeration"": {  
                           ""type"": ""string"",
                           ""description"": ""The name of the enumeration within the module, e.g. ENUM_StatusOptions, ENUM_YesNo, ENUM_TypesOfCars""  
                       },
                       ""values"": {  
                          ""type"": ""array"", 
                          ""description"": ""A list of values to be removed, each with a name"",  
                          ""items"": {  
                              ""type"": ""object"",  
                              ""properties"": {
                                 ""name"": {  
                                      ""type"": ""string"",  
                                      ""description"": ""The name of the item. It must start with a letter or underscore and can only contain letters, digits and underscores. This must be unique for the enumeration and not language dependent. This must already exist in the enumeration.""  
                                  }
                              },  
                              ""required"": [""name""]  
                          }  
                      }
                       },
                       ""required"": [""module"", ""enumeration"",""values""]
                   }  
               ")
             );
        }

        private ChatTool registerRemoveEnumeration()
        {
            return ChatTool.CreateFunctionTool(
            nameof(ModelTools.RemoveEnumeration),
            "Accepts the name of a module and an enumeration name to delete an existing enumeration. ",
            BinaryData.FromString(
                @"  
                   {  
                       ""type"": ""object"",
                       ""properties"": {  
                       ""module"": {  
                           ""type"": ""string"",  
                           ""description"": ""The name of the module, e.g. Administration, CommunityCommons""  
                       },  
                       ""enumeration"": {  
                           ""type"": ""string"",
                           ""description"": ""The name of the enumeration within the module, e.g. ENUM_StatusOptions, ENUM_YesNo, ENUM_TypesOfCars""  
                       }
                       },
                       ""required"": [""module"", ""enumeration""]
                   }  
               ")
             );
        }

        private ChatTool registerGetEnumerationValues()
        {
            return ChatTool.CreateFunctionTool(
            nameof(ModelTools.GetEnumerationValues),
            "Accepts the name of a module and an enumeration to get values from an existing enumeration. ",
            BinaryData.FromString(
                @"  
                   {  
                       ""type"": ""object"",
                       ""properties"": {  
                       ""module"": {  
                           ""type"": ""string"",  
                           ""description"": ""The name of the module, e.g. Administration, CommunityCommons""  
                       },  
                       ""enumeration"": {  
                           ""type"": ""string"",
                           ""description"": ""The name of the enumeration within the module, e.g. ENUM_StatusOptions, ENUM_YesNo, ENUM_TypesOfCars""  
                       },  
                       ""language"": {  
                           ""type"": ""string"",  
                           ""description"": ""The language code for the app, e.g. en_US, nl_NL, etc. Default is en_US""  
                       }
                       },
                       ""required"": [""module"", ""enumeration"",""language""]
                   }  
               ")
             );
        }

        private ChatTool registerUpdateEnumerationItemTranslations()
        {
            return ChatTool.CreateFunctionTool(
            nameof(ModelTools.UpdateEnumerationItems),
            "Accepts the name of a module, an enumeration, language code and a list of values to update translations of the existing enumeration. ",
            BinaryData.FromString(
                @"  
                   {  
                       ""type"": ""object"",
                       ""properties"": {  
                       ""module"": {  
                           ""type"": ""string"",  
                           ""description"": ""The name of the module, e.g. Administration, CommunityCommons""  
                       },  
                       ""enumeration"": {  
                           ""type"": ""string"",
                           ""description"": ""The name of the enumeration within the module, e.g. ENUM_StatusOptions, ENUM_YesNo, ENUM_TypesOfCars""  
                       },  
                       ""language"": {  
                           ""type"": ""string"",  
                           ""description"": ""The language code for the app, e.g. en_US, nl_NL, etc.""  
                       },
                       ""values"": {  
                          ""type"": ""array"", 
                          ""description"": ""A list of values to be updated, each with a name and value"",  
                          ""items"": {  
                              ""type"": ""object"",  
                              ""properties"": {  
                                  ""originalName"": {  
                                      ""type"": ""string"",  
                                      ""description"": ""The name of an existing item. It must start with a letter or underscore and can only contain letters, digits and underscores. This must be unique for the enumeration and not language dependent. This should already exist in the enumeration.""  
                                  },
                                  ""value"": {  
                                      ""type"": ""string"",  
                                      ""description"": ""The value to be added. This is the translation for the language specified.""  
                                  },
                                 ""name"": {  
                                      ""type"": ""string"",  
                                      ""description"": ""The name of an existing item. This should be the same as originalName unless this needs to be updated.""  
                                  }
                              },  
                              ""required"": [""value"",""name"",""originalName""]  
                          }  
                      }
                       },
                       ""required"": [""module"", ""enumeration"",""values"", ""language""]
                   }  
               ")
             );
        }
    }
}
