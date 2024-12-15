using Bb.Commands;
using Bb.ComponentDescriptors;
using Bb.ComponentModel.Translations;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Transactions;

namespace Bb.PropertyGrid
{

    public partial class DynamicPropertyComponent : ComponentBase
    {

        public DynamicPropertyComponent()
        {

        }

        [Parameter]
        public PropertyGridView Parent { get; set; }

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

        [Parameter]
        public Variant CurrentVariant { get; set; } = Variant.Outlined;

        [Parameter]
        public Margin CurrentMargin { get; set; } = Margin.Dense;

        [Parameter]
        public Descriptor Property { get; set; }

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

        //[Parameter]
        //public ITransactionManager TransactionManager { get; set; }

        public IDictionary<string, object> Parameters
        {
            get
            {
                
                var result = new Dictionary<string, object>()
                {
                    { "CurrentVariant", CurrentVariant },
                    { "CurrentMargin", CurrentMargin },
                    { "Descriptor", Property },
                    //{ "TransactionManager", TransactionManager },
                };

                Parent?.BuildDynamicParameter(result);

                return result;

            }
        }


        private object _model;
        private DynamicComponent Ui;

    }

}
