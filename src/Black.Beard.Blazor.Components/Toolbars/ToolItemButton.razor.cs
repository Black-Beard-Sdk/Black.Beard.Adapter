using Bb.ComponentModel.Translations;
using Microsoft.AspNetCore.Components;

namespace Bb.Toolbars
{
    public partial class ToolItemButton : ComponentBase, ITranslateHost
    {

        [Parameter]
        public Tool Item { get; set; }

        [Parameter]
        public ToolBar Panel { get; set; }

        [Inject]
        public ITranslateService TranslationService { get; set; }

    }
}
