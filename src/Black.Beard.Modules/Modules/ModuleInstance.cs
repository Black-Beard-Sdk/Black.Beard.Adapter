using Bb.Modules.Storage;

namespace Bb.Modules
{


    public class ModuleInstance
    {

        public ModuleInstance(Guid uuid, IStore<Guid, ModuleInstance> store)
        {
            this.Uuid = uuid;
            this._store = store;
        }

        public Guid Uuid { get; }

        private readonly IStore<Guid, ModuleInstance> _store;


    }



}
