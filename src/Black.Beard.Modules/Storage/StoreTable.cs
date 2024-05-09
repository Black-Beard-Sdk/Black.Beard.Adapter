using ICSharpCode.Decompiler.IL;
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



        public static IEnumerable<TableField> Parse(string sql)
        {
            List<TableField> result = new List<TableField>();

            var s = sql.IndexOf('(');
            var e = sql.IndexOf(')', s);
            var txt = sql.Substring(s, e - s).Trim(' ', '(', ')');
            var fields = txt.Split(',');
            foreach (var item in fields)
            {
                var f = item.Trim();
                var p = f.Split(' ');
                var name = p[0];
                var type = p[1];
                var field = new TableField(name, type);
                field.Parse(new Queue<string>(p.Skip(2)));
                result.Add(field);
            }

            return result;
        }


        public StringBuilder GetTableStructure()
        {
            var sb = new StringBuilder("SELECT sql FROM sqlite_master WHERE name = @name AND type = 'table'");
            return sb;
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


            foreach (var col in _columns.Where(c => c.IsPrimary | c.OptimistLock))
            {

                if (col.IsPrimary)
                    sb.Append($"{comma} {col.Name} = @{col.Variable}");

                if (!string.IsNullOrEmpty(col.DefaultValue) && col.UpdateHisto)
                    sb.Append($"{comma}{col.DefaultValue}");

                else if (col.OptimistLock)
                    sb.Append($"{comma} {col.Name} = @old_{col.Variable}");

                comma = " AND";
            }

            sb.Append(";");

            return sb;
        }

        public StringBuilder CreateExists()
        {

            var sb = new StringBuilder();

            sb.Append($"SELECT 1 FROM {_table} WHERE");

            string comma = string.Empty;
            foreach (var col in _columns.Where(c => c.IsPrimary))
            {
                sb.Append($"{comma} {col.Name} = @{col.Variable}");
                comma = " AND";
            }

            return sb;

        }


        public void Where(StringBuilder sb, params (string, object)[] parameters)
        {

            string comma = string.Empty;

            sb.AppendLine(" WHERE");
            foreach (var col in parameters)
            {
                sb.Append($"{comma} {col.Item1} = @{col.Item1.ToLower()}");
                comma = " AND";
            }


        }

        public StringBuilder CreateDelete(bool withDeleteOnPrimaryKey)
        {

            string comma = string.Empty;
            var sb = new StringBuilder($"DELETE FROM {_table}");

            if (withDeleteOnPrimaryKey)
            {
                sb.AppendLine(" WHERE");
                foreach (var col in _columns.Where(c => c.IsPrimary))
                {
                    sb.Append($"{comma} {col.Name} = @{col.Variable}");
                    comma = " AND";
                }
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

        internal StringBuilder AlterTable()
        {
            var sql = new StringBuilder($"ALTER TABLE {_table}");
            return sql;
        }

        public string TableName => _table;

        private readonly string _table;
        private List<TableField> _columns;

    }


}
