using Bb.ComponentModel.Attributes;
using Bb.Modules;

namespace Bb.Storage.Files
{

    [ExposeClass("Service", ExposedType = typeof(IStore<Guid, FeatureInstance>), LifeCycle = IocScopeEnum.Scoped)]
    public class FeatureInstanceFileStore : FileStoreBase<Guid, FeatureInstance>
    {

        public FeatureInstanceFileStore(IConfiguration configuration)
            : base(configuration, "Modules", "FeatureInstances", ModuleConstants.Extension)
        {

        }

        protected override void MapInstance(FeatureInstance instance)
        {
            base.MapInstance(instance);
        }



    }

}
