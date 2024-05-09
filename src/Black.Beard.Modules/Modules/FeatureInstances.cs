using Bb.ComponentModel.Attributes;
using Bb.Modules.Storage;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Bb.Modules
{
    [ExposeClass(UIConstants.Service, ExposedType = typeof(FeatureInstances), LifeCycle = IocScopeEnum.Singleton)]
    public class FeatureInstances
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ModuleModuleReferential"></param>
        public FeatureInstances(FeatureSpecifications ModuleModuleReferential,
            IStore<Guid, FeatureInstance> store)
        {
            this._referentiel = ModuleModuleReferential;
            this._store = store;
        }

        /// <summary>
        /// Return a module instance
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public FeatureInstance GetFeature(Guid uuid)
        {
            FeatureInstance module = _store.Load(uuid);
            module.Parent = this;
            return module;
        }


        /// <summary>
        /// Return a module instance
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public ObservableCollection<FeatureInstance> GetFeatures()
        {
            Initialize();
            var result = new ObservableCollection<FeatureInstance>(_store.Values());
            foreach (var item in result)
                item.Parent = this;
            return result;
        }



        /// <summary>
        /// Create a new module instance
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public FeatureInstance Create(Guid moduleUuid, Guid uuid, string name, string description)
        {

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrEmpty(description))
                throw new ArgumentNullException(nameof(description));

            Initialize();

            if (_store.Exists(uuid))
                throw new Exception("Module already exists");

            var result = new FeatureInstance()
            {
                Uuid = Guid.NewGuid(),
                Specification = uuid,
                ModuleUuid = moduleUuid,
                Label = name,
                Description = description,
                Parent = this

        };

            _store.Save(result);

            return result;

        }


        public void RemoveAllFeatureOf(ModuleInstance module)
        {
            _store.Remove(("ModuleUuid", module.Uuid));
        }

        public void Remove(FeatureInstance module)
        {
            _store.Remove(module.Uuid);
        }


        public void Save(FeatureInstance instance)
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


        private FeatureSpecifications _referentiel;
        private readonly IStore<Guid, FeatureInstance> _store;
        private volatile object _lock = new object();
        private bool _initialized;

    }



}
