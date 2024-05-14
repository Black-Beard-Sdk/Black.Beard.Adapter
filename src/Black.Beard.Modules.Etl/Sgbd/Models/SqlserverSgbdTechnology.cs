using Bb.ComponentModel.Attributes;

namespace Bb.Modules.Sgbd.Models
{


    [ExposeClass(ComponentModel.ConstantsCore.Plugin, ExposedType = typeof(SgbdTechnology), LifeCycle = IocScopeEnum.Transiant)]
    public class SqlserverSgbdTechnology : SgbdTechnology
    {


        public SqlserverSgbdTechnology()
            : base("Sqlserver", "Microsoft Sqlserver")
        {
            AddTypes();

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
            IsDefaultColumnType( AddColumnType("Integer", "int"));
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
