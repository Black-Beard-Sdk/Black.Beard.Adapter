
using Bb.UserInterfaces;
using Microsoft.AspNetCore.Components;

namespace Bb.Menus
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

