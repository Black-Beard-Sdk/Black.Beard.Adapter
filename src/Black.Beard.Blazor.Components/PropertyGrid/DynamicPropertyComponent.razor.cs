using Bb.ComponentModel.Translations;
using Bb.CustomComponents;
using Microsoft.AspNetCore.Components;
using MudBlazor;

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


        public IDictionary<string, object> Parameters 
        {
            get
            {
                var result = new Dictionary<string, object>()
                {
                    { "CurrentVariant", CurrentVariant },
                    { "CurrentMargin", CurrentMargin },
                    { "Property", Property },
                };
       
                return result;

            }
        }


        private DynamicComponent Ui;

        public object _model { get; set; }

        [Parameter]
        public Variant CurrentVariant { get; set; } = Variant.Outlined;

        [Parameter]
        public Margin CurrentMargin { get; set; } = Margin.Dense;

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
