using Bb.ComponentDescriptors;
using Bb.ComponentModel.Accessors;
using Bb.ComponentModel.Translations;
using Bb.Diagrams;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Transactions;

namespace Bb.PropertyGrid
{

    public partial class PropertyGridView : ITranslateHost, IDisposable
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
        public ShowPolicyEnum ShowPolicy { get; set; }

        [Parameter]
        public PropertyGridView Parent { get; set; }

        [Parameter]
        public Func<object, IDtcTransaction> TransactionFactory
        {
            get => _transactionFactory ?? Parent?.TransactionFactory;
            set { _transactionFactory = value; }
        }

        internal ITransaction StartTransaction(object datas)
        {

            if (TransactionFactory != null)
                return new TransactionGrid(TransactionFactory(datas));

            if (Parent != null)
                return Parent.StartTransaction(datas);

            return new TransactionGrid(null);

        }

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

                if (!this.ServiceProvider.IsDisposed())
                {
                    try
                    {
                        StateHasChanged();
                    }
                    catch (Exception ex)
                    {

                    }
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

        public void Dispose()
        {
            _disposed = true;
        }

        public ObjectDescriptor Descriptor { get; set; }

        public static string StrategyName { get; private set; }

        bool success;
        string[] errors = { };
        MudForm form;
        private object _selectedObject;
        private Func<ComponentDescriptors.PropertyObjectDescriptor, bool> _propertyFilter = c => true;
        private Dictionary<string, Func<object>> _dynamicProperties;
        private static bool _mapperInitialized = false;
        private static object _lock = _mapperInitialized = false;
        private Func<object, IDtcTransaction> _transactionFactory;
        private bool _disposed;
    }


    public static class PropertyGridViewExtensions
    {

        public static bool IsDisposed(this IServiceProvider self)
        {

            bool disposed = true;

            if (self != null)
            {
                var pp = self.GetType().GetAccessors(MemberStrategy.Instance);
                if (pp.TryGetValue("Disposed", out AccessorItem accessor))
                    disposed = accessor.GetTypedValue<bool>(self);
                else
                    disposed = false;
            }

            return disposed;
        }

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
