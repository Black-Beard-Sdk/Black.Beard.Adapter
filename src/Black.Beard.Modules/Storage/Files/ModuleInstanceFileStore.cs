using Bb.ComponentModel.Attributes;
using Bb.Modules;

namespace Bb.Storage.Files
{

    [ExposeClass("Service", ExposedType = typeof(IStore<Guid, ModuleInstance>), LifeCycle = IocScopeEnum.Scoped)]
    public class ModuleInstanceFileStore : FileStoreBase<Guid, ModuleInstance>
    {

        public ModuleInstanceFileStore(IConfiguration configuration, FeatureInstances featureInstances)
            : base(configuration, "Modules", "ModuleInstances", ModuleConstants.Extension)
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
