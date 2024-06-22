﻿using Bb;
using Bb.Modules;
using Bb.Modules.Storage;

namespace Bb.Storage.Files
{

    public class FileStoreBase<TKey, TValue> : IStore<TKey, TValue>
                where TKey : struct
        where TValue : ModelBase<TKey>, new()
    {

        public FileStoreBase(StoreFolder folder, string name, string extension)
        {
            this._storeRoot = folder;
            this._name = name;
            this._extension = extension;
            _pattern = "*" + _extension;
            _datas = new Dictionary<string, TValue>();
        }


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

        public bool Remove(TKey key)
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

        public int Remove((string, object) parameter)
        {
            throw new NotImplementedException();
        }

        public void Save(TValue value)
        {
            var file = GetFilename(value.Uuid);
            file.FullName.Save(value.Serialize(true));
        }

        public IEnumerable<TValue> Values()
        {

            var root = GetRoot().AsDirectory();

            foreach (var item in root.GetFiles(_pattern))
                EnsureUpdated(item);

            return _datas.Values;

        }

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

        protected virtual void MapInstance(TValue instance)
        {

        }


        private FileInfo GetFilename(TKey uuid)
        {
            var root = GetRoot();
            return root.Combine(uuid.ToString() + _extension).AsFile();
        }


        private string GetRoot()
        {

            if (_root == null)
                _root = this._storeRoot.GetRoot(this._name);

            return _root;

        }



        private readonly StoreFolder _storeRoot;
        private readonly string _name;
        private readonly string _extension;
        private readonly string _pattern;
        private readonly Dictionary<string, TValue> _datas;
        private string _root;
    }

}
