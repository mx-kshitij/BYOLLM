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
        public MessageModel HandleNewUserMessage(string messageJSON)
        {
            MessageModel message = JsonSerializer.Deserialize<MessageModel>(messageJSON)!;
            return message;
        }
    }

}
