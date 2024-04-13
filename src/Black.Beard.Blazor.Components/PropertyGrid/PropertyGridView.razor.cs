using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Translations;
using Bb.CustomComponents;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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

            PropertyFilter = c => true;

        }

        [Inject]
        public ITranslateService TranslateService { get; set; }

        [Inject]
        public IServiceProvider ServiceProvider { get; set; }

        [Parameter]
        public Func<PropertyObjectDescriptor, bool> PropertyFilter { get; set; }

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
            // StateHasChanged();
            if (PropertyHasChanged != null)
                PropertyHasChanged(obj);
        }

        public DiagnosticValidator Validate()
        {
            return Descriptor.Validate();
        }

        public ObjectDescriptor Descriptor { get; set; }

        public static string StrategyName { get; private set; }



        private static void Initialize(StrategyMapper strategy)
        {

            strategy.ToTarget(c => c.IsEnum, (t, mapper, descriptor) =>
            {
                descriptor.EditorType = typeof(ComponentEnumeration);
                descriptor.KingView = PropertyKingView.Enumeration.ToString();
                descriptor.ListProvider = typeof(EnumListProvider);
            });

            strategy.ToTarget<StringMaskAttribute>((attribute, mapper, descriptor) =>
            {
                descriptor.Mask = attribute.Mask;
            });

            strategy.ToTarget<DataTypeAttribute>((attribute, mapper, descriptor) =>
            {
                StrategyEditor str = null;
                switch (attribute.DataType)
                {

                    case DataType.DateTime:
                        if (!mapper.TryGetValueByType(typeof(DateTime), out str))
                        {
                            descriptor.EditorType = str.ComponentView;
                            descriptor.KingView = str.PropertyKingView;
                        }
                        break;

                    case DataType.Time:
                        if (!mapper.TryGetValueByType(typeof(DateTime), out str))
                        {
                            descriptor.EditorType = str.ComponentView;
                            descriptor.KingView = str.PropertyKingView;
                        }
                        break;

                    case DataType.Password:
                        descriptor.IsPassword = true;
                        descriptor.EditorType = typeof(ComponentPassword);
                        break;

                    case DataType.Duration:
                        descriptor.Mask = StringType.Time;
                        break;

                    case DataType.PhoneNumber:
                        descriptor.Mask = StringType.Telephone;
                        break;

                    case DataType.MultilineText:
                        descriptor.Line = 5;
                        break;

                    case DataType.EmailAddress:
                        descriptor.Mask = StringType.Email;
                        break;

                    case DataType.Url:
                        descriptor.Mask = StringType.Url;
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

            });

            strategy.ToTarget<ListProviderAttribute>((attribute, mapper, descriptor) =>
            {
                descriptor.ListProvider = attribute.ProviderListType;
                descriptor.EditorType = typeof(ComponentEnumeration);
                descriptor.KingView = PropertyKingView.Enumeration.ToString();
            });

            strategy.ToTarget<EditorAttribute>((attribute, mapper, descriptor) =>
            {
                descriptor.EditorType = Type.GetType(attribute.EditorTypeName);
            });

            strategy.ToTarget<PasswordPropertyTextAttribute>((attribute, mapper, descriptor) =>
            {
                descriptor.IsPassword = attribute.Password;
                descriptor.EditorType = typeof(ComponentPassword);
            });

            strategy.ToTarget<StepNumericAttribute>((attribute, mapper, descriptor) =>
            {
                descriptor.Step = attribute.Step;
            });

            strategy.ToTarget<StringLengthAttribute>((attribute, mapper, descriptor) =>
            {
                descriptor.Maximum = attribute.MaximumLength;
                descriptor.Minimum = attribute.MinimumLength;
            });

            strategy.ToTarget<MaxLengthAttribute>((attribute, mapper, descriptor) =>
            {
                descriptor.Maximum = attribute.Length;
            });

            strategy.ToTarget<MinLengthAttribute>((attribute, mapper, descriptor) =>
            {
                descriptor.Maximum = attribute.Length;
            });

            strategy.ToTarget<RangeAttribute>((attribute, mapper, descriptor) =>
            {
                descriptor.Minimum = (int)attribute.Minimum;
                descriptor.Maximum = (int)attribute.Maximum;
            });

            strategy.ToTarget<DisplayFormatAttribute>((attribute, mapper, descriptor) =>
            {
                descriptor.DataFormatString = attribute.DataFormatString;
                descriptor.HtmlEncode = attribute.HtmlEncode;
            });

            strategy.ToTarget<EditableAttribute>((attribute, mapper, descriptor) =>
            {
                descriptor.Browsable = attribute.AllowEdit;
            });

            strategy.ToTarget<BrowsableAttribute>((attribute, mapper, descriptor) =>
            {
                descriptor.Browsable = attribute.Browsable;
            });

            strategy.ToTarget<ReadOnlyAttribute>((attribute, mapper, descriptor) =>
            {
                descriptor.ReadOnly = attribute.IsReadOnly;
            });

            strategy.ToTarget<DefaultValueAttribute>((attribute, mapper, descriptor) =>
            {
                descriptor.DefaultValue = attribute.Value;
            });

            strategy.ToTarget<PropertyTabAttribute>((attribute, mapper, descriptor) =>
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
            });

            strategy.ToTarget<TypeConverterAttribute>((attribute, mapper, descriptor) =>
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
            });

            strategy.ToTarget<TypeDescriptionProviderAttribute>((attribute, mapper, descriptor) =>
            {
            });

            strategy.ToTarget<RequiredAttribute>((attribute, mapper, descriptor) =>
            {
                descriptor.Required = true;
            });
        }


        bool success;
        string[] errors = { };
        MudForm form;
        private object _selectedObject;

    }



}
