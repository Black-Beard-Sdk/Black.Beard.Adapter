using Bb.ComponentModel.Attributes;
using Bb.TypeDescriptors;
using System.ComponentModel.DataAnnotations;

namespace Bb.Modules.Sgbd.Models
{

    public static class SqlserverConstants
    {

        public const string NameConstraint = @"^[\p{L}_][\p{L}\p{N}@$#_]{0,127}$";

    }



    [ExposeClass(ComponentModel.ConstantsCore.Plugin, ExposedType = typeof(SgbdTechnology), LifeCycle = IocScopeEnum.Transiant)]
    public class SqlserverSgbdTechnology : SgbdTechnology
    {



        public SqlserverSgbdTechnology()
            : base("Sqlserver", "Microsoft Sqlserver")
        {

            AddTypes();

            var config = DynamicTypeDescriptionProvider.Configuration;

            config.Add<Table>(c =>
            {

                c.Property("Name", i =>
                {
                    i.AddAttributes(new RegularExpressionAttribute(SqlserverConstants.NameConstraint) { ErrorMessage = DatasComponentConstants.TableValidationMessage });
                });

            });

            config.Add<Column>(c =>
            {
                c.AddProperty("AutoIncrement", typeof(bool), i =>
                {
                    i.IsBrowsable(true)
                    .Description("Auto increment")
                    ;
                })
                 .AddProperty("IncrementStart", typeof(int), i =>
                {
                    i.IsBrowsable(true)
                    .Description("Auto start")
                    .DefaultValue(1)
                    
                    ;
                })
                 .AddProperty("IncrementStep", typeof(int), i =>
                {
                    i.IsBrowsable(true)
                    .Description("increment step")
                    .DefaultValue(1)
                    ;
                });
            }, d =>
            {
                return true;
            });

            config.Add<Index>(c =>
            {
                // c.AddProperty("Year", typeof(int), i =>
                // {
                //     i.IsBrowsable(true)
                //     .CanResetValue(true)
                //     ;
                // });
            });

            config.Add<ColumnIndex>(c =>
            {
                // c.AddProperty("Year", typeof(int), i =>
                // {
                //     i.IsBrowsable(true)
                //     .CanResetValue(true)
                //     ;
                // });
            });
        }


        private void AddTypes()
        {
            AddColumnType("Binary", "binary");
            AddColumnType("Boolean", "bit");
            AddColumnType("Char", "char");
            AddColumnType("DateTime", "datetime");
            AddColumnType("DateTimeOffset", "datetimeoffset");
            AddColumnType("Date", "date");
            AddColumnType("DateTime2", "datetime2");
            AddColumnType("Decimal", "decimal");
            AddColumnType("Double", "double");
            AddColumnType("Float", "float");
            AddColumnType("Geography", "geography");
            AddColumnType("Geometry", "geometry");
            AddColumnType("Guid", "uniqueidentifier");
            AddColumnType("HierarchyId", "hierarchyid"); AddColumnType("Time", "time");
            AddColumnType("Image", "image");
            IsDefaultColumnType(AddColumnType("Integer", "int"));
            AddColumnType("Json", "json");
            AddColumnType("Long", "bigint");
            AddColumnType("Money", "money");
            AddColumnType("NText", "ntext");
            AddColumnType("RowVersion", "rowversion");
            AddColumnType("SByte", "tinyint");
            AddColumnType("Short", "smallint");
            AddColumnType("SmallDateTime", "smalldatetime");
            AddColumnType("SmallMoney", "smallmoney");
            AddColumnType("String", "varchar");
            AddColumnType("Timestamp", "timestamp");
            AddColumnType("SqlVariant", "sql_variant");
            AddColumnType("Text", "text");
            AddColumnType("UniqueIdentifier", "uniqueidentifier");
            AddColumnType("Variant", "sql_variant");
            AddColumnType("VarBinary", "varbinary");
            AddColumnType("Xml", "xml");
        }


    }

}
