using Azure;
using Mendix.StudioPro.ExtensionsAPI.Model;
using Mendix.StudioPro.ExtensionsAPI.Model.Projects;
using Mendix.StudioPro.ExtensionsAPI.UI.WebView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BYOLLM
{
    public class ChatTools
    {
               
        public static void SendMessage(IWebView webView, string message)
        {
            webView.PostMessage("AssistantMessageResponse", message);
        }
    }
}
