
using Bb.UserInterfaces;
using Microsoft.AspNetCore.Components;

namespace Bb.Menus
{
    public partial class MenuNav
    {

        public MenuNav()
        {
            
        }

        [Parameter]
        public List<DynamicServerMenu> Menus { get; set; }  

    }

}
