using Bb.ComponentDescriptors;
using Bb.ComponentModel.Translations;
using Bb.Diagrams;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Transactions;

namespace Bb.PropertyGrid
{

    public partial class PropertyGridView : ITranslateHost
    {

        static PropertyGridView()
        {

            StrategyName = typeof(PropertyGridView).Name;

        }

        public PropertyGridView()
        {
            _dynamicProperties = new Dictionary<string, Func<object>>();
        }

        [Parameter]
        public Func<object, IDtcTransaction> TransactionFactory { get; set; }

        [Parameter]
        public Action<PropertyGridView, ComponentFieldBase>? Focused { get; set; }

        [Inject]
        public ITranslateService TranslationService { get; set; }

        [Inject]
        public IServiceProvider ServiceProvider { get; set; }

        [Parameter]
        public Func<ComponentDescriptors.PropertyObjectDescriptor, bool> PropertyFilter
        {
            get => _propertyFilter;
            set => _propertyFilter = value;
        }

        [Parameter]
        public Action<ComponentDescriptors.PropertyObjectDescriptor> AfterPropertyHaschanged { get; set; }

        [Parameter]
        public Variant CurrentVariant { get; set; } = Variant.Text;

        [Parameter]
        public Margin CurrentMargin { get; set; } = Margin.None;

        private void Update()
        {

            if (!_mapperInitialized)
                lock (_lock)
                    if (!_mapperInitialized)
                    {
                        Initialize(StrategyMapper.Get(StrategyName));
                        _mapperInitialized = true;
                    }

            if (_selectedObject != null)
            {

                Descriptor = new ObjectDescriptor
                (
                    _selectedObject,
                    _selectedObject?.GetType(),
                    this,
                    ServiceProvider,
                    StrategyName,
                    null,
                    PropertyFilter
                )
                {
                    PropertyHasChanged = PropertyHasChanged_Impl,
                };

                Descriptor.SetUI(this);

                this.Descriptor.PropertyHasChanged = this.SubPropertyHasChanged;

                try
                {
                    StateHasChanged();
                }
                catch (Exception ex)
                {

                }

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

                if (_selectedObject != value)
                {
                    if (_selectedObject is INotifyPropertyChanged old1)
                        old1.PropertyChanged -= PropertyChanged;

                    if (_selectedObject is INotifyCollectionChanged old2)
                        old2.CollectionChanged -= CollectionChanged;
                }

                _selectedObject = value;
                Update();

                if (_selectedObject is INotifyPropertyChanged old3)
                    old3.PropertyChanged += PropertyChanged;

                if (_selectedObject is INotifyCollectionChanged old4)
                    old4.CollectionChanged += CollectionChanged;
            }

        }

        private void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                StateHasChanged();
            }
            catch (Exception ex)
            {

            }
        }

        private void PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            try
            {
                StateHasChanged();
            }
            catch (Exception ex)
            {

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

        internal void BuildDynamicParameter(PropertyGridView parentView)
        {
            foreach (var item in parentView._dynamicProperties)
                _dynamicProperties.Add(item.Key, item.Value);
        }

        #region events

        internal virtual void SetFocus(ComponentFieldBase componentFieldBase)
        {
            if (Focused != null)
            {
                Focused.Invoke(this, componentFieldBase);
            }
            else
            {

            }
        }

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

        internal void PropertyHasChanged_Impl(ComponentDescriptors.PropertyObjectDescriptor obj, object instance)
        {
            PropertyHasChanged?.Invoke(this, new PropertyObjectDescriptorEventArgs(obj, instance));
        }

        private void SubPropertyHasChanged(ComponentDescriptors.PropertyObjectDescriptor obj, object instance)
        {
            Update();
            PropertyHasChanged_Impl(obj, instance);
            AfterPropertyHaschanged?.Invoke(obj);
        }

        private IEventArgInterceptor<PropertyObjectDescriptorEventArgs> _interceptor;

        public event EventHandler<PropertyObjectDescriptorEventArgs> PropertyHasChanged;

        #endregion events


        public DiagnosticValidator Validate()
        {
            return Descriptor.Validate();
        }

        public ObjectDescriptor Descriptor { get; set; }

        public static string StrategyName { get; private set; }


        internal ITransaction StartTransaction(object datas) => new TransactionGrid(TransactionFactory(datas));
        
        bool success;
        string[] errors = { };
        MudForm form;
        private object _selectedObject;
        private Func<ComponentDescriptors.PropertyObjectDescriptor, bool> _propertyFilter = c => true;
        private Dictionary<string, Func<object>> _dynamicProperties;
        private static bool _mapperInitialized = false;
        private static object _lock = _mapperInitialized = false;

    }

    public class PropertyObjectDescriptorEventArgs : EventArgs
    {

        public PropertyObjectDescriptorEventArgs(ComponentDescriptors.PropertyObjectDescriptor property, object instance)
        {
            this.Instance = instance;
            this.Property = property;
        }

        public object Instance { get; }

        public ComponentDescriptors.PropertyObjectDescriptor Property { get; }

    }


}
