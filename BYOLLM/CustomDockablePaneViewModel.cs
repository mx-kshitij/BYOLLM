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
        }

        public override void InitWebView(IWebView webView)
        {
            webView.Address = new Uri(_baseUri, "index");

            webView.MessageReceived += (_, args) =>
            {
                var currentApp = _getCurrentApp();
                if (currentApp == null) return;

                if (args.Message == "SendNewUserMessage")
                {
                    var requestBody = args.Data.ToJsonString();
                    string userMessage = new MessageHandler().HandleNewUserMessage(requestBody);
                    conversationHistory.Add(new UserChatMessage(userMessage));
                    chatCompletion = chatClient.CompleteChat(conversationHistory);
                    conversationHistory.Add(new AssistantChatMessage(chatCompletion.Content[0].Text));
                    webView.PostMessage("AssistantMessageResponse", chatCompletion.Content[0].Text);
                }
                else if (args.Message == "InitiateConnection")
                {
                    var requestBody = args.Data.ToJsonString();
                    ConfigurationModel config = JsonSerializer.Deserialize<ConfigurationModel>(requestBody)!;
                    chatClient = new OpenAIConnectionHandler(currentApp, _logService).InitiateConnection(config);

                    try
                    {
                        conversationHistory.Add(new SystemChatMessage(config.SystemPrompt));
                        chatCompletion = chatClient.CompleteChat(conversationHistory);
                        conversationHistory.Add(new AssistantChatMessage(chatCompletion.Content[0].Text));
                        webView.PostMessage("ConnectionEstablished", null);
                        webView.PostMessage("AssistantMessageResponse", chatCompletion.Content[0].Text);
                    }
                    catch (Exception ex)
                    {
                        webView.PostMessage("ConnectionFailed", null);
                        _msgService.ShowError("Connection Failed : " + ex.Message, ex.StackTrace);
                    }
                }
            };

        }
    }
}

