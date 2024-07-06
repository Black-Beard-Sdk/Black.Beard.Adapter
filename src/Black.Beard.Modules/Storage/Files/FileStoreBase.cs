using Bb.Expressions;
using Bb.Modules;


namespace Bb.Storage.Files
{


    public class FileStoreBase<TKey, TValue>
        : IStore<TKey, TValue>
        where TKey : struct
        where TValue : ModelBase<TKey>, new()
    {

        public FileStoreBase(IConfiguration configuration, string connectionStringName, string name, string extension)
        {

            var connectionString = configuration.GetConnectionString(connectionStringName);

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException($"The connection string name '{connectionStringName}' is not defined");

            this._TypeKey = typeof(TKey);
            this._TypeValue = typeof(TValue);
            this._storeRoot = connectionString;
            this._name = name;
            this._extension = extension;
            _pattern = "*" + _extension;
            _datas = new Dictionary<string, TValue>();

        }


        public string IndexName => _name;


        public bool Exists(TKey uuid)
        {
            var file = GetFilename(uuid);
            file.Refresh();
            return file.Exists;
        }

        public void Initialize()
        {
            GetRoot();
        }

        public TValue Load(TKey key)
        {

            var file = GetFilename(key);
            file.Refresh();

            EnsureUpdated(file);

            var id = Path.GetFileNameWithoutExtension(file.Name);

            if (_datas.TryGetValue(id, out TValue value))
                return value;

            return default;

        }

        public bool RemoveKey(TKey key)
        {

            var file = GetFilename(key);
            file.Refresh();

            if (file.Exists)
            {
                file.Delete();
                return true;
            }

            return false;

        }

        public void Save(TValue value)
        {
            var file = GetFilename(value.Uuid);
            file.FullName.Save(value.Serialize(true));
        }

        public IEnumerable<TValue> Index()
        {

            var root = GetRoot().AsDirectory();

            if (root.Exists)
                foreach (var item in root.GetFiles(_pattern))
                    EnsureUpdated(item);

            return _datas.Values;

        }

        public IEnumerable<TKey> Keys()
        {
            var root = GetRoot().AsDirectory();
            if (root.Exists)
                foreach (var item in root.GetFiles(_pattern))
                    yield return GetKey(item);
        }


        protected virtual void MapInstance(TValue instance)
        {

        }


        #region private

        private void EnsureUpdated(FileInfo file)
        {

            var id = Path.GetFileNameWithoutExtension(file.Name);

            if (file.Exists)
            {

                if (_datas.TryGetValue(id, out TValue value))
                {
                    if (value.LastUpdate != file.LastWriteTime)
                    {
                        value = file.FullName.LoadFromFileAndDeserialize<TValue>();
                        value.LastUpdate = file.LastWriteTime;
                        MapInstance(value);
                        _datas[id] = value;
                    }
                }
                else
                {
                    value = file.FullName.LoadFromFileAndDeserialize<TValue>();
                    value.LastUpdate = file.LastWriteTime;
                    MapInstance(value);
                    _datas.Add(id, value);
                }

            }
            else if (_datas.ContainsKey(id))
                _datas.Remove(id);

        }

        private FileInfo GetFilename(TKey uuid)
        {
            var root = GetRoot();
            return root.Combine(uuid.ToString() + _extension).AsFile();
        }

        private TKey GetKey(FileInfo file)
        {
            return (TKey)ConverterHelper.ToObject(Path.GetFileNameWithoutExtension(file.Name), _TypeKey);
        }

        private string GetRoot()
        {

            if (_root == null)
                _root = this._storeRoot.Combine(this._name);

            return _root;

        }

        private readonly string _storeRoot;
        private readonly string _name;
        private readonly string _extension;
        private readonly string _pattern;
        private readonly Dictionary<string, TValue> _datas;
        private readonly Type _TypeKey;
        private readonly Type _TypeValue;
        private string _root;

        #endregion private

    }

}
