using Bb.ComponentModel.Attributes;
using Bb.Modules.Storage;
using System.Runtime.CompilerServices;

namespace Bb.Modules
{

    [ExposeClass(UIConstants.Service, ExposedType = typeof(ModuleInstances), LifeCycle = IocScopeEnum.Singleton)]
    public class ModuleInstances
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ModuleModuleReferential"></param>
        public ModuleInstances(ModuleSpecifications ModuleModuleReferential, 
            IStore<Guid, ModuleInstance> store)
        {
            this._referentiel = ModuleModuleReferential;
            this._store = store;
        }

        /// <summary>
        /// Return a module instance
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public ModuleInstance GetModule(Guid uuid)
        {
            ModuleInstance module = null;

            //if (_store.Exist(uuid))
            //    module = _store.Load(uuid);

            return module;
        }


        /// <summary>
        /// Return a module instance
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public IEnumerable<ModuleInstance> GetModules()
        {
            Initialize();
            var values = _store.Values();
            return values;
        }


        /// <summary>
        /// Create a new module instance
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public ModuleInstance Create(Guid uuid, string name, string description)
        {

            Initialize();

            var result = new ModuleInstance(uuid, Guid.NewGuid(), _store)
            {
                Label = name,
                Description = description,
            };

            result.Save();

            return result;

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
