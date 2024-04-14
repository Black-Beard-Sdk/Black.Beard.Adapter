using Bb.ComponentModel.Translations;
using Bb.UIComponents;
using Microsoft.AspNetCore.Components;

namespace Bb.Adapter.Shared
{

    public partial class MainLayout
    {

        void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        [Inject]
        private UIService? UIService { get; set; }

        [Inject]
        private ITranslateService? translateService { get; set; }


        [Inject]
        private GuardMenuProvider? guardMenuProvider { get; set; }

        protected override async Task OnInitializedAsync()
        {
            
            menusLeft = new List<DynamicServerMenu>();
            var menuBuilder = new MenuConverter(translateService, guardMenuProvider);
            if (UIService != null)
            {
                var m = await UIService.GetUI(UIKeys.Menus.TopLeftMenu);
                if (m != null)
                    foreach (var m1 in m)
                        menusLeft.Add((DynamicServerMenu)menuBuilder.Convert(m1));
            }

            menusRight = new List<DynamicServerMenu>();
            menuBuilder = new MenuConverter(translateService, guardMenuProvider);
            if (UIService != null)
            {
                var m = await UIService.GetUI(UIKeys.Menus.TopRightMenu);
                if (m != null)
                    foreach (var m1 in m)
                        menusRight.Add((DynamicServerMenu)menuBuilder.Convert(m1));

            }
        }

        bool _drawerOpen = false;
        private List<DynamicServerMenu>? menusLeft;
        private List<DynamicServerMenu>? menusRight;

           

    }


}
