
using Bb.UIComponents;
using Microsoft.AspNetCore.Components;

namespace Bb.Adapter.Shared
{

    public partial class DynamicServerMenuComponentList
    {


        [Parameter]
        public List<DynamicServerMenu>? Menus { get; set; }


    }


}
