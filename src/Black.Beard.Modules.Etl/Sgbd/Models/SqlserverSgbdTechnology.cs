﻿using Bb.ComponentModel.Attributes;
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

            config.Add<SgbdDiagram>(c =>
            {

                c.AddProperty("Schema", typeof(string), i =>
                {
                    i.PropertyOrder(3)
                    ;
                });
            });

            config.Add<Table>(c =>
            {

                c.AddProperty("Schema", typeof(string), i =>
                {
                    i.PropertyOrder(3)
                    ;
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
                var type = d.GetColumnType();
                if (type != null)
                    return type.Category == ColumbTypeCategory.Integer;
                return false;

            });

            config.Add<Column>(c =>
            {

                c.AddProperty("Scale", typeof(int), i =>
                {
                    i.IsBrowsable(true)
                    .Description("count of digit after comma")
                    .DefaultValue(10)
                    ;
                });
            }, d =>
            {
                var type = d.GetColumnType();
                if (type != null)
                    return type.Category == ColumbTypeCategory.Number;
                return false;
            });

            config.Add<Column>(c =>
            {

                c.AddProperty("Precision", typeof(int), i =>
                {
                    i.IsBrowsable(true)
                    .Description("Count of digit before comma")
                    .DefaultValue(8)
                    ;
                });
            }, d =>
            {
                var type = d.GetColumnType();
                if (type != null)
                    return (type.Category == ColumbTypeCategory.Date)
                        || (type.Category == ColumbTypeCategory.Number);
                return false;
            });

            config.Add<Column>(c =>
            {
                c.AddProperty("Lenght", typeof(int), i =>
                {
                    i.IsBrowsable(true)
                    .Description("String lenght. -1 value is max")
                    .DefaultValue(50)
                    ;
                })
                 ;
            }, d =>
            {
                var type = d.GetColumnType();
                if (type != null)
                    return type.Category == ColumbTypeCategory.String;
                return false;

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
            AddColumnType("Binary", "binary", ColumbTypeCategory.Binary);
            AddColumnType("Boolean (bit)", "bit", ColumbTypeCategory.Boolean);
            AddColumnType("Char", "char", ColumbTypeCategory.String);
            AddColumnType("DateTime", "datetime", ColumbTypeCategory.Date);
            AddColumnType("DateTimeOffset", "datetimeoffset", ColumbTypeCategory.Date);
            AddColumnType("Date", "date", ColumbTypeCategory.Date);
            AddColumnType("DateTime2", "datetime2", ColumbTypeCategory.Date);
            AddColumnType("Decimal", "decimal", ColumbTypeCategory.Number);
            AddColumnType("Double", "double", ColumbTypeCategory.Number);
            AddColumnType("Float", "float", ColumbTypeCategory.Number);
            AddColumnType("Geography", "geography", ColumbTypeCategory.Other);
            AddColumnType("Geometry", "geometry", ColumbTypeCategory.Other);
            AddColumnType("HierarchyId", "hierarchyid", ColumbTypeCategory.Other);
            AddColumnType("Image", "image", ColumbTypeCategory.Other);
            IsDefaultColumnType(AddColumnType("Integer", "int", ColumbTypeCategory.Integer));
            AddColumnType("Json", "json", ColumbTypeCategory.Other);
            AddColumnType("Long", "bigint", ColumbTypeCategory.Integer);
            AddColumnType("Money", "money", ColumbTypeCategory.Number);
            AddColumnType("NText", "ntext", ColumbTypeCategory.String);
            AddColumnType("RowVersion", "rowversion", ColumbTypeCategory.Other);
            AddColumnType("Tinyint", "tinyint", ColumbTypeCategory.Integer);
            AddColumnType("Short", "smallint", ColumbTypeCategory.Integer);
            AddColumnType("SmallDateTime", "smalldatetime", ColumbTypeCategory.Date);
            AddColumnType("SmallMoney", "smallmoney", ColumbTypeCategory.Number);
            AddColumnType("Varchar", "varchar", ColumbTypeCategory.String);
            AddColumnType("Timestamp", "timestamp", ColumbTypeCategory.Date);
            AddColumnType("SqlVariant", "sql_variant", ColumbTypeCategory.Other);
            AddColumnType("Text", "text", ColumbTypeCategory.String);
            AddColumnType("Time", "time", ColumbTypeCategory.Date);
            AddColumnType("UniqueIdentifier", "uniqueidentifier", ColumbTypeCategory.UUid);
            AddColumnType("Variant", "sql_variant", ColumbTypeCategory.Binary);
            AddColumnType("VarBinary", "varbinary", ColumbTypeCategory.Binary);
            AddColumnType("Xml", "xml", ColumbTypeCategory.Other);
        }


    }

}
