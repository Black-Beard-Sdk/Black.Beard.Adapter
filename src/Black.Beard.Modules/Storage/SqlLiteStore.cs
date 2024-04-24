using Bb.ComponentModel.Attributes;
using System.Data;


namespace Bb.Modules.Storage
{



    [ExposeClass("Service", ExposedType = typeof(IStore<Guid, ModuleInstance>), LifeCycle = IocScopeEnum.Singleton)]
    public class SqlLiteStore : SqlLiteStoreBase<Guid, ModuleInstance>
    {


        private const string _name = "Name";
        private const string _specification = "Specification";
        private const string _data = "Data";


        public SqlLiteStore(ModuleSpecifications specifications)
            : base("Modules", (_name, "TEXT"), (_specification, "TEXT"), (_data, "TEXT"))
        {

            this._modulespecifications = specifications;

        }

        protected override ModuleInstance Map(IDataReader reader)
        {

            var payload = reader.GetString(reader.GetOrdinal(_data));
            var instance = payload.Deserialize<ModuleInstance>();

            instance.Uuid = reader.GetGuid(reader.GetOrdinal(_Uuid));
            instance.Version = reader.GetInt32(reader.GetOrdinal(_version));
            instance.LastUpdate = reader.GetDateTime(reader.GetOrdinal(_lastUpdate));
            instance.Inserted = reader.GetDateTime(reader.GetOrdinal(_inserted));

            return instance;

        }


        protected override void MapParameter(ModuleInstance value, List<(string, object)> parameters)
        {

            parameters.Add((_name, value.Label));
            parameters.Add((_specification, value.Specification));
            parameters.Add((_data, value.Serialize(false)));

        }



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

        //public StoreItem<ModuleInstance> Load(Guid key)
        //{

        //    List<(string, object)> arguments = new List<(string, object)>()
        //    {
        //        ("@uuid", key)
        //    };

        //    var sql = _table.CreateReadOne();

        //    StoreItem<ModuleInstance> result = null;

        //    Func<IDataReader, bool> action = reader =>
        //    {
        //        //var instance = new ModuleInstance(reader.GetGuid(0), this);
        //        //result = new StoreItem<ModuleInstance>(instance, reader.GetGuid(2));

        //        return true;
        //    };

        //    _db.ExecuteReader(sql, action, arguments.ToArray());

        //    return result;

        //}

        //public void Remove(Guid key)
        //{

        //}

        //public void Save(StoreItem<ModuleInstance> value)
        //{

        //    var sql = _table.CreateUpdate();


        //}

        //public void Save(ModuleInstance value)
        //{


        //    var item = Load(value.Uuid);
        //    if (item == null)
        //    {
        //        var sql = _table.CreateInsert();
        //        var payload = value.Serialize(false);
        //        var arguments = new List<(string, object)>()
        //        {
        //            (_Uuid, value.Uuid),
        //            (_name, value.Label),
        //            (_specification, value.Specification),
        //            (_version, 1),
        //            (_data, payload),
        //        };

        //        this._db.ExecuteNonQuery(sql, arguments.ToArray());

        //        item = Load(value.Uuid);
        //        value.Version = item.Version
        //    }
        //    else
        //    {

        //    }

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

        //private readonly IServiceProvider _provider;
        private readonly ModuleSpecifications _modulespecifications;

        //private readonly SqlLite _db;
        //private readonly StoreTable _table;

    }


}
