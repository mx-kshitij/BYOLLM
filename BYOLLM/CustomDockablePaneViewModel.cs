﻿using Mendix.StudioPro.ExtensionsAPI.Model;
using Mendix.StudioPro.ExtensionsAPI.Services;
using Mendix.StudioPro.ExtensionsAPI.UI.DockablePane;
using Mendix.StudioPro.ExtensionsAPI.UI.Services;
using Mendix.StudioPro.ExtensionsAPI.UI.WebView;
using OpenAI.Chat;
using System.Text.Json;

namespace Odin
{
    public class CustomDockablePaneViewModel : WebViewDockablePaneViewModel
    {
        private readonly Uri _baseUri;
        private readonly Func<IModel?> _getCurrentApp;
        private readonly ILogService _logService;
        private readonly IBackgroundJobService _bgService;
        private readonly IMessageBoxService _msgService;
        private readonly IDockingWindowService _dockingWindowService;
        private readonly IDomainModelService _domainModelService;
        private ChatClient? chatClient;
        private ChatCompletion? chatCompletion;
        private List<ChatMessage> conversationHistory;
        private ChatCompletionOptions options;
        private ToolsHandler toolsHandler;

        public CustomDockablePaneViewModel(Uri baseUri, Func<IModel?> getCurrentApp, ILogService logService, IBackgroundJobService bgService, IMessageBoxService msgService, IDockingWindowService dockingWindowService, IDomainModelService domainModelService)
        {
            _baseUri = baseUri;
            _getCurrentApp = getCurrentApp;
            _logService = logService;
            _bgService = bgService;
            _msgService = msgService;
            _dockingWindowService = dockingWindowService;
            _domainModelService = domainModelService;
            chatClient = null;
            chatCompletion = null;
            conversationHistory = new List<ChatMessage>();
            options = new ToolsRegistrar().registerTools();
        }

        public override void InitWebView(IWebView webView)
        {
            webView.Address = new Uri(_baseUri, "index");

            webView.MessageReceived += (_, args) =>
            {
                var currentApp = _getCurrentApp();
                if (currentApp == null) return;
                toolsHandler = new ToolsHandler(currentApp, webView, _domainModelService);

                if (args.Message == "SendNewUserMessage")
                {
                    var requestBody = args.Data.ToJsonString();
                    MessageModel userMessage = new MessageHandler().HandleNewUserMessage(requestBody);
                    chatCompletion = AddUserMessage(userMessage);
                    if(chatCompletion != null)
                    {
                        HandleChatResponse(chatCompletion, webView);
                    }
                }
                else if (args.Message == "InitiateConnection")
                {
                    var requestBody = args.Data.ToJsonString();
                    ConfigurationModel config = JsonSerializer.Deserialize<ConfigurationModel>(requestBody)!;
                    chatClient = new OpenAIConnectionHandler(currentApp, _logService).InitiateConnection(config);

                    try
                    {
                        string systemPrompt = Defaults.defaultSystemPrompt + " " + config.SystemPrompt;
                        chatCompletion = AddSystemMessage(systemPrompt);

                        if(chatCompletion != null)
                        {
                            webView.PostMessage("ConnectionEstablished", null);
                            if (chatCompletion.Content.Count != 0)
                            {
                                conversationHistory.Add(new AssistantChatMessage(chatCompletion.Content[0].Text));
                                webView.PostMessage("AssistantMessageResponse", chatCompletion.Content[0].Text);
                            }
                            else
                            {
                                HandleChatResponse(chatCompletion, webView);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        webView.PostMessage("ConnectionFailed", null);
                        _msgService.ShowError("Connection Failed : " + ex.Message, ex.StackTrace);
                    }
                }
                else if(args.Message == "Disconnect")
                {
                    _dockingWindowService.ClosePane("Odin");
                }
            };

        }

        public ChatCompletion AddSystemMessage(string message)
        {
            conversationHistory.Add(new SystemChatMessage(message));
            chatCompletion = chatClient.CompleteChat(conversationHistory, options);
            return chatCompletion;
        }

        public ChatCompletion AddUserMessage(MessageModel userMessage)
        {
            string imagePath = "", imageMime = "";
            if (userMessage.Attachment == null)
            {
                conversationHistory.Add(ChatMessage.CreateUserMessage(userMessage.Text));
            }
            else
            {
                imagePath = Path.Join(_getCurrentApp().Root.DirectoryPath, $"{Defaults.extensionPath}\\{Defaults.imageUploadedName}");
                imageMime = ImageHandler.SaveBase64ToImage(userMessage.Attachment, imagePath);
                if (imageMime != null)
                {
                    var newMessage = new UserChatMessage(userMessage.Text);
                    var imageData = File.ReadAllBytes(imagePath);
                    newMessage.Content.Add(ChatMessageContentPart.CreateImagePart(new BinaryData(imageData), $"{imageMime}"));
                    conversationHistory.Add(newMessage);
                }
                else
                {
                    conversationHistory.Add(new UserChatMessage("Image upload failed"));
                }
            }
            chatCompletion = chatClient.CompleteChat(conversationHistory, options);
            if (imagePath != "")
            {
                File.Delete(imagePath);
            }
            return chatCompletion;
        }

        public void HandleChatResponse(ChatCompletion chatCompletion, IWebView webView)
        {
            try
            {
                if (chatCompletion.Content.Count != 0 && chatCompletion.Content[0].Text.Trim() != "")
                {
                    conversationHistory.Add(new AssistantChatMessage(chatCompletion.Content[0].Text));
                    webView.PostMessage("AssistantMessageResponse", chatCompletion.Content[0].Text);
                }
                if (chatCompletion.ToolCalls.Count != 0)
                {
                    conversationHistory.Add(new AssistantChatMessage(chatCompletion.ToolCalls));
                    foreach (ChatToolCall toolCall in chatCompletion.ToolCalls)
                    {
                        string toolResponse = "";
                        try
                        {
                            webView.PostMessage("ThinkingMessageUpdate", UpdateToolMessageForUI(toolCall));
                            int toolOutput = toolsHandler.HandleTool(toolCall, ref toolResponse);
                        }
                        catch (Exception e)
                        {
                                toolResponse = $"An error occured in the tool call {toolCall.Id} : {e.Message}";
                        }
                        conversationHistory.Add(new ToolChatMessage(toolCall.Id, toolResponse));
                    }
                    chatCompletion = chatClient.CompleteChat(conversationHistory, options);
                    HandleChatResponse(chatCompletion, webView);
                }
            }
            catch (Exception ex)
            {
                _msgService.ShowError("Connection Failed : " + ex.Message, ex.StackTrace);
                webView.PostMessage("Error", ex.Message);
            }
        }

        public string UpdateToolMessageForUI(ChatToolCall tool)
        {
            if(tool == null)
            {
                return "";
            }
            if(tool.FunctionName.Contains("get", StringComparison.OrdinalIgnoreCase)){
                return $"Fetching data from your app...";
            }
            else if (tool.FunctionName.Contains("create", StringComparison.OrdinalIgnoreCase) || tool.FunctionName.Contains("add", StringComparison.OrdinalIgnoreCase))
            {
                return $"Creating artifacts in your app...";
            }
            else if (tool.FunctionName.Contains("update", StringComparison.OrdinalIgnoreCase))
            {
                return $"Updating artifacts in your app...";
            }
            else
            {
                return $"Doing something...";
            }
        }
    }
}

