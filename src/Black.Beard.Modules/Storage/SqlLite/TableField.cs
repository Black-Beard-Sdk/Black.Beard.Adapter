using Bb.ComponentModel.Accessors;
using System.Text;


namespace Bb.Storage.SqlLite
{
    [System.Diagnostics.DebuggerDisplay("{Name} {Type}")]
    public class TableField
    {


        public TableField(string name, string Type)
        {
            Name = name;
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

        public void AddNotNull(StringBuilder sb)
        {
            sb.Append($" ALTER COLUMN {Name} {Type}");

            if (NotNull)
                sb.Append(" NOT NULL");

            if (IsPrimary)
                sb.Append(" PRIMARY KEY");

            if (DefaultValue != null)
                sb.Append($" DEFAULT {DefaultValue}");

        }

        public void AddColumn(StringBuilder sb)
        {
            sb.Append($" ADD COLUMN {Name} {Type}");

            //if (NotNull)
            //    sb.Append(" NOT NULL");

            //if (IsPrimary)
            //    sb.Append(" PRIMARY KEY");

            if (DefaultValue != null)
                sb.Append($" DEFAULT {DefaultValue}");

        }

        public AccessorItem Accessor { get; }

        public string Name { get; }

        public string Variable { get => Name.ToLower(); }

        public string Type { get; }

        public bool IsPrimary { get; set; }

        public bool NotNull { get; set; }

        public string DefaultValue { get; set; }

        public bool OptimistLock { get; internal set; }

        public bool UpdateHisto { get; internal set; }

        public bool IsPayload { get; internal set; }

        public int Order { get; internal set; }

        public bool InsertHisto { get; internal set; }

        public Type TypeInitializeColumn { get; internal set; }

        internal void Parse(Queue<string> sql)
        {

            while (sql.Count > 0)
            {
                var item = sql.Dequeue();

                switch (item)
                {

                    case "PRIMARY":
                        IsPrimary = true;
                        sql.Dequeue();
                        break;

                    case "NOT":
                        NotNull = true;
                        sql.Dequeue();
                        break;

                    case "DEFAULT":
                        DefaultValue = sql.Dequeue();
                        break;


                    default:
                        break;
                }

            }

        }
    }


}
