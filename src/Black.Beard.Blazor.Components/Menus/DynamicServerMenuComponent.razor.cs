using Bb.ComponentModel.Translations;
using Bb.UserInterfaces;
using Microsoft.AspNetCore.Components;

namespace Bb.Menus
{

    public partial class DynamicServerMenuComponent : ITranslateHost
    {


        public DynamicServerMenuComponent()
            : base()
        {
                
        }

        [Parameter]
        public ServerMenu? Menu 
        {
            get => _menu;
            set
            {
                _menu = value;

            }
        }

        [Inject]
        public ITranslateService TranslationService { get; set; }


        private ServerMenu? _menu;

    }

}

