using Bb.ComponentModel.Translations;
using MudBlazor;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Bb.ComponentDescriptors
{


    [DebuggerDisplay("{Name} : {Type}")]
    public class PropertyObjectDescriptor : Descriptor
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentDescriptors.PropertyObjectDescriptor"/> class.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="parent"></param>
        /// <param name="strategyKey"></param>
        public PropertyObjectDescriptor(System.ComponentModel.PropertyDescriptor property, Descriptor parent, string strategyKey, Func<PropertyDescriptor, bool> propertyDescriptorFilter,
            Func<PropertyObjectDescriptor, bool> propertyFilter)
            : base(parent.ServiceProvider, parent, strategyKey, property.PropertyType, propertyDescriptorFilter, propertyFilter)
        {

            Parent = parent;
            Name = property.Name;
            PropertyDescriptor = property;

            Analyze();

            DefaultValue = null;
            Minimum = int.MinValue;
            Maximum = int.MaxValue;

            Step = 1;
            Line = 1;
            ComponentType = property.ComponentType;


            if (ResolveSubType(Type, out Type sub, out bool isNullable))
            {
                SubType = sub;
                IsNullable = isNullable;
                //AddMethod = Resolve(Type, "Add", "Add");
                //DelMethod = Resolve(Type, "Remove", "Del");
            }

            IsNullable = isNullable;


            //if (ResolveSubType
            //(
            //    Type, out Type sub,
            //    out bool isNullable, out bool isBrowsable, out bool isArray))
            //{
            //    SubType = sub;
            //    IsNullable = isNullable;
            //    Browsable = isBrowsable;
            //    if (isBrowsable)
            //    {
            //        if (isArray)
            //        {

            //        }
            //        else
            //        {

            //            AddMethod = Resolve(Type, "Add", "Add");
            //            DelMethod = Resolve(Type, "Remove", "Del");

            //            if (AddMethod == null && Value is IEnumerable e && IsEmpty(e))
            //                isBrowsable = false;

            //        }
            //    }

            //}
            //else
            //{
            //    IsNullable = isNullable;
            //    Browsable = isBrowsable;
            //}

        }

        private static bool IsEmpty(IEnumerable e)
        {
            foreach (var item in e)
                return false;
            return true;
        }

        protected override void Analyze()
        {

            var property = PropertyDescriptor;

            Display = property.DisplayName.GetTranslation(property.Name);
            Description = property.Description;
            Category = property.Category.GetTranslation();
            Browsable = property.IsBrowsable;
            ReadOnly = property.IsReadOnly;

            base.Analyze();

            IsValid = ComponentView != null;

        }

        /// <summary>
        /// return true if the property is in the list and a strategy is defined
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool Create(string name, Type type, out object result)
        {

            result = null;

            var _strategy = StrategyMapper.Get(name);
            if (_strategy != null)
            {
                if (_strategy.TryGetValueByType(type, out var strategies))
                {
                    result = strategies?.CreateInstance();
                    return true;
                }
            }

            return false;

        }

        /// <summary>
        /// return the value of the property for the current instance
        /// </summary>
        public override object Value
        {
            get
            {

                object result = PropertyDescriptor.GetValue(Parent.Value);

                //if (result == null)
                //    return this.DefaultValue;

                return result;

            }

            set
            {
                PropertyDescriptor.SetValue(Parent.Value, value);
                PropertyChange();
            }

        }

        /// <summary>
        /// Refresh
        /// </summary>
        public void PropertyChange()
        {
            Parent.HasChanged(this);
        }

        protected override void AssignStrategy(StrategyEditor strategy)
        {

            var i = strategy.AttributeInitializers;
            if (i != null)
            {
                var attributes = PropertyDescriptor.Attributes.OfType<Attribute>().ToList();
                foreach (Attribute attribute in attributes)
                    if (i.TryGetValue(attribute.GetType(), out var a))
                        a(attribute, _strategy, this);
            }

            base.AssignStrategy(strategy);

        }

        /// <summary>
        /// Validate the property in the instance
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool Validate(out DiagnosticValidatorItem result)
        {
            result = PropertyDescriptor.ValidateValue(Value, TranslationService);
            return result.IsValid;
        }

        /// <summary>
        /// Return true if Validation of the property in the instance is valid
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool Validate(object value, out DiagnosticValidatorItem result)
        {
            result = PropertyDescriptor.ValidateValue(value, TranslationService);
            return result.IsValid;
        }


        /// <summary>
        /// Return the translated category
        /// </summary>
        /// <returns></returns>
        public string GetCategory()
        {
            TranslatedKeyLabel r = Category ?? "Misc";
            if (TranslationService != null)
                return TranslationService?.Translate(r);
            return r;
        }

        /// <summary>
        /// Action to execute after validation UI status is changed
        /// </summary>
        public Action<IComponentFieldBase> UIPropertyValidationHasChanged { get; set; }

        /// <summary>
        /// Action to execute after validation status is changed
        /// </summary>
        public Action<PropertyObjectDescriptor> PropertyValidationHasChanged { get; set; }

        public Action<PropertyObjectDescriptor> PropertyHasChanged { get; set; }


        public override string ToString()
        {
            return $"{Name} : {Type.Name}, {SubType?.Name}";
        }


        /// <summary>
        /// Name of the property
        /// </summary>
        public string Name { get; }


        /// <summary>
        /// Name of the property
        /// </summary>
        public override TranslatedKeyLabel Display
        {
            get => string.IsNullOrEmpty(base.Display) ? Name : base.Display;
            internal protected set 
            {
                base.Display = value; 
            }
        }

        ///// <summary>
        ///// Global strategy name
        ///// </summary>
        //public string StrategyName { get; }

        public Type ComponentType { get; }

        public Type SubType { get; set; }

        //public Descriptor Parent { get; }

        public bool IsValid { get; private set; }

        /// <summary>
        /// PropertyDescriptor
        /// </summary>
        public System.ComponentModel.PropertyDescriptor PropertyDescriptor { get; set; }

        public TranslatedKeyLabel Category { get; set; }

        public bool ReadOnly { get; set; }

        public object DefaultValue { get; set; }

        public bool Required { get; set; }

        public bool HtmlEncode { get; set; }

        public int Minimum { get; set; }

        public int Maximum { get; set; }

        public float Step { get; set; }

        public bool IsPassword { get; set; }

        public Type ListProvider { get; set; }

        public int Line { get; set; }

        /// <summary>
        /// Delegate for create IMask
        /// </summary>
        public Func<IMask> CreateMask { get; internal set; }

        public string FormatString { get; set; }

        public string PatternString { get; set; }

        public StringType Mask { get; set; }



    }


}
