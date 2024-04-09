using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.DataAnnotations;
using Bb.ComponentModel.Translations;
using Bb.PropertyGrid;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static Bb.CustomComponents.PropertyDescriptorExtension;
using static MudBlazor.CategoryTypes;

namespace Bb.CustomComponents
{

    public class PropertyObjectDescriptor
    {

        static PropertyObjectDescriptor()
        {

            _strategies = new Dictionary<Type, StrategyEditor>
            {
                { typeof(char), new StrategyEditor(PropertyKingView.Char, typeof(ComponentChar), () => ' ') },
                { typeof(string), new StrategyEditor(PropertyKingView.String, typeof(ComponentString), () => string.Empty) },
                { typeof(bool), new StrategyEditor(PropertyKingView.Bool, typeof(ComponentBool), () => true) },
                { typeof(bool?), new StrategyEditor(PropertyKingView.Bool, typeof(ComponentBool), () => true) },
                { typeof(Int16), new StrategyEditor(PropertyKingView.Int16, typeof(ComponentInt16), () => 0) },
                { typeof(Int16?), new StrategyEditor(PropertyKingView.Int16, typeof(ComponentInt16), () => 0) },
                { typeof(Int32), new StrategyEditor(PropertyKingView.Int32, typeof(ComponentInt32), () => 0) },
                { typeof(Int32?), new StrategyEditor(PropertyKingView.Int32, typeof(ComponentInt32), () => 0) },
                { typeof(Int64), new StrategyEditor(PropertyKingView.Int64, typeof(ComponentInt64), () => 0) },
                { typeof(Int64?), new StrategyEditor(PropertyKingView.Int64, typeof(ComponentInt64), () => 0) },
                { typeof(UInt16), new StrategyEditor(PropertyKingView.UInt16, typeof(ComponentUInt16), () => 0) },
                { typeof(UInt16?), new StrategyEditor(PropertyKingView.UInt16, typeof(ComponentUInt16), () => 0) },
                { typeof(UInt32), new StrategyEditor(PropertyKingView.UInt32, typeof(ComponentUInt32), () => 0) },
                { typeof(UInt32?), new StrategyEditor(PropertyKingView.UInt32, typeof(ComponentInt32), () => 0) },
                { typeof(UInt64), new StrategyEditor(PropertyKingView.UInt64, typeof(ComponentUInt64), () => 0) },
                { typeof(UInt64?), new StrategyEditor(PropertyKingView.UInt64, typeof(ComponentInt64), () => 0) },
                { typeof(DateTime), new StrategyEditor(PropertyKingView.Date, typeof(ComponentDate), () => DateTime.UtcNow) },
                { typeof(DateTime?), new StrategyEditor(PropertyKingView.Date, typeof(ComponentDate), () => DateTime.UtcNow) },
                { typeof(DateTimeOffset), new StrategyEditor(PropertyKingView.DateOffset, typeof(ComponentDateOffset), () => DateTimeOffset.UtcNow) },
                { typeof(DateTimeOffset?), new StrategyEditor(PropertyKingView.DateOffset, typeof(ComponentDateOffset), () => DateTimeOffset.UtcNow) },
                { typeof(TimeSpan), new StrategyEditor(PropertyKingView.Time, typeof(ComponentTime), () => TimeSpan.FromMinutes(0)) },
                { typeof(TimeSpan?), new StrategyEditor(PropertyKingView.Time, typeof(ComponentTime), () => TimeSpan.FromMinutes(0)) },
                { typeof(float), new StrategyEditor(PropertyKingView.Float, typeof(ComponentFloat), () => 0f) },
                { typeof(float?), new StrategyEditor(PropertyKingView.Float, typeof(ComponentFloat), () => 0f) },
                { typeof(double), new StrategyEditor(PropertyKingView.Double, typeof(ComponentDouble), () => 0d) },
                { typeof(double?), new StrategyEditor(PropertyKingView.Double, typeof(ComponentDouble), () => 0d) },
                { typeof(decimal), new StrategyEditor(PropertyKingView.Decimal, typeof(ComponentDecimal), () => 0) },
                { typeof(decimal?), new StrategyEditor(PropertyKingView.Decimal, typeof(ComponentDecimal), () => 0) }
            };

        }

