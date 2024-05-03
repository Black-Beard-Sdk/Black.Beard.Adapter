using Bb.ComponentModel.Accessors;
using System.Text;
using static MudBlazor.CategoryTypes;


namespace Bb.Modules.Storage
{


    public class StoreTable
    {

        public StoreTable(string table)
        {
            _table = table;
            _columns = new List<TableField>();

        }

        public StoreTable AddColumn(TableField field)
        {
            _columns.Add(field);
            return this;
        }

        public StringBuilder CreateTable()
        {

            var sb = new StringBuilder();
            sb.Append($"CREATE TABLE IF NOT EXISTS {_table} (");

            string comma = string.Empty;
            foreach (var item in _columns)
            {

                sb.Append($"{comma}{item.Name} {item.Type}");

                if (!string.IsNullOrEmpty(item.DefaultValue))
                    sb.Append($" DEFAULT {item.DefaultValue}");

                if (item.IsPrimary)
                    sb.Append(" PRIMARY KEY");
                comma = ", ";
            }

            sb.Append(");");

            return sb;

        }


        public StringBuilder CreateInsert()
        {

            string comma = string.Empty;

            var sb = new StringBuilder();
            sb.Append($"INSERT INTO {_table} (");

            for (int i = 0; i < _columns.Count; i++)
            {
                sb.Append($"{comma}{_columns[i].Name}");
                comma = ", ";
            }

            sb.Append($") VALUES (");

            comma = string.Empty;
            foreach (var col in _columns)
            {
                if (!string.IsNullOrEmpty(col.DefaultValue) && (col.UpdateHisto || col.InsertHisto))
                    sb.Append($"{comma}{col.DefaultValue}");
                else
                    sb.Append($"{comma}@{col.Variable}");
                comma = ", ";
            }

            sb.Append(");");

            return sb;
        }


        public StringBuilder CreateUpdate()
        {
            var sb = new StringBuilder();
            sb.Append($"UPDATE {_table} SET ");

            string comma = string.Empty;

            foreach (var col in _columns.Where(c => !c.InsertHisto))
            {
                sb.Append($"{comma}{col.Name} = @{col.Variable}");
                comma = ", ";
            }

            sb.Append(" WHERE");

            comma = string.Empty;


            foreach (var col in _columns.Where(c => c.IsPrimary | c.CheckIntegrity))
            {

                if (col.IsPrimary)
                    sb.Append($"{comma} {col.Name} = @{col.Variable}");

                if (!string.IsNullOrEmpty(col.DefaultValue) && col.UpdateHisto)
                    sb.Append($"{comma}{col.DefaultValue}");

                else if (col.CheckIntegrity)
                    sb.Append($"{comma} {col.Name} = @old_{col.Variable}");

                else
                {

                }

                comma = " AND";
            }

            sb.Append(";");

            return sb;
        }


        public StringBuilder CreateDelete()
        {

            string comma = string.Empty;
            var sb = new StringBuilder($"DELETE FROM {_table} WHERE");
            foreach (var col in _columns.Where(c => c.IsPrimary))
            {
                sb.Append($"{comma} {col.Name} = @{col.Variable}");
                comma = " AND";
            }
            return sb;
        }

        public StringBuilder CreateReadOne()
        {
            var sb = CreateReadAll();

            sb.Append($" WHERE");

            string comma = string.Empty;
            foreach (var col in _columns.Where(c => c.IsPrimary))
            {
                sb.Append($"{comma} {col.Name} = @{col.Variable}");
                comma = " AND";
            }

            return sb;
        }

        public StringBuilder CreateReadAll()
        {
            var sb = new StringBuilder();
            sb.Append("SELECT ");

            string comma = string.Empty;
            for (int i = 0; i < _columns.Count; i++)
            {
                sb.Append($"{comma}{_columns[i].Name}");
                comma = ", ";
            }

            sb.Append($" FROM {_table}");

            return sb;
        }

        private readonly string _table;
        private List<TableField> _columns;

    }


    [System.Diagnostics.DebuggerDisplay("{Name} {Type}")]
    public class TableField
    {


        public TableField(string name, string Type)
        {
            this.Name = name;
            this.Type = Type;
        }

        public TableField(AccessorItem property)
        {
            Accessor = property;
            Name = property.Name;


            var type = property.Type;

            if (type.IsGenericType)
                type = property.Type.GenericTypeArguments[0];

            if (type == typeof(string))
                Type = "TEXT";

            else if (type == typeof(Guid))
                Type = "TEXT";

            else if (type == typeof(int))
                Type = "INTEGER";

            else if (type == typeof(DateTime) || type == typeof(DateTimeOffset))
                Type = "TIMESTAMP";


            else
            {

            }

        }

        public AccessorItem Accessor { get; }

        public string Name { get; }

        public string Variable { get => Name.ToLower(); }

        public string Type { get; }

        public bool IsPrimary { get; set; }

        public bool NotNull { get; set; }

        public string DefaultValue { get; set; }

        public bool CheckIntegrity { get; internal set; }

        public bool UpdateHisto { get; internal set; }
        public bool IsPayload { get; internal set; }
        public int Order { get; internal set; }
        public bool InsertHisto { get; internal set; }
    }


}
