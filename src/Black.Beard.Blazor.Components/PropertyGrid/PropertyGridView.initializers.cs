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
				.ConfigureWhere(c => c.IsEnum, (t, mapper, descriptor) =>
				{
					descriptor.EditorType = typeof(ComponentEnumeration);
					descriptor.KindView = PropertyKindView.Enumeration;
					descriptor.ListProvider = typeof(EnumListProvider);
				})

				.ConfigureWhere(c => typeof(IEnumerable).IsAssignableFrom(c), (t, mapper, descriptor) =>
				{
					foreach (var item in descriptor.Type.GetInterfaces())
						if (item.IsGenericType && item.GetGenericTypeDefinition() is Type type && type == typeof(ICollection<>))
						{
							descriptor.SubType = item.GetGenericArguments()[0];
							descriptor.KindView = PropertyKindView.List;
							descriptor.EditorType = typeof(ComponentList);
						}
				})
			;

			strategy
				.ConfigureOnAttribute<DataTypeAttribute>((attribute, mapper, descriptor) =>
				{
					StrategyEditor str = null;
					switch (attribute.DataType)
					{

						case DataType.DateTime:
							if (!mapper.TryGetValueByType(typeof(DateTime), out str))
							{
								descriptor.EditorType = str.ComponentView;
								descriptor.KindView = str.PropertyKindView;
							}
							break;

						case DataType.Time:
							if (!mapper.TryGetValueByType(typeof(DateTime), out str))
							{
								descriptor.EditorType = str.ComponentView;
								descriptor.KindView = str.PropertyKindView;
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

				.ConfigureOnAttribute<ListProviderAttribute>((attribute, mapper, descriptor) =>
				{
					descriptor.ListProvider = attribute.ProviderListType;
					descriptor.EditorType = typeof(ComponentEnumeration);
					descriptor.KindView = PropertyKindView.Enumeration;
				})

				.ConfigureOnAttribute<PasswordPropertyTextAttribute>((attribute, mapper, descriptor) =>
				{
					descriptor.IsPassword = attribute.Password;
					descriptor.EditorType = typeof(ComponentPassword);
                    //descriptor.KindView = PropertyKindView.Password;

                })

				.ConfigureOnAttribute<PropertyTabAttribute>((attribute, mapper, descriptor) =>
				{
					if (System.Diagnostics.Debugger.IsAttached)
						System.Diagnostics.Debugger.Break();
				})

				.ConfigureOnAttribute<TypeConverterAttribute>((attribute, mapper, descriptor) =>
				{
					if (System.Diagnostics.Debugger.IsAttached)
						System.Diagnostics.Debugger.Break();
				})

				.ConfigureOnAttribute<TypeDescriptionProviderAttribute>((attribute, mapper, descriptor) =>
				{
				})

            ;

        }

		private static void InitializeMapping(StrategyMapper strategy)
		{

			strategy
				.ToTarget<ComponentBool>(PropertyKindView.Bool)
                .ToTarget<ComponentChar>(PropertyKindView.Char)
				.ToTarget<ComponentDate>(PropertyKindView.Date)
                .ToTarget<ComponentDateOffset>(PropertyKindView.DateOffset)
				.ToTarget<ComponentDecimal>(PropertyKindView.Decimal)
				.ToTarget<ComponentDouble>(PropertyKindView.Double)
				.ToTarget<ComponentFloat>(PropertyKindView.Float)
                .ToTarget<ComponentInt16>(PropertyKindView.Int16)
                .ToTarget<ComponentInt32>(PropertyKindView.Int32)
                .ToTarget<ComponentInt64>(PropertyKindView.Int64)
				.ToTarget<ComponentString>(PropertyKindView.String)
				.ToTarget<ComponentTime>(PropertyKindView.Time, (c, d) => { d.Mask = StringType.Time; })
				.ToTarget<ComponentUInt16>(PropertyKindView.UInt16)
				.ToTarget<ComponentUInt32>(PropertyKindView.UInt32)
				.ToTarget<ComponentUInt64>(PropertyKindView.UInt64)
				.ToTarget<ComponentGuid>(PropertyKindView.Guid, (c, d) =>
				{
                    //d.DataFormatString = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX";
                    //= ;
                    d.FormatString = "^[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}$";

                    d.CreateMask = () => GuidMask.Guid();
                })
            ;

		}

	}

}