        public static bool Create(Type type, out object? result)
        {

            result = null;

            if (_strategies.TryGetValue(type, out var strategies))
            {
                result = strategies.CreateInstance();
                return true;
            }

            return false;

        }

        public PropertyObjectDescriptor(PropertyDescriptor property, ObjectDescriptor parent)
        {

            this.Parameters = new Dictionary<string, object>
            {
                { "Property", this }
            };

            this.Parent = parent;
            this.Name = property.Name;
            this.PropertyDescriptor = property;

            this.Display = property.DisplayName.GetTranslation(property.Name);
            this.Description = property.Description;
            this.Category = property.Category.GetTranslation();

            this.Browsable = true;
            this.ReadOnly = false;
            this.DefaultValue = null;
            this.Minimum = Int32.MinValue;
            this.Maximum = Int32.MaxValue;
            this.Step = 1;
            this.Type = PropertyDescriptor.PropertyType;
            this.Line = 1;


            if (this.Type.IsGenericType && this.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                this.IsNullable = true;
                this.SubType = Type.GetGenericArguments()[0];
            }
            else
                this.SubType = typeof(void);

            if (_strategies.TryGetValue(this.Type, out StrategyEditor? strategy))
                AssignStrategy(strategy);

            else if (this.Type.IsEnum)
            {
                this.EditorType = typeof(ComponentEnumeration);
                this.KingView = PropertyKingView.Enumeration;
                this.ListProvider = typeof(EnumListProvider);
            }

            else if (typeof(IEnumerable).IsAssignableFrom(this.Type))
                foreach (var item in this.Type.GetInterfaces())
                    if (item.IsGenericType && item.GetGenericTypeDefinition() is Type type && type == typeof(ICollection<>))
                    {
                        this.SubType = item.GetGenericArguments()[0];
                        this.KingView = PropertyKingView.List;
                        this.EditorType = typeof(ComponentList);
                    }

            this.IsValid = this.EditorType != null;

        }


