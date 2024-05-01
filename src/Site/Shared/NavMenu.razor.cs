using Bb.ComponentModel.Translations;
using Bb.UIComponents;
using Bb.UserInterfaces;
using Microsoft.AspNetCore.Components;

namespace Site.Shared
{
    public partial class NavMenu
    {


        [Inject]
        private MenuService? MenuService { get; set; }

        //[Inject]
        //private ITranslateService? TranslateService { get; set; }


        protected override async Task OnInitializedAsync()
        {

            //topMenusLeft = await Map(UIKeys.Menus.TopLeftMenu);
            //topMenusRight = await Map(UIKeys.Menus.TopRightMenu);
            menusLeft = await Map(UIKeys.Menus.LeftMenu);

            await base.OnInitializedAsync();
        }

        private async Task<ServerMenu> Map(string key)
        {

            if (MenuService != null)
                return MenuService.GetMenu(key);

            return new ServerMenu(null);

        }


        private ServerMenu? topMenusLeft;
        private ServerMenu? menusLeft;
        private ServerMenu? topMenusRight;

    }
}
