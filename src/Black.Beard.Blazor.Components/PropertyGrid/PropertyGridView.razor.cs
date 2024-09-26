using Bb.ComponentDescriptors;
using Bb.ComponentModel.Translations;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Bb.PropertyGrid
{

    public partial class PropertyGridView
    {

        static PropertyGridView()
        {

            StrategyName = typeof(PropertyGridView).Name;

        }

        private static bool _mapperInitialized = false;
        private static object _lock = _mapperInitialized = false;

        public PropertyGridView()
        {

        }

        [Inject]
        public ITranslateService TranslateService { get; set; }

        [Inject]
        public IServiceProvider ServiceProvider { get; set; }

        [Parameter]
        public Func<PropertyObjectDescriptor, bool> PropertyFilter
        {
            get => _propertyFilter;
            set => _propertyFilter = value;
        }


        [Parameter]
        public Action<PropertyObjectDescriptor> PropertyHasChanged { get; set; }

        [Parameter]
        public Variant CurrentVariant { get; set; } = Variant.Text;

        [Parameter]
        public Margin CurrentMargin { get; set; } = Margin.Dense;

        private void Update()
        {

            if (!_mapperInitialized)
                lock (_lock)
                    if (!_mapperInitialized)
                    {
                        Initialize(StrategyMapper.Get(StrategyName));
                        _mapperInitialized = true;
                    }

            Descriptor = new ObjectDescriptor(_selectedObject, _selectedObject?.GetType(), TranslateService, ServiceProvider, StrategyName, null, PropertyFilter)
            {
                PropertyHasChanged = this.PropertyHasChanged,
            };

            this.Descriptor.PropertyHasChanged = this.SubPropertyHasChanged;
            StateHasChanged();

        }

        [Parameter]
        public bool WithGroup { get; set; }

        public void Refresh()
        {
           Update();
        }

        [Parameter]
        public object SelectedObject
        {
            get => _selectedObject;
            set
            {
                _selectedObject = value;
                Update();
            }
        }

        private void SubPropertyHasChanged(PropertyObjectDescriptor obj)
        {
            Update();
            //StateHasChanged();
            if (PropertyHasChanged != null)
                PropertyHasChanged(obj);
        }

        public DiagnosticValidator Validate()
        {
            return Descriptor.Validate();
        }

        public ObjectDescriptor Descriptor { get; set; }

        public static string StrategyName { get; private set; }


        bool success;
        string[] errors = { };
        MudForm form;
        private object _selectedObject;
        private Func<PropertyObjectDescriptor, bool> _propertyFilter = c => true;

    }



}
