using Bb.ComponentModel.Attributes;
using Bb.Modules;

namespace Bb.Storage.Files
{

    [ExposeClass("Service", ExposedType = typeof(IStore<Guid, Solution>), LifeCycle = IocScopeEnum.Scoped)]
    public class ModuleInstanceFileStore : FileStoreBase<Guid, Solution>
    {

        public ModuleInstanceFileStore(IConfiguration configuration, Documents featureInstances)
            : base(configuration, "Modules", "ModuleInstances", ModuleConstants.Extension)
        {
            _featureInstances = featureInstances;
        }

        protected override void MapInstance(Solution instance)
        {
            instance.Documents = _featureInstances;
        }

        private readonly Documents _featureInstances;

    }

}
