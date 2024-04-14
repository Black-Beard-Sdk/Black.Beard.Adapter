using Bb.ComponentModel.Attributes;
using Bb.CustomComponents;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bb.PropertyGrid
{

	public partial class PropertyGridView
	{

		private static void Initialize(StrategyMapper strategy)
		{

			InitializeMapping(strategy);

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

		private static void InitializeMapping(StrategyMapper strategy)
		{

			strategy
				.ToTarget<ComponentChar>(PropertyKingView.Char)
				.ToTarget<ComponentBool>(PropertyKingView.Bool)
				.ToTarget<ComponentString>(PropertyKingView.String)
				//.ToTarget<ComponentTime>(PropertyKingView.Time)
				//.ToTarget<ComponentInt16>(PropertyKingView.Int16)
				//.ToTarget<ComponentInt32>(PropertyKingView.Int32)
				//.ToTarget<ComponentInt64>(PropertyKingView.Int64)
				//.ToTarget<ComponentUInt16>(PropertyKingView.UInt16)
				//.ToTarget<ComponentUInt32>(PropertyKingView.UInt32)
				//.ToTarget<ComponentUInt64>(PropertyKingView.UInt64)
				//.ToTarget<ComponentDate>(PropertyKingView.Date)
				//.ToTarget<ComponentDateOffset>(PropertyKingView.DateOffset)
				//.ToTarget<ComponentFloat>(PropertyKingView.Float)
				//.ToTarget<ComponentDouble>(PropertyKingView.Double)
				//.ToTarget<ComponentDecimal>(PropertyKingView.Decimal)
			;

		}

	}

}
