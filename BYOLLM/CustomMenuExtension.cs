using Mendix.StudioPro.ExtensionsAPI.UI.Menu;
using Mendix.StudioPro.ExtensionsAPI.UI.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BYOLLM
{
    [Export(typeof(Mendix.StudioPro.ExtensionsAPI.UI.Menu.MenuExtension))]
    public class CustomMenuExtension : MenuExtension
    {
        private readonly IDockingWindowService _dockingWindowService;

        [ImportingConstructor]
        public CustomMenuExtension(IDockingWindowService dockingWindowService)
        {
            _dockingWindowService = dockingWindowService;
        }

        public override IEnumerable<MenuViewModel> GetMenus()
        {
            yield return new MenuViewModel("Studio AI Dev", () => _dockingWindowService.OpenPane(CustomDockablePaneExtension.PaneId));
        }
    }
}
