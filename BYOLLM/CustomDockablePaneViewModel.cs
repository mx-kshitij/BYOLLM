using Mendix.StudioPro.ExtensionsAPI.Model;
using Mendix.StudioPro.ExtensionsAPI.Services;
using Mendix.StudioPro.ExtensionsAPI.UI.DockablePane;
using Mendix.StudioPro.ExtensionsAPI.UI.Services;
using Mendix.StudioPro.ExtensionsAPI.UI.WebView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public CustomDockablePaneViewModel(Uri baseUri, Func<IModel?> getCurrentApp, ILogService logService, IBackgroundJobService bgService, IMessageBoxService msgService, IDockingWindowService dockingWindowService)
        {
            _baseUri = baseUri;
            _getCurrentApp = getCurrentApp;
            _logService = logService;
            _bgService = bgService;
            _msgService = msgService;
            _dockingWindowService = dockingWindowService;
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
                    new MessageHandler(currentApp, _logService, _bgService, _msgService).HandleNewUserMessage(requestBody);
                    webView.PostMessage("NewUserMessageResponse", "Message added");
                }
            };
        
        }
    }
}

