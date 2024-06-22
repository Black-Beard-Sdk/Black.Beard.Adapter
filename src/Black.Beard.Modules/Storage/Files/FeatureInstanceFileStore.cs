using Bb.ComponentModel.Attributes;
using Bb.Modules;
using Bb.Modules.Storage;

namespace Bb.Storage.Files
{

    [ExposeClass("Service", ExposedType = typeof(IStore<Guid, FeatureInstance>), LifeCycle = IocScopeEnum.Scoped)]
    public class FeatureInstanceFileStore : FileStoreBase<Guid, FeatureInstance>
    {

        public FeatureInstanceFileStore(StoreFolder folder)
            : base(folder, "FeatureInstances", ModuleConstants.Extension)
        {

        }

        protected override void MapInstance(FeatureInstance instance)
        {
            base.MapInstance(instance);
        }



    }

}
