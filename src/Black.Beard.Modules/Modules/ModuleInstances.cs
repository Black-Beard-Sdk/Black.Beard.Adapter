using Bb.ComponentModel.Attributes;
using Bb.Modules.Storage;

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
        /// Create a new module instance
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public ModuleInstance Create(Guid uuid, string name, string description)
        {

            //if (_store.Exist(uuid))
            //    return _store.Load(uuid);

            var result = new ModuleInstance(uuid, Guid.NewGuid(), _store)
            {
                Label = name,
                Description = description,
            };

            return result;

        }

        private ModuleSpecifications _referentiel;
        private readonly IStore<Guid, ModuleInstance> _store;
        private volatile object _lock = new object();

    }


}
