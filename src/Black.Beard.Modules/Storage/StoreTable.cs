using System.Text;


namespace Bb.Modules.Storage
{
    public class StoreTable
    {


        public StoreTable(string table)
        {
            _table = table;
            _columns = new List<(string, string)>();
        }


        public StoreTable AddColumn(string name, string type)
        {
            _columns.Add((name, type));
            return this;
        }


        public StringBuilder CreateTable()
        {

            var sb = new StringBuilder();
            sb.Append($"CREATE TABLE IF NOT EXISTS {_table} (Uuid TEXT PRIMARY KEY");

            foreach (var item in _columns)
                sb.Append($", {item.Item1} {item.Item2}");

            sb.Append(", Inserted TIMESTAMP DEFAULT CURRENT_TIMESTAMP, LastUpdate TIMESTAMP)");

            return sb;

        }


        public StringBuilder CreateInsert()
        {
            var sb = new StringBuilder();
            sb.Append($"INSERT INTO {_table} (Uuid");

            for (int i = 0; i < _columns.Count; i++)
                sb.Append($", @{_columns[i].Item1}");
            sb.Append($", LastUpdate)  VALUES (@uuid");

            for (int i = 0; i < _columns.Count; i++)
                sb.Append($", @{_columns[i].Item1}");

            sb.Append(", TIMESTAMP);");
            return sb;
        }


        public StringBuilder CreateUpdate()
        {
            var sb = new StringBuilder();
            sb.Append($"UPDATE {_table} SET ");
            for (int i = 0; i < _columns.Count; i++)
                sb.Append($", {_columns[i].Item1} = @{_columns[i]}");
                sb.Append(", LastUpdate = TIMESTAMP");
            sb.Append(" WHERE Uuid = @uuid AND version = @oldVersion;");
            return sb;
        }


        public StringBuilder CreateDelete()
        {
            var sb = new StringBuilder($"DELETE FROM {_table} WHERE uuid = @uuid");
            return sb;
        }

        public StringBuilder CreateReadOne()
        {
            var sb = new StringBuilder();
            sb.Append("SELECT Uuid");
            for (int i = 0; i < _columns.Count; i++)
                sb.Append($", {_columns[i].Item1}");
            sb.Append(" FROM {_table} WHERE Uuid = @uuid");
            return sb;
        }

        public StringBuilder CreateReadAll()
        {
            var sb = new StringBuilder();
            sb.Append("SELECT Uuid");
            for (int i = 0; i < _columns.Count; i++)
                sb.Append($", {_columns[i].Item1}");
            sb.Append(" FROM {_table}");
            return sb;
        }

        internal StringBuilder CreateRead(params string[] items)
        {
            var sb = new StringBuilder();
            sb.Append("SELECT ");
            sb.Append(items[0]);
            for (int i = 1; i < _columns.Count; i++)
                sb.Append($", {items[i]}");
            sb.Append(" FROM {_table}");
            return sb;
        }

        private readonly string _table;
        private List<(string, string)> _columns;

    }


}
