
using Bb.ComponentModel.Translations;
using Bb.UserInterfaces;
using Microsoft.AspNetCore.Components;

namespace Bb.Menus
{

    public partial class MenuNav : ITranslateHost
    {

        public MenuNav()
        {
            
        }

        [Parameter]
        public ServerMenu Menus { get; set; }

        [Inject]
        public ITranslateService TranslationService { get; set; }


    }

}
