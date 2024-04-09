using Bb.ComponentModel.Translations;
using Bb.CustomComponents;
using Microsoft.AspNetCore.Components;

namespace Bb.PropertyGrid
{

    public partial class DynamicPropertyComponent
    {

        public DynamicPropertyComponent()
        {

        }
        
        [Parameter]
        public object Model { get; set; }


        [Parameter]
        public string Field { get; set; }


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


        protected override Task OnInitializedAsync()
        {
            
            this._descriptor = new ObjectDescriptor(Model, Model?.GetType(), TranslateService, ServiceProvider, (p) => p.Name == this.Field);
            if (_descriptor.Items.Count() == 0)
                throw new InvalidOperationException($"Missing {this.Field} field");
            this.property = _descriptor.Items.First();
            this.property.PropertyHasChanged = PropertyHasChanged;
            this.property.PropertyValidationHasChanged = PropertyValidationHasChanged;
            
            if (PropertyHasInitialized != null)
                PropertyHasInitialized(this.property);

            return base.OnInitializedAsync();
        }


        private PropertyObjectDescriptor? property;
        private ObjectDescriptor _descriptor;

    }

}
