using Bb.Adapter.Services;
using Bb.ComponentModel.Translations;
using Bb.UIComponents;
using Bb.UIComponents.Guards;
using Bb.UserInterfaces;
using Microsoft.AspNetCore.Components;
using static Bb.UIComponents.UIKeys;

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
        private ITranslateService? TranslateService { get; set; }


        [Inject]
        private GuardMenuProvider? GuardMenuProvider { get; set; }

        [Inject]
        private IServiceProvider? ServiceProvider { get; set; }


        protected override async Task OnInitializedAsync()
        {
            topMenusLeft = await Map(UIKeys.Menus.TopLeftMenu);
            topMenusRight = await Map(UIKeys.Menus.TopRightMenu);
            menusLeft = await Map(UIKeys.Menus.LeftMenu);
        }

        private async Task<List<DynamicServerMenu>> Map(string key)
        {

            var menu = new List<DynamicServerMenu>();
            var menuBuilder = new MenuConverter(TranslateService, GuardMenuProvider, ServiceProvider);
            if (UIService != null)
            {
                var m = await UIService.GetUI(key);
                if (m != null)
                    foreach (var m1 in m)
                    {
                        var menu1 = (DynamicServerMenu)menuBuilder.Convert(m1);
                        menu.Add(menu1);

                    }
            }

            return menu;

        }


        bool _drawerOpen = false;
        private List<DynamicServerMenu>? topMenusLeft;
        private List<DynamicServerMenu>? menusLeft;
        private List<DynamicServerMenu>? topMenusRight;

           

    }


}
