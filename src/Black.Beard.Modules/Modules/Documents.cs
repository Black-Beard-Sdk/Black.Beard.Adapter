using Bb.Addons;
using Bb.ComponentModel.Attributes;
using Bb.Storage;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Bb.Modules
{
    [ExposeClass(UIConstants.Service, ExposedType = typeof(Documents), LifeCycle = IocScopeEnum.Scoped)]
    public class Documents
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ModuleModuleReferential"></param>
        public Documents(AddonFeatures ModuleModuleReferential,
            IStore<Guid, Document> store)
        {
            this._referentiel = ModuleModuleReferential;
            this._store = store;
        }

        /// <summary>
        /// Return a module instance
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public Document GeDocument(Guid uuid)
        {
            Document feature = _store.Load(uuid);
            if (feature != null)
            {
                feature.Parent = this;
                var s = _referentiel.GetFeature(feature.Specification);
                feature.Feature = s;
            }
            return feature;
        }


        /// <summary>
        /// Return a module instance
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public ObservableCollection<Document> GetDocuments()
        {

            Initialize();

            List<Document> list = new List<Document>();
            foreach (var item in _store.Index())
            {
                item.Parent = this;
                var s = _referentiel.GetFeature(item.Specification);
                item.Feature = s;
                list.Add(item);
            }

            var result = new ObservableCollection<Document>(list);
            return result;

        }



        /// <summary>
        /// Create a new module instance
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public Document Create(Guid moduleUuid, Guid uuid, string name, string description)
        {

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrEmpty(description))
                throw new ArgumentNullException(nameof(description));

            Initialize();

            if (_store.Exists(uuid))
                throw new Exception("Feature already exists");

            var result = new Document()
            {
                Uuid = Guid.NewGuid(),
                Specification = uuid,
                ModuleUuid = moduleUuid,
                Label = name,
                Description = description,
                Parent = this,
                Model = null,
            };

            _store.Save(result);

            return result;

        }


        public void RemoveAllFeatureOf(Solution module)
        {

            _store.Index()
                .Where(c => c.ModuleUuid == module.Uuid)
                .ToList()
                .ForEach(c => _store.RemoveKey(c.Uuid));

        }

        public void Remove(Document module)
        {
            _store.RemoveKey(module.Uuid);
        }


        public void Save(Document instance)
        {
            _store.Save(instance);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Initialize()
        {
            if (!_initialized)
                lock (_lock)
                    if (!_initialized)
                    {
                        _store.Initialize();
                        _initialized = true;
                    }
        }


        private AddonFeatures _referentiel;
        private readonly IStore<Guid, Document> _store;
        private volatile object _lock = new object();
        private bool _initialized;

    }



}
