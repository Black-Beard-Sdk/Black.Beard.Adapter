
using Bb.ComponentModel.Accessors;
using Bb.ComponentModel.Translations;
using Bb.CustomComponents;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.ComponentModel;

namespace Bb.PropertyGrid
{

    public partial class PropertyGridView
    {

        public PropertyGridView()
        {
            PropertyFilter = c => true;
        }

        [Inject]
        public ITranslateService TranslateService { get; set; }

        [Inject]
        public IServiceProvider ServiceProvider { get; set; }

        [Parameter]
        public Func<PropertyDescriptor, bool> PropertyFilter { get; set; }

        [Parameter]
        public Action<PropertyObjectDescriptor> PropertyHasChanged { get; set; }

        protected override Task OnInitializedAsync()
        {
            Update();
            return base.OnInitializedAsync();
        }

        private void Update()
        {
            Descriptor = new ObjectDescriptor(_selectedObject, _selectedObject?.GetType(), TranslateService, ServiceProvider, PropertyFilter)
            {
                PropertyHasChanged = this.PropertyHasChanged,
            };
            this.Descriptor.PropertyHasChanged = this.SubPropertyHasChanged;
            StateHasChanged();
        }

        [Parameter]
        public bool WithGroup { get; set; }

        [Parameter]
        public object SelectedObject
        {
            get => _selectedObject;
            set
            {
                if (value != null)
                {
                    _selectedObject = value;
                }
                Update();
                StateHasChanged();
            }
        }

        private void SubPropertyHasChanged(PropertyObjectDescriptor obj)
        {
            StateHasChanged();
            if (PropertyHasChanged != null)
                PropertyHasChanged(obj);
        }

        public DiagnosticValidator Validate()
        {
            return Descriptor.Validate();
        }

        public ObjectDescriptor Descriptor { get; set; }


        bool success;
        string[] errors = { };
        MudForm form;
        private object _selectedObject;

    }



}
