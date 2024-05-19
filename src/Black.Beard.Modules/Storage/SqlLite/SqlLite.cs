using Microsoft.Data.Sqlite;
using System.Data;
using System.Text;

namespace Bb.Storage.SqlLite
{

    public class SqlLite
    {

        public SqlLite(string path, string databaseName)
        {

            _path = path.Combine(databaseName + ".db").AsFile();
            if (!_path.Directory.Exists)
                _path.Directory.Create();
            _connectionString = $"Data Source={_path.FullName}";

        }

        public int ExecuteNonQuery(StringBuilder sql, params (string, object)[] parameters)
        {
            int result = -1;

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(sql.ToString(), connection))
                {
                    MapParameter(command, parameters);
                    result = command.ExecuteNonQuery();
                }
            }

            return result;

        }

        private static void MapParameter(SqliteCommand command, params (string, object)[] parameters)
        {
            if (parameters != null)
                foreach (var item in parameters)
                    if (item.Item1.StartsWith("@"))
                        command.Parameters.AddWithValue(item.Item1, item.Item2);
                    else
                        command.Parameters.AddWithValue($"@{item.Item1}", item.Item2);
        }

        public void ExecuteReader(StringBuilder sql, Func<IDataReader, bool> action, params (string, object)[] parameters)
        {

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqliteCommand(sql.ToString(), connection))
                {

                    MapParameter(command, parameters);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var r = action(reader);
                        if (!r)
                            break;
                    }
                }

            }

        }


        private readonly FileInfo _path;
        private readonly string _connectionString;
    }

}