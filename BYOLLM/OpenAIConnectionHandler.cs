using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Mendix.StudioPro.ExtensionsAPI.Model;
using Mendix.StudioPro.ExtensionsAPI.Services;
using OpenAI.Chat;

namespace Odin
{
    public class OpenAIConnectionHandler
    {
        private readonly ILogService _logService;
        private readonly IModel _currentApp;

        public OpenAIConnectionHandler(IModel currentApp, ILogService logService)
        {
            _logService = logService;
            _currentApp = currentApp;
        }
        public ChatClient InitiateConnection(ConfigurationModel config)
        {
            AzureOpenAIClient openAIClient;
            if (config.UseEntraId)
            {
                if (OperatingSystem.IsMacOS())
                {
                    openAIClient = new AzureOpenAIClient(new Uri(config.Endpoint), new DefaultAzureCredential(includeInteractiveCredentials: true));
                }
                else
                {
                    openAIClient = new AzureOpenAIClient(new Uri(config.Endpoint), new DefaultAzureCredential());
                }
            }
            else
            {
                openAIClient = new AzureOpenAIClient(new Uri(config.Endpoint), new AzureKeyCredential(config.Apikey));
            }

            ChatClient chatClient = openAIClient.GetChatClient(config.Deployment);

            new ConfigurationStorage(_currentApp, _logService).SaveConfiguration(config);
            return chatClient;
        }
    }
}
