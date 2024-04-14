using Bb.UIComponents;
using Microsoft.AspNetCore.Components;

namespace Bb.Adapter.Shared
{

    public partial class DynamicServerMenuComponent
    {


        public DynamicServerMenuComponent()
            : base()
        {
                
        }

        [Parameter]
        public DynamicServerMenu? Menu 
        {
            get => _menu;
            set
            {
                _menu = value;

            }
        }

        private DynamicServerMenu? _menu;

    }

}

