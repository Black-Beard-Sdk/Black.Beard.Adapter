using Bb.ComponentModel.Translations;
using Bb.TypeDescriptors;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;
using Bb.ComponentModel.Accessors;

namespace Bb.ComponentDescriptors
{

    public class Descriptor : ITranslateHost
    {

        public Descriptor(
            IServiceProvider serviceProvider,
            ITranslateHost hostTranslateService,
            string strategyKey,
            Type type,
            Func<PropertyDescriptor, bool> propertyDescriptorFilter,
            Func<PropertyObjectDescriptor, bool> propertyFilter
            )
        {


            if (type == null)
                throw new ArgumentNullException(nameof(type));

            _items = new List<Descriptor>();
            _strategy = string.IsNullOrEmpty(strategyKey)
                ? StrategyMapper.Get(string.Empty)
                : StrategyMapper.Get(strategyKey);

            ServiceProvider = serviceProvider;
            TranslationService = hostTranslateService.TranslationService;

            StrategyName = _strategy.Key;

            this.Type = type;
            this.IsNullable = type.IsClass;

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

            if (_types.Contains(type) && CanBeCreated(type))
            {

                var a = type.GetAccessors();

                try
                {
                    _accessorValueLabel = a.Where(c => c.ContainsAttribute<ValueLabelAttribute>()).FirstOrDefault()
                        ?? a.Where(c => c.ContainsAttribute<KeyAttribute>()).FirstOrDefault();
                }
                catch (Exception ex)
                {


                }

            }

        }

        public Descriptor CreateSub(object instance, Type type = null)
        {
            return new SubObjectDescriptor(instance, type ?? instance.GetType(), this);
        }

        public string GetValueLabel(object parent, string defaultValue)
        {
            if (_accessorValueLabel != null)
            {
                var i = _accessorValueLabel.GetValue(parent)?.ToString();
                if (!string.IsNullOrEmpty(i))
                    return i;
            }
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

        //protected static bool ResolveSubType(Type type, out Type subType, out bool isNullable, out bool isBrowsable, out bool isArray)
        //{

        //    isArray = false;
        //    subType = null;
        //    isBrowsable = true;
        //    isNullable = false;

        //    if (_types.Contains(type)) { }

        //    else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        //    {
        //        isNullable = true;
        //        subType = type.GetGenericArguments()[0];
        //    }

        //    else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
        //    {
        //        var interfaces = type.GetInterfaces();
        //        foreach (var item in interfaces)
        //            if (item.IsGenericType && item.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        //            {
        //                subType = item.GetGenericArguments()[0];
        //                if (!CanBeCreated(subType))
        //                    isBrowsable = false;
        //                break;
        //            }
        //        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        //        {
        //            subType = type.GetGenericArguments()[0];
        //            if (!CanBeCreated(subType))
        //                isBrowsable = false;
        //        }
        //    }

        //    else if (type.IsArray)
        //    {
        //        subType = type.GetElementType();
        //        isArray = true;
        //    }
        //    else
        //    {

        //    }

        //    return subType != default;

        //}

        protected static bool ResolveSubType(Type type, out Type subType, out bool isNullable)
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
            else
            {

            }

            return subType != default;

        }


        private static MethodInfo GetMethod(Type type, string name)
        {

            var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(c => c.Name == name).ToList();

            if (methods.Count > 1)
            {

            }

            var method = methods.FirstOrDefault();

            return method;

        }

        protected static Method Resolve(Type type, string methodName, string methodResultName)
        {

            Method m = null;
            var method = GetMethod(type, methodName);
            if (method != null)
            {
                Action<object, object> action = (object instance, object value) =>
                {
                    method.Invoke(instance, new object[] { value });
                };

                m = new Method()
                {
                    Name = methodResultName,
                    Action = action,
                    Type = method.DeclaringType,
                };

            }

            if (m == null)
            {

            }

            return m;

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
            this._items.Add(descriptor);

        }

        public virtual void SetUI(object ui)
        {
            Ui = ui;
            foreach (var item in _items)
                item.SetUI(ui);
        }


        public static bool IsStapleType(Type type)
        {
            return _types.Contains(type);
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


        public Descriptor Parent { get; protected set; }

        /// <summary>
        /// Validation error text
        /// </summary>
        public string ErrorText { get; set; }

        /// <summary>
        /// Return true if the validation is failed
        /// </summary>
        public bool InError { get; set; }

        public bool IsNullable { get; set; }

        public ITranslateService TranslationService { get; }

        public IServiceProvider ServiceProvider { get; }

        public virtual object Value { get; set; }

        public bool IsEnumerable { get; private set; }

        public Type ComponentView { get; set; }

        public virtual TranslatedKeyLabel Display { get; internal protected set; }

        public virtual TranslatedKeyLabel Description { get; internal protected set; }

        public bool Browsable { get; internal protected set; }

        public bool Enabled { get; internal set; }

        public string KindView { get; set; }

        public object Ui { get; protected set; }

        public Func<PropertyDescriptor, bool> PropertyDescriptorFilter { get; }

        public Func<PropertyObjectDescriptor, bool> PropertyFilter { get; }

        public IEnumerable<Descriptor> Items { get => _items; }

        /// <summary>
        /// Global strategy name
        /// </summary>
        public string StrategyName { get; }

        public Type Type { get; }

        public Action<PropertyObjectDescriptor, object> PropertyHasChanged { get; set; }


        public bool CanAdd => AddMethod != null;
        public Method AddMethod { get; protected set; }

        public bool CanDel => DelMethod != null;
        public Method DelMethod { get; protected set; }


        protected readonly StrategyMapper _strategy;
        private readonly List<Descriptor> _items;
        private readonly string? _valueLabel;
        private readonly AccessorItem? _accessorValueLabel;

        private static HashSet<Type> _types = new HashSet<Type>()
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
        };

    }


    public class Method
    {

        public Type Type { get; set; }

        public string Name { get; set; }

        public Action<object, object> Action { get; set; }

    }

    //[System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = true)]
    //public sealed class MethodAttribute : Attribute
    //{

    //    // This is a positional argument
    //    public MethodAttribute(string context, string methodType, string methodName)
    //    {
    //        this.Context = context;
    //        this.MethodType = methodType;
    //        this.MethodName = methodName;
    //    }

    //    public string Context { get; }

    //    public string MethodType { get; }

    //    public string MethodName { get; }

    //}

}