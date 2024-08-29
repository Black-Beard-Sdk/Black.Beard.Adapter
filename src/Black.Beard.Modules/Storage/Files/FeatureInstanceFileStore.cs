using Bb.ComponentModel.Attributes;
using Bb.Modules;

namespace Bb.Storage.Files
{

    [ExposeClass("Service", ExposedType = typeof(IStore<Guid, Document>), LifeCycle = IocScopeEnum.Scoped)]
    public class FeatureInstanceFileStore : FileStoreBase<Guid, Document>
    {

        public FeatureInstanceFileStore(IConfiguration configuration)
            : base(configuration, "Modules", "FeatureInstances", ModuleConstants.Extension)
        {

        }

        protected override void MapInstance(Document instance)
        {
            base.MapInstance(instance);
        }



    }

}
