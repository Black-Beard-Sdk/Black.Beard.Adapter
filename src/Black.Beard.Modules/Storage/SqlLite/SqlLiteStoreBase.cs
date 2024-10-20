using Bb.ComponentModel.Accessors;
using Bb.Expressions;
using Bb.Modules;
using Bb.Storage.SqlLite;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;


namespace Bb.Storage
{


    public abstract class SqlLiteStoreBase<TKey, TValue> 
        : IStore<TKey, TValue>
        where TKey : struct
        where TValue : ModelBase<TKey>, new()
    {


        public SqlLiteStoreBase(IConfiguration configuration, string connectionStringName, string tableName, params TableField[] columns)
        {

            var  connectionString = configuration.GetConnectionString(connectionStringName);

            _db = new SqlLiteCommand(connectionString);
            _table = new StoreTable(tableName);

            if (columns.Length == 0)
                columns = GetColumns();

            foreach (var item in columns)
                _table.AddColumn(item);

            _columns = columns;

        }

        public string IndexName => _table.TableName;


        public void Initialize()
        {
            var sql = _table.CreateTable();
            var count = _db.ExecuteNonQuery(sql);
            if (count == 0)
                InitializeAlterTable();

        }

        private void InitializeAlterTable()
        {

            List<TableField> fields = new List<TableField>();
            StringBuilder sql = _table.GetTableStructure();

            _db.ExecuteReader(sql, c =>
            {
                var sqlCreate = c.GetString(0);
                fields = StoreTable.Parse(sqlCreate).ToList();
                return true;
            }, ("name", _table.TableName));


            List<TableField> _added = new List<TableField>();

            foreach (var c in _columns)
                if (!fields.Any(d => d.Name == c.Name))
                {
                    sql = _table.AlterTable();
                    c.AddColumn(sql);
                    _db.ExecuteNonQuery(sql);
                    _added.Add(c);
                }

            if (_added.Count > 0)
            {

                var values = Index();
                foreach (var item in values)
                {

                    foreach (var c in _added)
                    {
                        var init = (IInitializeColumn<TKey, TValue>)Activator.CreateInstance(c.TypeInitializeColumn);
                        var changed = init.InitializeColumns(item);
                        if (changed)
                            Save(item);
                    }

                }

                foreach (var c in _added)
                {
                    if (c.NotNull)
                    {
                        sql = _table.AlterTable();
                        c.AddNotNull(sql);
                        _db.ExecuteNonQuery(sql);
                    }
                }

            }

        }

        public virtual bool Exists(TKey key)
        {
            var sql = _table.CreateExists();
            bool exists = false;
            _db.ExecuteReader(sql, c =>
            {
                exists = true;
                return false;
            }, (_Uuid.ToLower(), key));
            return exists;
        }

        public virtual TValue? Load(TKey key)
        {
            var sql = _table.CreateReadOne();
            var results = Read(sql, (_Uuid.ToLower(), key));
            return results.FirstOrDefault();

        }

        public IEnumerable<TValue> Index()
        {
            var sql = _table.CreateReadAll();
            var results = Read(sql);
            return results;
        }

        public IEnumerable<TKey> Keys()
        {
            var sql = _table.CreateReadKey();
            var results = ReadKey(sql);
            return results;
        }

        public virtual bool RemoveKey(TKey key)
        {

            var sql = _table.CreateDelete(true);
            var results = _db.ExecuteNonQuery(sql, (_Uuid.ToLower(), key));

            return results > 0;

        }


        public int Remove((string, object) parameter)
        {
            var sql = _table.CreateDelete(false);
            _table.Where(sql, parameter);
            var results = _db.ExecuteNonQuery(sql, (parameter.Item1.ToLower(), parameter.Item2));
            return results;
        }


        public virtual void Save(TValue value)
        {

            int count = 0;

            if (Exists(value.Uuid) && value.LastUpdate.HasValue)
            {

                count = UpdateItem(value);
                if (count > 0)
                {
                    value.Version++;
                }

            }
            else
                InsertItem(value);

        }


        protected virtual List<(string, object)> MapParameter(HashSet<string> expectedParameters, TValue instance)
        {


            List<(string, object)> parameters = new List<(string, object)>();


            foreach (var item in _columns.Where(c => c.OptimistLock))
            {

                var value = item.Accessor.GetValue(instance);

                if (expectedParameters.Contains("old_" + item.Variable))
                    parameters.Add(("old_" + item.Variable, value));

                if (expectedParameters.Contains(item.Variable))
                    if (item.Accessor != null)
                    {

                        if (item.Accessor.Type == typeof(int))
                        {
                            if (value == null || (int)value == 0)
                                parameters.Add((item.Variable, 1));
                            else
                                parameters.Add((item.Variable, ((int)value) + 1));
                        }

                        else if (item.Accessor.Type == typeof(DateTimeOffset))
                            parameters.Add((item.Variable, DateTime.Now));

                        else if (item.Accessor.Type == typeof(string))
                            parameters.Add((item.Variable, Guid.NewGuid().ToString()));

                        else if (item.Accessor.Type == typeof(Guid))
                            parameters.Add((item.Variable, Guid.NewGuid()));

                        else
                        {

                        }

                    }


            }


            foreach (var item in _columns)
                if (!parameters.Any(c => c.Item1 == item.Variable))
                    if (expectedParameters.Contains(item.Variable))
                    {
                        if (item.IsPayload)
                            parameters.Add((item.Variable.ToLower(), instance.Serialize(false)));
                        else
                            parameters.Add((item.Variable.ToLower(), item.Accessor.GetValue(instance)));
                    }

            return parameters;

        }

        protected HashSet<string> GetExpectedParameters(StringBuilder sql)
        {
            HashSet<string> expectedParameters = new HashSet<string>();
            var p = new Regex("@[a-zA-Z0-9_]+").Matches(sql.ToString()).Select(c => c.Value);
            foreach (var item in p)
                expectedParameters.Add(item.Substring(1));
            return expectedParameters;
        }

        protected virtual TValue MapInstance(IDataReader reader)
        {

            var payload = reader.GetString(reader.GetOrdinal(_data));
            var instance = payload.Deserialize<TValue>();

            foreach (var item in _columns.Where(c => !c.IsPayload && !c.IsPrimary))
            {

                object value;

                var index = reader.GetOrdinal(item.Name);
                var type = item.Accessor.Type;

                value = ConverterHelper.ToObject(ReadValue(reader, index, type), type);
                item.Accessor.SetValue(instance, value);

            }

            return instance;

        }

        protected virtual IEnumerable<TKey> MapKeys(IDataReader reader)
        {

            List<TKey> keys = new List<TKey>();
            foreach (var item in _columns.Where(c => !c.IsPayload && !c.IsPrimary))
            {              
                var index = reader.GetOrdinal(item.Name);
                var type = item.Accessor.Type;
                keys.Add((TKey)ConverterHelper.ToObject(ReadValue(reader, index, typeof(TKey)), typeof(TKey)));
            }

            return keys;

        }

        private object ReadValue(IDataReader reader, int index, Type type)
        {

            object value;

            var nameType = type.IsGenericType
                    ? type.GetGenericArguments()[0].Name
                    : type.Name;

            switch (nameType)
            {
                case "DateTimeOffset":
                    value = new DateTimeOffset(reader.GetDateTime(index));
                    break;
                case "DateTime":
                    value = reader.GetDateTime(index);
                    break;
                case "Int16":
                    value = reader.GetInt16(index);
                    break;
                case "Int32":
                    value = reader.GetInt32(index);
                    break;
                case "Int64":
                    value = reader.GetInt64(index);
                    break;
                case "Byte":
                    value = reader.GetByte(index);
                    break;
                case "Char":
                    value = reader.GetChar(index);
                    break;
                case "Double":
                    value = reader.GetDouble(index);
                    break;
                case "Float":
                    value = reader.GetFloat(index);
                    break;
                case "Decimal":
                    value = reader.GetDecimal(index);
                    break;
                case "String":
                    value = reader.GetString(index);
                    break;
                case "Guid":
                    value = reader.GetGuid(index);
                    break;
                case "Boolean":
                    value = reader.GetBoolean(index);
                    break;
                default:
                    value = reader.GetValue(index);
                    break;
            }

            return value;

        }

        private int UpdateItem(TValue value)
        {
            var sql = _table.CreateUpdate();
            var parameters = MapParameter(GetExpectedParameters(sql), value);
            var count = _db.ExecuteNonQuery(sql, parameters.ToArray());
            return count;
        }


        private int InsertItem(TValue value)
        {
            var sql = _table.CreateInsert();
            var parameters = MapParameter(GetExpectedParameters(sql), value);
            var count = _db.ExecuteNonQuery(sql, parameters.ToArray());
            return count;
        }


        private List<TValue> Read(StringBuilder sql, params (string, object)[] arguments)
        {

            List<TValue> results = new List<TValue>();

            Func<IDataReader, bool> action = reader =>
            {

                var result = MapInstance(reader);
                results.Add(result);

                return true;
            };

            _db.ExecuteReader(sql, action, arguments);

            return results;

        }

        private List<TKey> ReadKey(StringBuilder sql, params (string, object)[] arguments)
        {

            List<TKey> results = new List<TKey>();

            Func<IDataReader, bool> action = reader =>
            {
                var result = MapKeys(reader);
                results.Add(result.FirstOrDefault());
                return true;
            };

            _db.ExecuteReader(sql, action, arguments);

            return results;

        }


        public static TableField[] GetColumns()
        {

            List<TableField> _items = new List<TableField>();
            var properties = PropertyAccessor.GetProperties(typeof(TValue), AccessorStrategyEnum.Direct);

            foreach (AccessorItem property in properties)
            {

                var o = new TableField(property);

                var attribute = property.Member.GetCustomAttributes(true).OfType<StoreDescriptorAttribute>().FirstOrDefault();

                if (attribute != null)
                {

                    o.Order = attribute.Order;

                    if (attribute.Externalize)
                    {

                        _items.Add(o);

                        if (attribute.IsPrimary)
                            o.IsPrimary = true;

                        else if (attribute.InsertHisto)
                        {
                            o.InsertHisto = true;
                            o.DefaultValue = "CURRENT_TIMESTAMP";
                        }

                        else if (attribute.UpdateHisto)
                        {
                            o.UpdateHisto = true;
                            o.DefaultValue = "CURRENT_TIMESTAMP";
                        }

                        else if (attribute.CheckIntegrity)
                            o.OptimistLock = true;

                        o.TypeInitializeColumn = attribute.TypeInitializeColumn;

                    }
                }

            }

            _items.Add(new TableField("Data", "TEXT") { IsPayload = true, Order = _items.Count + 1 });

            return _items.OrderBy(c => c.Order).ToArray();

        }


        protected const string _data = "Data";
        protected readonly IServiceProvider _provider;
        protected readonly SqlLiteCommand _db;
        protected readonly StoreTable _table;
        private readonly TableField[] _columns;
        protected const string _Uuid = "Uuid";
        protected const string _version = "Version";
        protected const string _lastUpdate = "LastUpdate";
        protected const string _inserted = "Inserted";

    }


}
