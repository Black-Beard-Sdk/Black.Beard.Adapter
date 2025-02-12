using Bb.ComponentModel.Translations;
using Bb.TypeDescriptors;
using System.Data;
using System.Reflection;
using System.Collections;

namespace Bb.ComponentDescriptors
{


    public class Descriptor : ITranslateHost
    {

        static Descriptor()
        {
            Descriptor._listKeyValue = new ListKeyLabels();
        }


        public Descriptor(
            IServiceProvider serviceProvider,
            ITranslateHost hostTranslateService,
            string strategyKey,
            Type? type,
            Func<System.ComponentModel.PropertyDescriptor, bool> propertyDescriptorFilter,
            Func<PropertyObjectDescriptor, bool> propertyFilter
            )
        {

            Resolvers = new List<IPropertyDescriptorTypeResolver>();

            _items = new List<Descriptor>();
            _strategy = string.IsNullOrEmpty(strategyKey)
                ? StrategyMapper.Get(string.Empty)
                : StrategyMapper.Get(strategyKey);

            ServiceProvider = serviceProvider;
            TranslationService = hostTranslateService.TranslationService;

            StrategyName = _strategy.Key;

            if (propertyDescriptorFilter != null)
                PropertyDescriptorFilter = propertyDescriptorFilter;

            else
                PropertyDescriptorFilter = (p) =>
                {
                    if (p is IDynamicActiveProperty i)
                        return i.IsActive(Value);
                    return true;
                };

            if (propertyFilter != null)
                PropertyFilter = propertyFilter;
            else
                PropertyFilter = (p) => true;
            
            if(type != null)
                SetType(type);

        }


        protected void SetType(Type type)
        {
        
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            this.Type = type;

            this.Nullable = type.IsClass;
            IsStapleType = type.IsStapleType();

            if (type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type))
                this._listAccessor = new ListAccessor(type);

            if (!_types2.Contains(type) && CanBeCreated(type))
                Descriptor._listKeyValue.Add(type);

            bool isNullable = false;
            Type? sub = null;

            if (ResolveSubType(Type, ref sub, ref isNullable))
                SubType = sub;

            Nullable = isNullable;

        }

        public Descriptor CreateSub(object instance, Type type = null)
        {
            return new SubObjectDescriptor(instance, type ?? instance.GetType(), this)
            {
                Enabled = true,
            };
        }

        public string GetValueKey(object parent, object child)
        {

            if (parent is IDictionary d)
                foreach (dynamic item in d)
                    if (item.Value == child)
                        return item.Key;

            if (Descriptor._listKeyValue.TryGetKey(child, out var result))
                return result(child)?.ToString();


            if (parent is IEnumerable l)
            {
                int i = 0;
                foreach (dynamic item in l)
                {
                    if (item == child)
                        return i.ToString();
                    i++;
                }
            }

            return default;

        }

        public string GetValueLabel(object item, string defaultValue)
        {

            if (item == null)
                return string.Empty;

            if (Descriptor._listKeyValue.TryGetLabel(item, out var result))
                return result(item);

            if (IsStapleType)
                return item.ToString();

            return defaultValue;

        }


        protected virtual void AssignStrategy(StrategyEditor strategy)
        {

            foreach (var item in strategy.Initializers)
                item(Type, _strategy, this);

        }

        protected virtual void Analyze()
        {          

            if (_strategy.TryGetValueByType(Type, out StrategyEditor strategyEditor))
            {
                IsEnumerable = strategyEditor.IsEnumerable;
                ComponentView = strategyEditor.ComponentView;
                if (ComponentView == null)
                {

                }

                KindView = strategyEditor.PropertyKindView;
                strategyEditor.Initializer?.Invoke(_strategy, this);
                AssignStrategy(strategyEditor);
            }

        }


        #region Type analyze

