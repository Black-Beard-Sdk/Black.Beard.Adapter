## PropertyGrid

The propertyGrid ca shown the properties of a model and allow the user to edit them. The component is based on the decoration by attribute

```html

    <PropertyGridView 
        SelectedObject="@CurrentModel" 
        PropertyHasChanged="PropertyHasChanged" />
    
```


```csharp

    public class Model
    {
        [Required]  // decoration for specify the property is required
        public string Name { get; set; }
    }
  
```

```csharp

    // System.Runtime.CompilerServices

    // specify a mask for string data
    [StringMaskAttribute("stringMask")]

    // specify the type of the data is DateTime
    [DataTypeAttribute (DataType.DateTime)]

    // specify the type of the data is Date
    [DataTypeAttribute (DataType.Date)]

    // specify the type of the data is TimeSpan
    [DataTypeAttribute (DataType.Time)]

    // specify the type of the data is TimeSpan
    [DataTypeAttribute (DataType.Time)]

    // specify the type of the data is string with mask '*******'
    [DataTypeAttribute (DataType.Password)]
    [PasswordPropertyTextAttribute]

    // specify the type of the data is string with mask 'dd:dd'
    [DataTypeAttribute (DataType.Duration)]

    // specify the type of the data is string with mask email
    [DataTypeAttribute (DataType.Telephone)]

    // specify the type of the data is text multi line.
    [DataTypeAttribute (DataType.MultilineText)]
    [DisplayOnUITextAreaAttribute]

    // specify the type of the data is email
    [DataTypeAttribute (DataType.EmailAddress)]

    // specify the type of the data is url
    [DataTypeAttribute (DataType.Url)]

    // Specify contraints 
    [StringLengthAttribute(minimum length, maximum length)]
    [MaxLengthAttribute(length string)]
    [MinLengthAttribute(length)]
    [RangeAttribute(minimum, maximum)]

    // Not yet implemented

    // specify the type of the data is string with mask '¤ dd.dd'
    [DataTypeAttribute (DataType.Currency)]

    // specify the type of the data is string multi line
    [DataTypeAttribute (DataType.Html)]

    // specify the type of the data is read only
    [ReadOnlyAttribute]

    case DataType.ImageUrl:
    case DataType.CreditCard:
    case DataType.PostalCode:
    case DataType.Upload:
    case DataType.Custom:
   
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
    
    
    case StepNumericAttribute stepNumeric:
    case DisplayFormatAttribute displayFormat:
    case EditableAttribute editable:
    case BrowsableAttribute browsable:
    case DefaultValueAttribute defaultValue:
    case PropertyTabAttribute propertyTab:
    case TypeConverterAttribute typeConverter:
    case TypeDescriptionProviderAttribute typeDescriptionProvider:

```