using Bb.ComponentDescriptors;
using Bb.ComponentModel.Attributes;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bb.PropertyGrid
{

	public partial class PropertyGridView
	{

		private static void Initialize(StrategyMapper strategy)
		{

			InitializeMapping(strategy);

			strategy
				.ToTarget(c => c.IsEnum, (t, mapper, descriptor) =>
				{
					descriptor.EditorType = typeof(ComponentEnumeration);
					descriptor.KindView = PropertyKingView.Enumeration.ToString();
					descriptor.ListProvider = typeof(EnumListProvider);
				})

				.ToTarget(c => typeof(IEnumerable).IsAssignableFrom(c), (t, mapper, descriptor) =>
				{
					foreach (var item in descriptor.Type.GetInterfaces())
						if (item.IsGenericType && item.GetGenericTypeDefinition() is Type type && type == typeof(ICollection<>))
						{
							descriptor.SubType = item.GetGenericArguments()[0];
							descriptor.KindView = PropertyKingView.List.ToString();
							descriptor.EditorType = typeof(ComponentList);
						}
				})
			;

			strategy

				.ToTarget<StringMaskAttribute>((attribute, mapper, descriptor) =>
				{
					descriptor.Mask = attribute.Mask;
				})

				.ToTarget<DataTypeAttribute>((attribute, mapper, descriptor) =>
				{
					StrategyEditor str = null;
					switch (attribute.DataType)
					{

						case DataType.DateTime:
							if (!mapper.TryGetValueByType(typeof(DateTime), out str))
							{
								descriptor.EditorType = str.ComponentView;
								descriptor.KindView = str.PropertyKingView;
							}
							break;

						case DataType.Time:
							if (!mapper.TryGetValueByType(typeof(DateTime), out str))
							{
								descriptor.EditorType = str.ComponentView;
								descriptor.KindView = str.PropertyKingView;
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

				})

				.ToTarget<ListProviderAttribute>((attribute, mapper, descriptor) =>
				{
					descriptor.ListProvider = attribute.ProviderListType;
					descriptor.EditorType = typeof(ComponentEnumeration);
					descriptor.KindView = PropertyKingView.Enumeration.ToString();
				})

				.ToTarget<EditorAttribute>((attribute, mapper, descriptor) =>
				{
					descriptor.EditorType = Type.GetType(attribute.EditorTypeName);
				})

				.ToTarget<PasswordPropertyTextAttribute>((attribute, mapper, descriptor) =>
				{
					descriptor.IsPassword = attribute.Password;
					descriptor.EditorType = typeof(ComponentPassword);
				})

				.ToTarget<StepNumericAttribute>((attribute, mapper, descriptor) =>
				{
					descriptor.Step = attribute.Step;
				})

				.ToTarget<StringLengthAttribute>((attribute, mapper, descriptor) =>
				{
					descriptor.Maximum = attribute.MaximumLength;
					descriptor.Minimum = attribute.MinimumLength;
				})

				.ToTarget<MaxLengthAttribute>((attribute, mapper, descriptor) =>
				{
					descriptor.Maximum = attribute.Length;
				})

				.ToTarget<MinLengthAttribute>((attribute, mapper, descriptor) =>
				{
					descriptor.Maximum = attribute.Length;
				})

				.ToTarget<RangeAttribute>((attribute, mapper, descriptor) =>
				{
					descriptor.Minimum = (int)attribute.Minimum;
					descriptor.Maximum = (int)attribute.Maximum;
				})

				.ToTarget<DisplayFormatAttribute>((attribute, mapper, descriptor) =>
				{
					descriptor.DataFormatString = attribute.DataFormatString;
					descriptor.HtmlEncode = attribute.HtmlEncode;
				})

				.ToTarget<EditableAttribute>((attribute, mapper, descriptor) =>
				{
					descriptor.Browsable = attribute.AllowEdit;
				})

				.ToTarget<BrowsableAttribute>((attribute, mapper, descriptor) =>
				{
					descriptor.Browsable = attribute.Browsable;
				})

				.ToTarget<ReadOnlyAttribute>((attribute, mapper, descriptor) =>
				{
					descriptor.ReadOnly = attribute.IsReadOnly;
				})

				.ToTarget<DefaultValueAttribute>((attribute, mapper, descriptor) =>
				{
					descriptor.DefaultValue = attribute.Value;
				})

				.ToTarget<PropertyTabAttribute>((attribute, mapper, descriptor) =>
				{
					if (System.Diagnostics.Debugger.IsAttached)
						System.Diagnostics.Debugger.Break();
				})

				.ToTarget<TypeConverterAttribute>((attribute, mapper, descriptor) =>
				{
					if (System.Diagnostics.Debugger.IsAttached)
						System.Diagnostics.Debugger.Break();
				})

				.ToTarget<TypeDescriptionProviderAttribute>((attribute, mapper, descriptor) =>
				{
				})

				.ToTarget<RequiredAttribute>((attribute, mapper, descriptor) =>
				{
					descriptor.Required = true;
				})

                .ToTarget<DesignerAttribute>((attribute, mapper, descriptor) =>
                {
                    descriptor.EditorType = Type.GetType(attribute.DesignerTypeName);
                })

            ;

        }

		private static void InitializeMapping(StrategyMapper strategy)
		{

			strategy
				.ToTarget<ComponentBool>(PropertyKingView.Bool)
                .ToTarget<ComponentChar>(PropertyKingView.Char)
				.ToTarget<ComponentDate>(PropertyKingView.Date)
                .ToTarget<ComponentDateOffset>(PropertyKingView.DateOffset)
				.ToTarget<ComponentDecimal>(PropertyKingView.Decimal)
				.ToTarget<ComponentDouble>(PropertyKingView.Double)
				.ToTarget<ComponentFloat>(PropertyKingView.Float)
                .ToTarget<ComponentInt16>(PropertyKingView.Int16)
                .ToTarget<ComponentInt32>(PropertyKingView.Int32)
                .ToTarget<ComponentInt64>(PropertyKingView.Int64)
				.ToTarget<ComponentString>(PropertyKingView.String)
				.ToTarget<ComponentTime>(PropertyKingView.Time, (c, d) => { d.Mask = StringType.Time; })
				.ToTarget<ComponentUInt16>(PropertyKingView.UInt16)
				.ToTarget<ComponentUInt32>(PropertyKingView.UInt32)
				.ToTarget<ComponentUInt64>(PropertyKingView.UInt64)
			;

		}

	}

}
