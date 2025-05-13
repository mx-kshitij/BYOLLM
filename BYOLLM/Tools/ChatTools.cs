using Mendix.StudioPro.ExtensionsAPI.UI.WebView;

namespace Odin
{
    public class ChatTools
    {
               
        public static void SendMessage(IWebView webView, string message)
        {
            webView.PostMessage("AssistantMessageResponse", message);
        }
    }
}
