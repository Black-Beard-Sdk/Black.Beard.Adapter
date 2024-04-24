
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
        public ServerMenu Menus { get; set; }  

    }

}
