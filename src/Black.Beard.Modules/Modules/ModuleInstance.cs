using Bb.Modules.Storage;

namespace Bb.Modules
{


    public class ModelBase
    {

        public Guid Uuid { get; set; }

        public int Version { get; set; }

        public DateTimeOffset? LastUpdate { get; set; }
        public DateTimeOffset? Inserted { get; set; }

    }

    public class ModuleInstance : ModelBase
    {

        public ModuleInstance()
        {
            
        }

        public ModuleInstance(Guid uuidSpecification, Guid key, IStore<Guid, ModuleInstance> store)
        {

            this.Uuid = key;
            this.Specification = uuidSpecification;
            this._store = store;
        }


        
        public string Label { get; internal set; }
        
        public string Description { get; internal set; }

        public Guid Specification { get; }

        internal void Save()
        {
            
            _store.Save(this);

        }

        private readonly IStore<Guid, ModuleInstance> _store;

    }



}
