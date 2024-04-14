using Bb.UIComponents;
using Microsoft.AspNetCore.Components;

namespace Bb.Adapter.Shared
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
