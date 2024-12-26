using Bb.ComponentDescriptors;
using Bb.ComponentModel.Attributes;
using Microsoft.AspNetCore.Components;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Transactions;

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
                    descriptor.ComponentView = typeof(ComponentEnumeration);
                    descriptor.KindView = PropertyKindView.Enumeration;
                    if (descriptor is ComponentDescriptors.PropertyObjectDescriptor prop)
                        prop.ListProvider = typeof(EnumListProvider);
                })

                .ConfigureWhere(c => typeof(IEnumerable).IsAssignableFrom(c), (t, mapper, descriptor) =>
                {
                    foreach (var item in descriptor.Type.GetInterfaces())
                        if (item.IsGenericType && item.GetGenericTypeDefinition() is Type type && type == typeof(ICollection<>))
                        {

                            if (descriptor is ComponentDescriptors.PropertyObjectDescriptor prop)
                                prop.SubType = item.GetGenericArguments()[0];
                            descriptor.KindView = PropertyKindView.List;
                            descriptor.ComponentView = typeof(ComponentList);
                        }
                })
            ;

            strategy
                .ConfigureOnAttribute<DataTypeAttribute>((attribute, mapper, descriptor) =>
                {

                    if (descriptor is ComponentDescriptors.PropertyObjectDescriptor prop)
                    {

                        StrategyEditor str = null;
                        switch (attribute.DataType)
                        {

                            case DataType.DateTime:
                                if (!mapper.TryGetValueByType(typeof(DateTime), out str))
                                {
                                    descriptor.ComponentView = str.ComponentView;
                                    descriptor.KindView = str.PropertyKindView;
                                }
                                break;

                            case DataType.Time:
                                if (!mapper.TryGetValueByType(typeof(DateTime), out str))
                                {
                                    descriptor.ComponentView = str.ComponentView;
                                    descriptor.KindView = str.PropertyKindView;
                                }
                                break;

                            case DataType.Password:
                                prop.IsPassword = true;
                                descriptor.ComponentView = typeof(ComponentPassword);

                                break;

                            case DataType.Duration:
                                prop.Mask = StringType.Time;
                                break;

                            case DataType.PhoneNumber:
                                prop.Mask = StringType.Telephone;
                                break;

                            case DataType.MultilineText:
                                prop.Line = 5;
                                break;

                            case DataType.EmailAddress:
                                prop.Mask = StringType.Email;
                                break;

                            case DataType.Url:
                                prop.Mask = StringType.Url;
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

                    }
                })

                .ConfigureOnAttribute<ListProviderAttribute>((attribute, mapper, descriptor) =>
                {
                    if (descriptor is ComponentDescriptors.PropertyObjectDescriptor prop)
                        prop.ListProvider = attribute.ProviderListType;
                    descriptor.ComponentView = typeof(ComponentEnumeration);
                    descriptor.KindView = PropertyKindView.Enumeration;
                })

                .ConfigureOnAttribute<PasswordPropertyTextAttribute>((attribute, mapper, descriptor) =>
                {
                    if (descriptor is ComponentDescriptors.PropertyObjectDescriptor prop)
                        prop.IsPassword = attribute.Password;
                    descriptor.ComponentView = typeof(ComponentPassword);
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
                .ToTarget<ComponentGrid>(PropertyKindView.Object)
                .ToTarget<ComponentList>(PropertyKindView.List)
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
                .ToTarget<ComponentTime>(PropertyKindView.Time, (c, d) =>
                {
                    if (d is ComponentDescriptors.PropertyObjectDescriptor prop)
                        prop.Mask = StringType.Time;
                })
                .ToTarget<ComponentUInt16>(PropertyKindView.UInt16)
                .ToTarget<ComponentUInt32>(PropertyKindView.UInt32)
                .ToTarget<ComponentUInt64>(PropertyKindView.UInt64)
                .ToTarget<ComponentGuid>(PropertyKindView.Guid, (c, d) =>
                {
                    //d.DataFormatString = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX";
                    //= ;

                    if (d is ComponentDescriptors.PropertyObjectDescriptor prop)
                    {
                        prop.FormatString = "^[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}$";
                        prop.CreateMask = () => GuidMask.Guid();
                    }
                })
            ;

        }


    }

    public struct TransactionGrid : ITransaction
    {

        public TransactionGrid(IDtcTransaction transaction)
        {
            _rollbacked = false;
            _commited = false;
            this._transaction = transaction;
        }

        public void Abort()
        {
            _transaction?.Abort(0, 0, 0);
            _rollbacked = true;
        }


        public void Dispose()
        {

            if (_transaction != null)
            {
                if (!_rollbacked && !_commited)
                    _transaction.Abort(0, 0, 0);

                if (_transaction is IDisposable d)
                    d.Dispose();
            }

        }

        public void Commit()
        {
            _transaction?.Commit(0, 0, 0);
            _commited = true;
        }

        private IDtcTransaction _transaction;
        private bool _rollbacked;
        private bool _commited;

    }

}
