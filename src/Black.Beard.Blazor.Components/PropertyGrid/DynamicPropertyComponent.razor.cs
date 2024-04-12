using Bb.ComponentModel.Translations;
using Bb.CustomComponents;
using Microsoft.AspNetCore.Components;

namespace Bb.PropertyGrid
{

    public partial class DynamicPropertyComponent : ComponentBase
    {

        public DynamicPropertyComponent()
        {

        }        

        [Parameter]
        public object Model 
        {
            get => _model;
            set
            {
                _model = value;
                var o = this;
                StateHasChanged();
            }
        }

        private DynamicComponent Ui;

        public object _model { get; set; }


        [Parameter]
        public PropertyObjectDescriptor Property { get; set; }

        [Parameter]
        public Action<PropertyObjectDescriptor> PropertyValidationHasChanged { get; set; }

        [Parameter]
        public Action<PropertyObjectDescriptor> PropertyHasChanged { get; set; }

        [Parameter]
        public Action<PropertyObjectDescriptor> PropertyHasInitialized { get; set; }

        [Inject]
        public ITranslateService TranslateService { get; set; }

        [Inject]
        public IServiceProvider ServiceProvider { get; set; }

    }

}
