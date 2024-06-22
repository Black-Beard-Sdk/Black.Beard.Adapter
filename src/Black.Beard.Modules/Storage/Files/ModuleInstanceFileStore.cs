using Bb.ComponentModel.Attributes;
using Bb.Modules;
using Bb.Modules.Storage;

namespace Bb.Storage.Files
{

    [ExposeClass("Service", ExposedType = typeof(IStore<Guid, ModuleInstance>), LifeCycle = IocScopeEnum.Scoped)]
    public class ModuleInstanceFileStore : FileStoreBase<Guid, ModuleInstance>
    {

        public ModuleInstanceFileStore(StoreFolder folder, FeatureInstances featureInstances)
            : base(folder, "ModuleInstances", ModuleConstants.Extension)
        {
            _featureInstances = featureInstances;
        }

        protected override void MapInstance(ModuleInstance instance)
        {
            instance.FeatureInstances = _featureInstances;
        }

        private readonly FeatureInstances _featureInstances;

    }

}
