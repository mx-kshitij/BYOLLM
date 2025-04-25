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
        private readonly IDomainModelService _domainModelService;
        public const string PaneId = "StudioAIDev";

        [ImportingConstructor]
        public CustomDockablePaneExtension(ILogService logService, IBackgroundJobService bgService, IMessageBoxService msgService, IDockingWindowService dockingWindowService, IDomainModelService domainModelService)
        {
            _logService = logService;
            _bgService = bgService;
            _msgService = msgService;
            _dockingWindowService = dockingWindowService;
            _domainModelService = domainModelService;
        }

        public override string Id => PaneId;

        public override DockablePaneViewModelBase Open()
        {
            return new CustomDockablePaneViewModel(WebServerBaseUrl, () => CurrentApp, _logService, _bgService, _msgService, _dockingWindowService, _domainModelService) { Title = "Studio AI Dev" };
        }
    }
}
