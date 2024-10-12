using Bb.ComponentModel.Translations;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Bb.Toolbars
{
    public class ClickableMudToggleIconButton : MudToggleIconButton
    {


        public ClickableMudToggleIconButton()
        {
            ToggledChanged = new EventCallback<bool>(this, ButtonToggledChanged);
        }

        [Parameter]
        public Tool Item { get; set; }

        [Parameter]
        public ToolBar Panel { get; set; }

        [Inject]
        public ITranslateService TranslationService { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            //if (ToolboxList.CurrentLink == Instance)
            //    Toggled = true;
        }

        public void ButtonToggledChanged(bool toggled)
        {
            if (toggled)
                Panel.CurrentClicked = this.Item;
            else
                Panel.CurrentClicked = null;
        }

    }


}
