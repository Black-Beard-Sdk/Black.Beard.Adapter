using Bb.ComponentModel.Attributes;
using Bb.Storage;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Bb.Modules
{


    [ExposeClass(UIConstants.Service, ExposedType = typeof(ModuleInstances), LifeCycle = IocScopeEnum.Scoped)]
    public class ModuleInstances
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ModuleReferential"></param>
        public ModuleInstances(ModuleSpecifications ModuleReferential,
            IStore<Guid, ModuleInstance> store)
        {
            this._referentiel = ModuleReferential;
            this._store = store;
        }

        /// <summary>
        /// Return a module instance
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public ModuleInstance GetModule(Guid uuid)
        {
            ModuleInstance module = _store.Load(uuid);
            var s = _referentiel.GetModule(module.Specification);
            module.ModuleSpecification = s;
            return module;
        }


        /// <summary>
        /// Return a module instance
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public ObservableCollection<ModuleInstance> GetModules()
        {
            Initialize();

            List<ModuleInstance> list = new List<ModuleInstance>();
            foreach (var item in _store.Index())
            {
                var s = _referentiel.GetModule(item.Specification);
                item.ModuleSpecification = s;
                list.Add(item);
            }

            var result = new ObservableCollection<ModuleInstance>(list);
            return result;
        }


        /// <summary>
        /// Create a new module instance
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public ModuleInstance Create(Guid uuid, string name, string description)
        {

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrEmpty(description))
                throw new ArgumentNullException(nameof(description));

            Initialize();

            if (_store.Exists(uuid))
                throw new Exception("Module already exists");

            var result = new ModuleInstance()
            {
                Uuid = Guid.NewGuid(),
                Specification = uuid,
                Label = name,
                Description = description,
            };


            _store.Save(result);

            return result;

        }


        public void Remove(ModuleInstance module)
        {          
            module.FeatureInstances.RemoveAllFeatureOf(module);
            _store.RemoveKey(module.Uuid);
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


        private ModuleSpecifications _referentiel;
        private readonly IStore<Guid, ModuleInstance> _store;
        private volatile object _lock = new object();
        private bool _initialized;

    }



}
