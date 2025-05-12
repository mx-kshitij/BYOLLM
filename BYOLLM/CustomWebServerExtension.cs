using Mendix.StudioPro.ExtensionsAPI.Services;
using Mendix.StudioPro.ExtensionsAPI.UI.Services;
using Mendix.StudioPro.ExtensionsAPI.UI.WebServer;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BYOLLM
{

    [Export(typeof(WebServerExtension))]
    public class CustomWebServerExtension : WebServerExtension
    {
        private readonly IExtensionFileService _extensionFileService;
        private readonly ILogService _logService;
        private readonly IBackgroundJobService _bgService;
        private readonly IMessageBoxService _msgService;

        [ImportingConstructor]
        public CustomWebServerExtension(IExtensionFileService extensionFileService, ILogService logService, IBackgroundJobService bgService, IMessageBoxService msgService)
        {
            _extensionFileService = extensionFileService;
            _logService = logService;
            _bgService = bgService;
            _msgService = msgService;
        }

        public override void InitializeWebServer(IWebServer webServer)
        {
            webServer.AddRoute("index", ServeIndex);
            webServer.AddRoute("main.js", ServeMainJs);
            webServer.AddRoute("theme", ServeTheme);
            webServer.AddRoute("getConfiguration", ServeConfiguration);
            webServer.AddRoute("currentLanguage", ServeCurrentLanguage);
        }

        private async Task ServeIndex(HttpListenerRequest request, HttpListenerResponse response, CancellationToken ct)
        {
            var indexFilePath = _extensionFileService.ResolvePath("wwwroot", "index.html");
            await response.SendFileAndClose("text/html", indexFilePath, ct);
        }

        private async Task ServeMainJs(HttpListenerRequest request, HttpListenerResponse response, CancellationToken ct)
        {
            var indexFilePath = _extensionFileService.ResolvePath("wwwroot", "main.js");
            await response.SendFileAndClose("text/javascript", indexFilePath, ct);
        }

        private async Task ServeTheme(HttpListenerRequest request, HttpListenerResponse response, CancellationToken ct)
        {
            var theme = Configuration.Theme.ToString();
            var jsonStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(jsonStream, theme, cancellationToken: ct);
            response.SendJsonAndClose(jsonStream);
        }

        private async Task ServeCurrentLanguage(HttpListenerRequest request, HttpListenerResponse response, CancellationToken ct)
        {
            var currentLanguage = Configuration.CurrentLanguage;
            var jsonStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(jsonStream, currentLanguage, cancellationToken: ct);
            response.SendJsonAndClose(jsonStream);
        }

        private async Task ServeConfiguration(HttpListenerRequest request, HttpListenerResponse response, CancellationToken ct)
        {
            if (CurrentApp == null)
            {
                response.SendNoBodyAndClose(404);
                return;
            }

            ConfigurationModel configuration = new ConfigurationStorage(CurrentApp, _logService).LoadConfiguration();
            var jsonStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(jsonStream, configuration, cancellationToken: ct);

            response.SendJsonAndClose(jsonStream);
        }
    }
}
