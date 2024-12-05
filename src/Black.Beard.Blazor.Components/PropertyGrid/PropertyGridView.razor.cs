using Bb.ComponentDescriptors;
using Bb.ComponentModel.Translations;
using Bb.Diagrams;
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
            _dynamicProperties = new Dictionary<string, Func<object>>();
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

            Descriptor = new ObjectDescriptor(
                _selectedObject,
                _selectedObject?.GetType(),
                TranslateService,
                ServiceProvider,
                StrategyName,
                null,
                PropertyFilter)
            {
                PropertyHasChanged = PropertyHasChanged_Impl,
            };

            this.Descriptor.PropertyHasChanged = this.SubPropertyHasChanged;

            try
            {
                StateHasChanged();
            }
            catch (Exception ex)
            {

            }

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



        public void AddDynamicProperty(string key, Func<object> func)
        {
            if (_dynamicProperties.ContainsKey(key))
                _dynamicProperties[key] = func;
            else
                _dynamicProperties.Add(key, func);
        }

        internal void BuildDynamicParameter(Dictionary<string, object> result)
        {
            foreach (var item in _dynamicProperties)
                result.Add(item.Key, item.Value());
        }


        #region events

        private IEventArgInterceptor<PropertyObjectDescriptorEventArgs> _interceptor;

        public void Raise(IEventArgInterceptor<PropertyObjectDescriptorEventArgs> interceptor)
        {

            if (interceptor != null)
            {
                if (_interceptor != null)
                    UnRaise();
                _interceptor = interceptor;
                this.PropertyHasChanged += _interceptor.Invoke;
            }
        }

        public void UnRaise()
        {
            if (_interceptor != null)
                this.PropertyHasChanged -= _interceptor.Invoke;
        }

        public event EventHandler<PropertyObjectDescriptorEventArgs> PropertyHasChanged;

        internal void PropertyHasChanged_Impl(PropertyObjectDescriptor obj, object instance)
        {
            PropertyHasChanged?.Invoke(this, new PropertyObjectDescriptorEventArgs(obj, instance));
        }

        private void SubPropertyHasChanged(PropertyObjectDescriptor obj, object instance)
        {
            Update();
            PropertyHasChanged_Impl(obj, instance);
        }

        #endregion events


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
        private Dictionary<string, Func<object>> _dynamicProperties;

    }

    public class PropertyObjectDescriptorEventArgs : EventArgs
    {

        public PropertyObjectDescriptorEventArgs(PropertyObjectDescriptor property, object instance)
        {
            this.Instance = instance;
            this.Property = property;
        }

        public object Instance { get; }

        public PropertyObjectDescriptor Property { get; }

    }


}
