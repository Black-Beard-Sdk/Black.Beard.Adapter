using Bb.ComponentModel.Factories;
using Bb.ComponentModel.Translations;
using Bb.PropertyGrid;
using ICSharpCode.Decompiler.TypeSystem;
using System.Collections;
using System.ComponentModel;

namespace Bb.CustomComponents
{



    public class PropertyObjectDescriptor
    {      

        public PropertyObjectDescriptor(PropertyDescriptor property, ObjectDescriptor parent, string strategyKey)
        {

            _strategy = string.IsNullOrEmpty(strategyKey)
                ? StrategyMapper.Get(string.Empty)
                : StrategyMapper.Get(strategyKey);

            StrategyName = _strategy.Key;            
            this.Parent = parent;
            this.Name = property.Name;
            this.PropertyDescriptor = property;
            this.Display = property.DisplayName.GetTranslation(property.Name);
            this.Description = property.Description;
            this.Category = property.Category.GetTranslation();
            this.Browsable = property.IsBrowsable;
            this.ReadOnly = property.IsReadOnly;
            this.DefaultValue = null;
            this.Minimum = Int32.MinValue;
            this.Maximum = Int32.MaxValue;
            this.Type = PropertyDescriptor.PropertyType;
            this.Step = 1;
            this.Line = 1;
            this.ComponentType = property.ComponentType;

            if (this.Type.IsGenericType && this.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                this.IsNullable = true;
                this.SubType = Type.GetGenericArguments()[0];
            }
            else
                this.SubType = typeof(void);
        }

        internal PropertyObjectDescriptor Build()
        {

            if (_strategy.TryGetValueByType(this.Type, out StrategyEditor? strategyEditor))
                AssignStrategy(strategyEditor);
         
            this.IsValid = this.EditorType != null;

            return this;

        }

        public static bool Create(string name, Type type, out object? result)
        {

            result = null;

            var _strategy = StrategyMapper.Get(name);

            if (_strategy.TryGetValueByType(type, out var strategies))
            {
                result = strategies?.CreateInstance();
                return true;
            }

            return false;

        }

        public object? Value
        {
            get
            {

                object result = PropertyDescriptor.GetValue(Parent.Instance);

                //if (result == null)
                //    return this.DefaultValue;

                return result;

            }

            set
            {
                PropertyDescriptor.SetValue(Parent.Instance, value);
                PropertyChange();
            }

        }

        public Action<ComponentFieldBase> UIPropertyValidationHasChanged { get; set; }

        public Action<PropertyObjectDescriptor> PropertyValidationHasChanged { get; set; }

        public Action<PropertyObjectDescriptor> PropertyHasChanged { get; set; }

        public void PropertyChange()
        {
            this.Parent.HasChanged(this);
        }

        private void AssignStrategy(StrategyEditor strategy)
        {

            this.EditorType = strategy.ComponentView;
            this.KindView = strategy.PropertyKingView;

            if (strategy.Initializer != null)
                strategy.Initializer(_strategy, this);

            var i = strategy.AttributeInitializers;
            if (i != null)
            {
                var attributes = PropertyDescriptor.Attributes.OfType<Attribute>().ToList();
                foreach (Attribute attribute in attributes)
                    if (i.TryGetValue(attribute.GetType(), out var a))
                        a(attribute, _strategy, this);
            }

            foreach (var item in strategy.Initializers)
                item(this.Type, _strategy, this);

        }

        public bool Validate(out DiagnosticValidatorItem result)
        {
            result = this.PropertyDescriptor.ValidateValue(this.Value, this.Parent.TranslateService);
            return result.IsValid;
        }

        public bool Validate(object value, out DiagnosticValidatorItem result)
        {
            result = this.PropertyDescriptor.ValidateValue(value, this.Parent.TranslateService);
            return result.IsValid;
        }

        public Type Type { get; internal set; }

        private readonly StrategyMapper _strategy;

        public string StrategyName { get; }

        public string? ErrorText { get; internal set; }

        public bool InError { get; internal set; }

        public string GetDisplay()
        {
            return Parent.TranslateService.Translate(this.Display);
        }

        public string GetDescription()
        {
            return Parent.TranslateService.Translate(this.Description);
        }

        public string GetCategory()
        {
            return Parent.TranslateService.Translate(this.Category);
        }

        public Type SubType { get; set; }

        public string Name { get; }
        public ObjectDescriptor Parent { get; }

        public bool IsValid { get; private set; }

        public PropertyDescriptor PropertyDescriptor { get; set; }

        public TranslatedKeyLabel Display { get; set; }

        public TranslatedKeyLabel Description { get; set; }

        public TranslatedKeyLabel Category { get; set; }

        public bool Browsable { get; set; }

        public bool ReadOnly { get; set; }

        public object? DefaultValue { get; set; }

        public bool Required { get; set; }

        public string? DataFormatString { get; set; }

        public bool HtmlEncode { get; set; }

        public string KindView { get; set; }

        public bool IsNullable { get; set; }

        public int Minimum { get; set; }

        public int Maximum { get; set; }

        public float Step { get; set; }

        public bool IsPassword { get; set; }

        public Type? EditorType { get; set; }

        public Type ListProvider { get; set; }

        public int Line { get; set; }

        public Type ComponentType { get; }

        public StringType Mask { get; set; }

        public bool Enabled { get; set; }

    }

}
