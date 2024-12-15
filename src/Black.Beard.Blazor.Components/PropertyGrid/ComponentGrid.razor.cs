
using Bb.ComponentDescriptors;
using Bb.ComponentModel.Translations;
using Microsoft.AspNetCore.Components;

namespace Bb.PropertyGrid
{

    public partial class ComponentGrid
    {

        public ITranslateService TranslationService => ParentGrid.TranslationService;

        public override void OnFocusedRelay(PropertyGridView sender, ComponentFieldBase component)
        {
            base.OnFocusedRelay(sender, component);
        }

        protected void PropertyHasChanged(PropertyObjectDescriptor obj)
        {
            StateHasChanged();
        }

        private PropertyGridView CurrentPropertyGridView;

    }

}