        internal protected static bool ResolveSubType(Type type, ref Type? subType, ref bool isNullable)
        {

            subType = null;
            isNullable = false;            

            if (_types.Contains(type)) { }

            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                isNullable = true;
                subType = type.GetGenericArguments()[0];
            }

            else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
            {
                var interfaces = type.GetInterfaces();
                foreach (var item in interfaces)
                    if (item.IsGenericType && item.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    {
                        subType = item.GetGenericArguments()[0];
                        //if (!CanBeCreated(subType))
                        //    isBrowsable = false;
                        break;
                    }
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    subType = type.GetGenericArguments()[0];
                    //if (!CanBeCreated(subType))
                    //    isBrowsable = false;
                }
            }

            else if (type.IsArray)
            {
                subType = type.GetElementType();
                // isArray = true;
            }

            return subType != default;

        }

        #endregion Type analyze

        public bool CanBeCreated()
        {
            return CanBeCreated(Type);
        }

        public static bool CanBeCreated(Type type)
        {

            if (type == null)
                return false;

            if (type == typeof(void))
                return false;

            if (type.IsAbstract)
                return false;

            if (type.IsInterface)
                return false;

            if (type.IsEnum)
                return false;

            if (_types.Contains(type))
                return true;

            if (type.GetConstructor([]) == null)
                return false;

            return true;

        }

        #region Validation

        /// <summary>
        /// Validate the property in the instance
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public virtual bool Validate(out DiagnosticValidatorItem result)
        {
            result = new DiagnosticValidatorItem();
            return true;
        }

        /// <summary>
        /// Return true if Validation of the property in the instance is valid
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public virtual bool Validate(object value, out DiagnosticValidatorItem result)
        {
            result = new DiagnosticValidatorItem();
            return true;
        }

        public DiagnosticValidator Validate()
        {

            var validator = new DiagnosticValidator();

            foreach (var item in _items)
                if (item.Enabled && !item.Validate(out var result))
                    validator.Add(result);

            return validator;

        }

        public void ValidationChanged<T>(IComponentFieldBase<T> componentFieldBase)
        {

            UiPropertyValidationHasChanged?.Invoke(componentFieldBase);

            ValidationHasChanged?.Invoke(componentFieldBase.Descriptor);

        }

        public Action<IComponentFieldBase> UiPropertyValidationHasChanged { get; set; }

        public Action<Descriptor> ValidationHasChanged { get; set; }

        #endregion Validation


        protected void Add(Descriptor descriptor)
        {
            descriptor.Parent = this;
            this._items.Add(descriptor);

        }

        public virtual void SetUI(object ui)
        {
            Ui = ui;
            foreach (var item in _items)
                item.SetUI(ui);
        }

        internal void HasChanged(PropertyObjectDescriptor propertyObjectDescriptor)
        {
            PropertyHasChanged?.Invoke(propertyObjectDescriptor, Value);
        }

        public override string ToString()
        {
            return Type.Name;
        }

        /// <summary>
        /// Return the translated display
        /// </summary>
        /// <returns></returns>
        public virtual string GetDisplay()
        {
            TranslatedKeyLabel r = Display ?? string.Empty;
            if (TranslationService != null)
                return TranslationService.Translate(r);
            return r;
        }

        /// <summary>
        /// Return the translated description
        /// </summary>
        /// <returns></returns>
        public virtual string GetDescription()
        {
            TranslatedKeyLabel r = Description ?? string.Empty;
            if (TranslationService != null)
                return TranslationService.Translate(r);
            return r;
        }


        public int ChildCount => _items.Count;

        public Descriptor RootParent => Parent == null ? this : Parent.RootParent;

        public Descriptor Parent { get; protected set; }

        internal void SetParent(Descriptor parent)
        {
            Parent = parent;
        }


        public string Kind => GetType().Name; 

        /// <summary>
        /// Validation error text
        /// </summary>
        public string ErrorText { get; set; }

        /// <summary>
        /// Return true if the validation is failed
        /// </summary>
        public bool InError { get; set; }

        public bool Nullable { get; set; }

        public bool IsStapleType { get; private set; }

        //public static bool IsStapleType(Type type)
        //{
        //    return _types.Contains(type);
        //}


        public ITranslateService TranslationService { get; }

        public IServiceProvider ServiceProvider { get; }

        public virtual object Value { get; set; }

        public bool IsList { get; private set; }

        public bool IsEnumerable { get; private set; }


        public virtual TranslatedKeyLabel Display { get; internal protected set; }

        public virtual TranslatedKeyLabel Description { get; internal protected set; }

        public bool Browsable { get; internal protected set; }

        public bool Enabled { get; internal set; }

        public string KindView { get; set; }

        public object Ui { get; protected set; }

        public Func<System.ComponentModel.PropertyDescriptor, bool> PropertyDescriptorFilter { get; }

        public Func<PropertyObjectDescriptor, bool> PropertyFilter { get; }

        public IEnumerable<Descriptor> Items { get => _items; }


        /// <summary>
        /// Global strategy name
        /// </summary>
        public string StrategyName { get; }

        public Type ComponentView { get; set; }

        public Type SubType { get; set; }

        public Type Type { get; private set; }

        public List<IPropertyDescriptorTypeResolver> Resolvers { get; protected set; }

        public Action<PropertyObjectDescriptor, object> PropertyHasChanged { get; set; }
        public ListAccessor ListAccessor => _listAccessor;
        

        protected readonly StrategyMapper _strategy;


        private readonly List<Descriptor> _items;
        private readonly string? _valueLabel;
        private static readonly ListKeyLabels _listKeyValue;
        private static HashSet<Type> _types = new HashSet<Type>()
        {
            typeof(string),
            typeof(int),
            typeof(int?),
            typeof(long),
            typeof(long?),
            typeof(short),
            typeof(short?),
            typeof(byte),
            typeof(byte?),
            typeof(decimal),
            typeof(decimal?),
            typeof(float),
            typeof(float?),
            typeof(double),
            typeof(double?),
            typeof(bool),
            typeof(bool?),
            typeof(DateTime),
            typeof(DateTime?),
            typeof(DateTimeOffset),
            typeof(DateTimeOffset?),
            typeof(TimeSpan),
            typeof(TimeSpan?),
            typeof(Guid),
            typeof(Guid?),
            typeof(char),
            typeof(char?),
        };

        private static HashSet<Type> _types2 = new HashSet<Type>()
        {
            typeof(string),
            typeof(int),
            typeof(long),
            typeof(short),
            typeof(byte),
            typeof(decimal),
            typeof(float),
            typeof(double),
            typeof(bool),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Guid),
            typeof(char),

            typeof(int?),
            typeof(long?),
            typeof(short?),
            typeof(byte?),
            typeof(decimal?),
            typeof(float?),
            typeof(double?),
            typeof(bool?),
            typeof(DateTime?),
            typeof(DateTimeOffset?),
            typeof(TimeSpan?),
            typeof(Guid?),
            typeof(char?),

            typeof(List<string>),
            typeof(List<int>),
            typeof(List<long>),
            typeof(List<short>),
            typeof(List<byte>),
            typeof(List<decimal>),
            typeof(List<float>),
            typeof(List<double>),
            typeof(List<bool>),
            typeof(List<DateTime>),
            typeof(List<DateTimeOffset>),
            typeof(List<TimeSpan>),
            typeof(List<Guid>),
            typeof(List<char>),

            typeof(string[]),
            typeof(int[]),
            typeof(long[]),
            typeof(short[]),
            typeof(byte[]),
            typeof(decimal[]),
            typeof(float[]),
            typeof(double[]),
            typeof(bool[]),
            typeof(DateTime[]),
            typeof(DateTimeOffset[]),
            typeof(TimeSpan[]),
            typeof(Guid[]),
            typeof(char[]),

        };
        public ListAccessor _listAccessor;

    }

}