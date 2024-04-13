using Bb.ComponentModel.Attributes;
using System.Data;


namespace Bb.Modules.Storage
{



    [ExposeClass("Service", ExposedType = typeof(IStore<Guid, ModuleInstance>), LifeCycle = IocScopeEnum.Singleton)]
    public class SqlLiteStore : IStore<Guid, ModuleInstance>
    {


        private string _name = "Name";
        private string _specification = "Specification";
        private string _version = "Version";
        private string _data = "Data";
        private string _Uuid = "Data";
        private string _lastUpdate = "LastUpdate";


        public SqlLiteStore(ModuleSpecifications specifications)
        {

            this._modulespecifications = specifications;            
            _db = new SqlLite(Path.GetTempPath(), "adapter");
            _table = new StoreTable("Modules")
                .AddColumn(_name, "TEXT")
                .AddColumn(_specification, "TEXT")
                .AddColumn(_version, "TEXT")
                .AddColumn(_data, "TEXT")
                ;

        }


        


        public bool Exist(Guid key)
        {

            List<(string, object)> arguments = new List<(string, object)>()
            {
                ("@uuid", key)
            };

            var sql = _table.CreateReadOne();

            Func<IDataReader, bool> action = reader =>
            {
                return false;
            };

            _db.ExecuteReader(sql, action, arguments.ToArray());

            return false;
        }


        public List<(Guid, string, ModuleSpecification)> Keys()
        {

            var result = new List<(Guid, string, ModuleSpecification)>();

            var sql = _table.CreateRead("Uuid", "Name", "Specification");

            Func<IDataReader, bool> action = reader =>
            {
                var module = _modulespecifications.GetModule(reader.GetGuid(2));
                result.Add((reader.GetGuid(0), reader.GetString(1), module));
                return true;
            };

            _db.ExecuteReader(sql, action);

            return result;

        }

        public StoreItem<ModuleInstance> Load(Guid key)
        {

            List<(string, object)> arguments = new List<(string, object)>()
            {
                ("@uuid", key)
            };

            var sql = _table.CreateReadOne();

            StoreItem<ModuleInstance> result = null;

            Func<IDataReader, bool> action = reader =>
            {
                var instance = new ModuleInstance(reader.GetGuid(0), this);
                result = new StoreItem<ModuleInstance>(instance, reader.GetGuid(2));

                return true;
            };

            _db.ExecuteReader(sql, action, arguments.ToArray());

            return result;

        }

        public void Remove(Guid key)
        {

        }

        public void Save(StoreItem<ModuleInstance> value)
        {

            var sql = _table.CreateInsert();


        }

        public List<StoreItem<ModuleInstance>> Values()
        {

            var result = new List<StoreItem<ModuleInstance>>();

            var keys = Keys();

            foreach (var item in keys)
            {
                var module = Load(item.Item1);
                result.Add(module);
            }

            return result;

        }

        private readonly IServiceProvider _provider;
        private readonly ModuleSpecifications _modulespecifications;
        private readonly SqlLite _db;
        private readonly StoreTable _table;

    }


}
