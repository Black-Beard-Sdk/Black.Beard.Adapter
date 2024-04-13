using Microsoft.Data.Sqlite;
using System.Data;
using System.Text;

namespace Bb.Modules.Storage
{
    public class SqlLite
    {

        public SqlLite(string path, string databaseName)
        {

            _path = path.Combine(databaseName + ".db").AsFile();
            if (!_path.Directory.Exists)
                _path.Directory.Create();
            _connectionString = $"Data Source={_path.FullName};Version=3;";

        }

        public int ExecuteNonQuery(StringBuilder sql, params (string, object)[] paramters)
        {
            int result = -1;

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(sql.ToString(), connection))
                {

                    foreach (var item in paramters)
                        command.Parameters.AddWithValue(item.Item1, item.Item2);

                    result = command.ExecuteNonQuery();

                }
            }

            return result;

        }

        public void ExecuteReader(StringBuilder sql, Func<IDataReader, bool> action, params (string, object)[] paramters)
        {

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqliteCommand(sql.ToString(), connection))
                {

                    foreach (var item in paramters)
                        command.Parameters.AddWithValue(item.Item1, item.Item2);

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