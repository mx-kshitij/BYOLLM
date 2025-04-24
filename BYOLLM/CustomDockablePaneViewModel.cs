using Mendix.StudioPro.ExtensionsAPI.Model;
using Mendix.StudioPro.ExtensionsAPI.Services;
using Mendix.StudioPro.ExtensionsAPI.UI.DockablePane;
using Mendix.StudioPro.ExtensionsAPI.UI.Services;
using Mendix.StudioPro.ExtensionsAPI.UI.WebView;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BYOLLM
{
    public class CustomDockablePaneViewModel : WebViewDockablePaneViewModel
    {
        private readonly Uri _baseUri;
        private readonly Func<IModel?> _getCurrentApp;
        private readonly ILogService _logService;
        private readonly IBackgroundJobService _bgService;
        private readonly IMessageBoxService _msgService;
        private readonly IDockingWindowService _dockingWindowService;
        private ChatClient? chatClient;
        private ChatCompletion? chatCompletion;
        private List<ChatMessage> conversationHistory;
        private ChatCompletionOptions options;
        private ToolsHandler toolsHandler;

        public CustomDockablePaneViewModel(Uri baseUri, Func<IModel?> getCurrentApp, ILogService logService, IBackgroundJobService bgService, IMessageBoxService msgService, IDockingWindowService dockingWindowService)
        {
            _baseUri = baseUri;
            _getCurrentApp = getCurrentApp;
            _logService = logService;
            _bgService = bgService;
            _msgService = msgService;
            _dockingWindowService = dockingWindowService;
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
                toolsHandler = new ToolsHandler(currentApp, webView);

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
                            conversationHistory.Add(new AssistantChatMessage(chatCompletion.Content[0].Text));
                            webView.PostMessage("ConnectionEstablished", null);
                            webView.PostMessage("AssistantMessageResponse", chatCompletion.Content[0].Text);
                        }
                    }
                    catch (Exception ex)
                    {
                        webView.PostMessage("ConnectionFailed", null);
                        _msgService.ShowError("Connection Failed : " + ex.Message, ex.StackTrace);
                    }
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
            if (userMessage.Attachment == null)
            {
                conversationHistory.Add(new UserChatMessage(userMessage.Text));
            }
            else
            {
                conversationHistory.Add(new UserChatMessage(userMessage.Text + "attachment: " + userMessage.Attachment));
            }
            chatCompletion = chatClient.CompleteChat(conversationHistory, options);
            return chatCompletion;
        }

        public void HandleChatResponse(ChatCompletion chatCompletion, IWebView webView)
        {
            if (chatCompletion.Content.Count != 0)
            {
                conversationHistory.Add(new AssistantChatMessage(chatCompletion.Content[0].Text));
                webView.PostMessage("AssistantMessageResponse", chatCompletion.Content[0].Text);
            }
            else if (chatCompletion.ToolCalls.Count != 0)
            {
                conversationHistory.Add(new AssistantChatMessage(chatCompletion.ToolCalls));
                foreach (ChatToolCall toolCall in chatCompletion.ToolCalls)
                {
                    string toolResponse = "";
                    int toolOutput = toolsHandler.HandleTool(toolCall, ref toolResponse);
                    conversationHistory.Add(new ToolChatMessage(toolCall.Id, toolResponse));
                }
                chatCompletion = chatClient.CompleteChat(conversationHistory, options);
                HandleChatResponse(chatCompletion, webView);
            }
        }
    }
}

