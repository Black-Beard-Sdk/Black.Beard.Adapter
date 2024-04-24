using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Data;
using System.Reflection;
using System.Text;
using static MudBlazor.Colors;


namespace Bb.Modules.Storage
{
    public abstract class SqlLiteStoreBase<TKey, TValue> : IStore<TKey, TValue>
        where TKey : struct
        where TValue : ModelBase, new()
    {

        protected const string _Uuid = "Uuid";
        protected const string _version = "Version";
        protected const string _lastUpdate = "LastUpdate";
        protected const string _inserted = "Inserted";


        public SqlLiteStoreBase(string tableName, params (string, string)[] columns)
        {

            var baseName = Assembly.GetEntryAssembly().GetName().Name.Replace(".", "_");

            _db = new SqlLite(Path.GetTempPath(), baseName);
            _table = new StoreTable(tableName);

            foreach (var item in columns)
                _table.AddColumn(item.Item1, item.Item2);

            _table.AddColumn(_version, "TEXT")
                ;

        }


        public void Initialize()
        {
            var sql = _table.CreateTable();
            _db.ExecuteNonQuery(sql);
        }



        public virtual TValue? Load(TKey key)
        {

            List<(string, object)> arguments = new List<(string, object)>()
            {
                ("@uuid", key)
            };

            var sql = _table.CreateReadOne();
            var results = Read(arguments, sql);
            return results.FirstOrDefault();

        }

        public List<TValue> Values()
        {
            List<(string, object)> arguments = new List<(string, object)>();
            var sql = _table.CreateReadAll();
            var results = Read(arguments, sql);
            return results;
        }

        private List<TValue> Read(List<(string, object)> arguments, StringBuilder sql)
        {

            List<TValue> results = new List<TValue>();

            Func<IDataReader, bool> action = reader =>
            {

                var result = Map(reader);
                results.Add(result);

                return true;
            };

            _db.ExecuteReader(sql, action, arguments.ToArray());

            return results;

        }

        public virtual void Save(TValue value)
        {

            int count = 0;

            if (value.LastUpdate.HasValue)
                count = UpdateItem(value);

            if (count == 0)
                InsertItem(value);

        }

        private int UpdateItem(TValue value)
        {
            var sql = _table.CreateUpdate();
            List<(string, object)> parameters = new List<(string, object)>()
            {
                (_Uuid, value.Uuid),
                (_version, value.Version + 1),
                ("oldVersion", value.Version),
            };
            MapParameter(value, parameters);
            var count = _db.ExecuteNonQuery(sql, parameters.ToArray());
            return count;
        }

        private int InsertItem(TValue value)
        {

            List<(string, object)> parameters = new List<(string, object)>();
            int count = -1;

            var sqlInsert = _table.CreateInsert();
            parameters = new List<(string, object)>()
                {
                    (_Uuid, value.Uuid),
                    (_version, 1),
                };

            MapParameter(value, parameters);

            count = _db.ExecuteNonQuery(sqlInsert, parameters.ToArray());

            return count;

        }

        protected abstract void MapParameter(TValue value, List<(string, object)> parameters);

        protected abstract TValue Map(IDataReader reader);

        //public bool Exist(Guid key)
        //{

        //    List<(string, object)> arguments = new List<(string, object)>()
        //    {
        //        ("@uuid", key)
        //    };

        //    var sql = _table.CreateReadOne();

        //    Func<IDataReader, bool> action = reader =>
        //    {
        //        return false;
        //    };

        //    _db.ExecuteReader(sql, action, arguments.ToArray());

        //    return false;
        //}


        //public List<(Guid, string, ModuleSpecification)> Keys()
        //{

        //    var result = new List<(Guid, string, ModuleSpecification)>();

        //    var sql = _table.CreateRead("Uuid", "Name", "Specification");

        //    Func<IDataReader, bool> action = reader =>
        //    {
        //        var module = _modulespecifications.GetModule(reader.GetGuid(2));
        //        result.Add((reader.GetGuid(0), reader.GetString(1), module));
        //        return true;
        //    };

        //    _db.ExecuteReader(sql, action);

        //    return result;

        //}


        //public void Remove(Guid key)
        //{

        //}

        //public void Save(StoreItem<ModuleInstance> value)
        //{
        //    var sql = _table.CreateUpdate();
        //}


        //public List<StoreItem<ModuleInstance>> Values()
        //{

        //    var result = new List<StoreItem<ModuleInstance>>();

        //    var keys = Keys();

        //    foreach (var item in keys)
        //    {
        //        var module = Load(item.Item1);
        //        result.Add(module);
        //    }

        //    return result;

        //}

        protected readonly IServiceProvider _provider;
        protected readonly SqlLite _db;
        protected readonly StoreTable _table;

    }


}
