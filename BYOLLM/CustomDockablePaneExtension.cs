using Mendix.StudioPro.ExtensionsAPI.Services;
using Mendix.StudioPro.ExtensionsAPI.UI.DockablePane;
using Mendix.StudioPro.ExtensionsAPI.UI.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BYOLLM
{
    [Export(typeof(DockablePaneExtension))]
    public class CustomDockablePaneExtension : DockablePaneExtension
    {
        private readonly ILogService _logService;
        private readonly IBackgroundJobService _bgService;
        private readonly IMessageBoxService _msgService;
        private readonly IDockingWindowService _dockingWindowService;
        public const string PaneId = "StudioAIDev";

        [ImportingConstructor]
        public CustomDockablePaneExtension(ILogService logService, IBackgroundJobService bgService, IMessageBoxService msgService, IDockingWindowService dockingWindowService)
        {
            _logService = logService;
            _bgService = bgService;
            _msgService = msgService;
            _dockingWindowService = dockingWindowService;
        }

        public override string Id => PaneId;

        public override DockablePaneViewModelBase Open()
        {
            return new CustomDockablePaneViewModel(WebServerBaseUrl, () => CurrentApp, _logService, _bgService, _msgService, _dockingWindowService) { Title = "Studio AI Dev" };
        }
    }
}
