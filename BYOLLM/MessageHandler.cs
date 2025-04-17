using Mendix.StudioPro.ExtensionsAPI.BackgroundJobs;
using Mendix.StudioPro.ExtensionsAPI.Model;
using Mendix.StudioPro.ExtensionsAPI.Model.Projects;
using Mendix.StudioPro.ExtensionsAPI.Services;
using Mendix.StudioPro.ExtensionsAPI.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BYOLLM
{
    public class MessageHandler
    {
        //private readonly ILogService _logService;
        //private readonly IModel _currentApp;
        //private readonly IBackgroundJobService _bgService;
        //private readonly IMessageBoxService _msgService;

        //public MessageHandler(IModel currentApp, ILogService logService, IBackgroundJobService bgService, IMessageBoxService msgService)
        //{
        //    _logService = logService;
        //    _currentApp = currentApp;
        //    _bgService = bgService;
        //    _msgService = msgService;
        //}

        public string HandleNewUserMessage(string messageJSON)
        {
            MessageModel message = JsonSerializer.Deserialize<MessageModel>(messageJSON)!;
            return message.Text;
        }
    }

}
