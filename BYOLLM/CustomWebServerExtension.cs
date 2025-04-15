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
            //webServer.AddRoute("newusermsg", NewUserMessage);
            //webServer.AddRoute("connect", InitiateConnection);
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
        //private async Task NewUserMessage(HttpListenerRequest request, HttpListenerResponse response, CancellationToken ct)
        //{
        //    if (CurrentApp == null)
        //    {
        //        response.SendNoBodyAndClose(404);
        //        return;
        //    }

        //    if (!request.HasEntityBody)
        //    {
        //        response.SendNoBodyAndClose(400);
        //        return;
        //    }

        //    System.IO.Stream body = request.InputStream;
        //    System.Text.Encoding encoding = request.ContentEncoding;
        //    System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);

        //    string requestBody = "";

        //    try
        //    {
        //        requestBody = reader.ReadToEnd();
        //        var jsonStream = new MemoryStream();
        //        await JsonSerializer.SerializeAsync(jsonStream, "Added new message", cancellationToken: ct);

        //        response.SendJsonAndClose(jsonStream);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logService.Error("Error parsing ", ex);
        //    }
        //    try
        //    {
        //        new MessageHandler(CurrentApp, _logService, _bgService, _msgService).HandleNewUserMessage(requestBody);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logService.Error("Failed to add new message", ex);
        //    }


        //    body.Close();
        //    reader.Close();
        //}

        //private async Task InitiateConnection(HttpListenerRequest request, HttpListenerResponse response, CancellationToken ct)
        //{
        //    if (CurrentApp == null)
        //    {
        //        response.SendNoBodyAndClose(404);
        //        return;
        //    }

        //    if (!request.HasEntityBody)
        //    {
        //        response.SendNoBodyAndClose(400);
        //        return;
        //    }

        //    System.IO.Stream body = request.InputStream;
        //    System.Text.Encoding encoding = request.ContentEncoding;
        //    System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);

        //    string requestBody = "";

        //    try
        //    {
        //        requestBody = reader.ReadToEnd();
        //        var jsonStream = new MemoryStream();
        //        await JsonSerializer.SerializeAsync(jsonStream, "Initiating connection", cancellationToken: ct);

        //        response.SendJsonAndClose(jsonStream);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logService.Error("Error parsing ", ex);
        //    }
        //    try
        //    {
        //        // implement here
        //    }
        //    catch (Exception ex)
        //    {
        //        _logService.Error("Failed to connect", ex);
        //    }


        //    body.Close();
        //    reader.Close();
        //}
    }
}