        public object? Value
        {
            get
            {

                object result = PropertyDescriptor.GetValue(Parent.Instance);

                if (result == null)
                    return this.DefaultValue;

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

        internal void AnalyzeAttributes()
        {

            var attributes = PropertyDescriptor.Attributes.OfType<Attribute>().ToList();
            StrategyEditor? strategy;

            foreach (Attribute attribute in attributes)
            {

                if (attribute.GetType().Namespace != "System.Runtime.CompilerServices")

                    switch (attribute)
                    {

                        case StringMaskAttribute stringMask:
                            this.Mask = stringMask.Mask;
                            break;

                        case DataTypeAttribute dataTypeAttribute:
                            switch (dataTypeAttribute.DataType)
                            {

                                case DataType.DateTime:
                                    AssignStrategy(_strategies[typeof(DateTime)]);
                                    break;

                                case DataType.Date:
                                    AssignStrategy(_strategies[typeof(DateTime)]);
                                    break;

                                case DataType.Time:
                                    AssignStrategy(_strategies[typeof(TimeSpan)]);
                                    break;

                                case DataType.Password:
                                    IsPassword = true;
                                    this.EditorType = typeof(ComponentPassword);
                                    break;

                                case DataType.Duration:
                                    this.Mask = StringType.Time;
                                    break;

                                case DataType.PhoneNumber:
                                    this.Mask = StringType.Telephone;
                                    break;

                                case DataType.MultilineText:
                                    Line = 5;
                                    break;

                                case DataType.EmailAddress:
                                    this.Mask = StringType.Email;
                                    break;

                                case DataType.Url:
                                    this.Mask = StringType.Url;
                                    break;

                                case DataType.Currency:
                                    break;
                                case DataType.Html:
                                    break;
                                case DataType.ImageUrl:
                                    break;
                                case DataType.CreditCard:
                                    break;
                                case DataType.PostalCode:
                                    break;
                                case DataType.Upload:
                                    break;
                                case DataType.Custom:
                                    break;
                                default:
                                    break;

                            }
                            break;

                        case DisplayOnUITextAreaAttribute:
                            this.Line = 5;
                            break;

                        case ListProviderAttribute listProviderAttribute:
                            this.ListProvider = listProviderAttribute.EnumerationResolver;
                            this.EditorType = typeof(ComponentEnumeration);
                            this.KingView = PropertyKingView.Enumeration;
                            break;

                        case EditorAttribute editor:
                            this.EditorType = Type.GetType(editor.EditorTypeName);
                            break;

                        case CategoryAttribute:
                        case DisplayNameAttribute:
                        case DescriptionAttribute:
                            break;

                        case PasswordPropertyTextAttribute passwordPropertyText:
                            IsPassword = passwordPropertyText.Password;
                            this.EditorType = typeof(ComponentPassword);
                            break;

                        case StepNumericAttribute stepNumeric:
                            this.Step = stepNumeric.Step;
                            break;

                        case StringLengthAttribute stringLength:
                            this.Maximum = stringLength.MaximumLength;
                            this.Minimum = stringLength.MinimumLength;
                            break;

                        case MaxLengthAttribute maxLength:
                            this.Maximum = maxLength.Length;
                            break;

                        case MinLengthAttribute minLength:
                            this.Maximum = minLength.Length;
                            break;

                        case RangeAttribute range:
                            this.Minimum = (int)range.Minimum;
                            this.Maximum = (int)range.Maximum;
                            break;

                        case DisplayFormatAttribute displayFormat:
                            this.DataFormatString = displayFormat.DataFormatString;
                            this.HtmlEncode = displayFormat.HtmlEncode;
                            break;

                        case EditableAttribute editable:
                            this.Browsable = editable.AllowEdit;
                            break;

                        case BrowsableAttribute browsable:
                            this.Browsable = browsable.Browsable;
                            break;

                        case ReadOnlyAttribute readOnly:
                            this.ReadOnly = readOnly.IsReadOnly;
                            break;

                        case DefaultValueAttribute defaultValue:
                            this.DefaultValue = defaultValue.Value;
                            break;

                        case PropertyTabAttribute propertyTab:
                            if (System.Diagnostics.Debugger.IsAttached)
                                System.Diagnostics.Debugger.Break();
                            break;

                        case TypeConverterAttribute typeConverter:
                            if (System.Diagnostics.Debugger.IsAttached)
                                System.Diagnostics.Debugger.Break();
                            break;

                        case TypeDescriptionProviderAttribute typeDescriptionProvider:
                            if (System.Diagnostics.Debugger.IsAttached)
                                System.Diagnostics.Debugger.Break();
                            break;

                        //case Bb.ComponentModel.DataAnnotations.TranslationKeyAttribute translationKey:
                        //    break;

                        default:
                            break;

                    }

                if (attribute is ValidationAttribute validation)
                    if (validation is RequiredAttribute required)
                        this.Required = true;

            }

        }

        private void AssignStrategy(StrategyEditor strategy)
        {
            this.EditorType = strategy.ComponentView;
            this.KingView = strategy.PropertyKingView;
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


        public IDictionary<string, object> Parameters { get; set; }


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

        public TranslatedKeyLabel Display { get; private set; }

        public TranslatedKeyLabel Description { get; private set; }

        public TranslatedKeyLabel Category { get; private set; }

        public bool Browsable { get; private set; }

        public bool ReadOnly { get; private set; }

        public object? DefaultValue { get; private set; }

        public bool Required { get; private set; }

        public string? DataFormatString { get; private set; }

        public bool HtmlEncode { get; private set; }
        public string Name { get; }
        public ObjectDescriptor Parent { get; }

        public PropertyKingView KingView { get; private set; }

        public bool IsNullable { get; }
        public Type SubType { get; }

        public int Minimum { get; private set; }

        public int Maximum { get; private set; }

        public float Step { get; private set; }

        public bool IsPassword { get; private set; }

        public Type? EditorType { get; private set; }

        public Type ListProvider { get; private set; }

        public PropertyDescriptor PropertyDescriptor { get; set; }

        public bool IsValid { get; }
        public int Line { get; private set; }
        public StringType Mask { get; private set; }

        private static Dictionary<Type, StrategyEditor> _strategies;

    }


    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class StringMaskAttribute : Attribute
    {

        public StringMaskAttribute(StringType mask)
        {
            this.Mask = mask;
        }

        public StringType Mask { get; }
    }

    public enum StringType
    {
        Undefined,
        Email,
        Number,
        //Search,
        Telephone,
        Url,
        Color,
        Date,
        DateTimeLocal,
        Month,
        Time,
        Week,
    }


    public static class PropertyDescriptorExtension
    {

        public static DiagnosticValidatorItem ValidateValue(this PropertyDescriptor descriptor, object value, ITranslateService translator = null)
        {

            var messages = new DiagnosticValidatorItem(descriptor);

            var attributes = descriptor
                .Attributes
                .OfType<Attribute>()
                .ToList();

            foreach (Attribute attribute in attributes)
                if (attribute is ValidationAttribute validation)
                    if (!validation.IsValid(value))
                    {

                        var label = descriptor.DisplayName;

                        if (translator != null)
                        {
                            var ll = label.Replace(descriptor.Name, "{0}");
                            label = translator.Translate(ll);
                        }

                        var message = validation.FormatErrorMessage(label);

                        if (translator != null && validation.FormatErrorMessage(string.Empty).IsValidTranslationKey())
                            messages.Add(translator.Translate(message));
                        else
                            messages.Add(message);

                    }

            return messages;

        }


        public static DiagnosticValidatorItem ValidateInstance(this PropertyDescriptor descriptor, object instance, ITranslateService translator = null)
        {

            var messages = new DiagnosticValidatorItem(descriptor);

            var attributes = descriptor
                .Attributes
                .OfType<Attribute>()
                .ToList();

            object value = descriptor.GetValue(instance);

            foreach (Attribute attribute in attributes)
                if (attribute is ValidationAttribute validation)
                    if (!validation.IsValid(value))
                    {

                        var label = descriptor.DisplayName;

                        if (translator != null)
                        {
                            var ll = label.Replace(descriptor.Name, "{0}");
                            label = translator.Translate(ll);
                        }

                        var message = validation.FormatErrorMessage(label);

                        if (translator != null && validation.FormatErrorMessage(string.Empty).IsValidTranslationKey())
                            messages.Add(translator.Translate(message));
                        else
                            messages.Add(message);

                    }

            return messages;

        }

        public static DiagnosticValidator Validate(this object self, ITranslateService translator = null)
        {

            DiagnosticValidator validator = new DiagnosticValidator();
            var properties = TypeDescriptor.GetProperties(self.GetType());
            foreach (PropertyDescriptor property in properties)
            {
                var result = property.ValidateInstance(self, translator);
                if (!result.IsValid)
                    validator.Add(result);
            }

            return validator;

        }



    }


    public class DiagnosticValidatorItem
    {

        public DiagnosticValidatorItem(PropertyDescriptor descriptor)
        {
            this.Descriptor = descriptor;
            this._diagnostics = new List<string>();
        }

        public void Add(string message)
        {
            _diagnostics.Add(message);
        }


        public string Message => String.Concat(_diagnostics.Select(c => ", " + c)).Trim(',', ' ');

        public bool IsValid => _diagnostics.Count == 0;

        public List<string> MessageService => _diagnostics;

        public PropertyDescriptor Descriptor { get; }

        private readonly List<string> _diagnostics;

    }

    public class DiagnosticValidator
    {

        public DiagnosticValidator()
        {
            this._diagnostics = new List<DiagnosticValidatorItem>();
        }

        public DiagnosticValidatorItem Add(DiagnosticValidatorItem item)
        {
            _diagnostics.Add(item);
            return item;
        }

        private readonly List<DiagnosticValidatorItem> _diagnostics;

        public bool IsValid => !_diagnostics.Where(c => !c.IsValid).Any();
    }


}
