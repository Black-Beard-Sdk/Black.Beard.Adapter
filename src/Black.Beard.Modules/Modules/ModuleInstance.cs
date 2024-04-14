using Bb.Modules.Storage;

namespace Bb.Modules
{


    public class ModuleInstance
    {

        public ModuleInstance(Guid uuidSpecification, Guid key, IStore<Guid, ModuleInstance> store)
        {

            this.Uuid = key;
            this.Specification = Specification;
            this._store = store;
        }

        public Guid Specification { get; }

        public Guid Uuid { get; }
        public string Label { get; internal set; }
        public string Description { get; internal set; }

        private readonly IStore<Guid, ModuleInstance> _store;

        internal void Save()
        {
            
            _store.Save(this);

        }
    }



}
