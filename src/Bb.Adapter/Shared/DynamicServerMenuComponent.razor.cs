using Bb.UIComponents;
using Microsoft.AspNetCore.Components;

namespace Bb.Adapter.Shared
{

    public partial class DynamicServerMenuComponent
    {


        [Parameter]
        public DynamicServerMenu? Menu { get; set; }


        [Parameter]
        public bool Enabled { get; set; }

    }

}
