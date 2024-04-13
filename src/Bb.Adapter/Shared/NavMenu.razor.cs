using Bb.ComponentModel.Translations;
using Bb.UIComponents;
using Microsoft.AspNetCore.Components;

namespace Bb.Adapter.Shared
{

    public partial class NavMenu
    {

        [Inject]
        private UIService? uIService { get; set; }

        [Inject]
        private ITranslateService? translateService { get; set; }

        [Inject]
        private GuardMenuProvider? GuardMenuProvider { get; set; }

        public List<DynamicServerMenu>? Menus { get; set; }

        protected override async Task OnInitializedAsync()
        {

            Menus = new List<DynamicServerMenu>();
            var menuBuilder = new MenuConverter(translateService, GuardMenuProvider);
            if (uIService != null)
            {
                var m = await uIService.GetUI(UIService.LeftMenu);
                if (m != null)
                    foreach (var m1 in m)
                        Menus.Add((DynamicServerMenu)menuBuilder.Convert(m1));

            }

        }

    }

}
