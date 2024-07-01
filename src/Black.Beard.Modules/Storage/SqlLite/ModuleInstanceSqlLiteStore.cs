using Bb.Modules;

namespace Bb.Storage.SqlLite
{


    //[ExposeClass("Service", ExposedType = typeof(IStore<Guid, ModuleInstance>), LifeCycle = IocScopeEnum.Scoped)]
    public class ModuleInstanceSqlLiteStore : SqlLiteStoreBase<Guid, ModuleInstance>
    {


        public ModuleInstanceSqlLiteStore(IConfiguration configuration, ModuleSpecifications moduleSpecifications, FeatureInstances featureInstances)
            : base(configuration, "Modules", "Modules")
        {
            _moduleSpecifications = moduleSpecifications;
            _featureInstances = featureInstances;
        }


        //protected override ModuleInstance MapInstance(IDataReader reader)
        //{
        //    var result = base.MapInstance(reader);
        //    result.ModuleSpecification = _moduleSpecifications.GetModule(result.Specification);
        //    result.FeatureInstances = _featureInstances;
        //    return result;
        //}

        private readonly ModuleSpecifications _moduleSpecifications;
        private readonly FeatureInstances _featureInstances;

    }


}
