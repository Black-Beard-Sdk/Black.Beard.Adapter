using Bb.ComponentModel.Attributes;
using System.Data;


namespace Bb.Modules.Storage
{
    [ExposeClass("Service", ExposedType = typeof(IStore<Guid, FeatureInstance>), LifeCycle = IocScopeEnum.Singleton)]
    public class FeatureInstanceSqlLiteStore : SqlLiteStoreBase<Guid, FeatureInstance>
    {


        public FeatureInstanceSqlLiteStore(FeatureSpecifications featureSpecifications)
            : base("Features")
        {

            _featureSpecifications = featureSpecifications;

        }


        protected override FeatureInstance MapInstance(IDataReader reader)
        {
            var result = base.MapInstance(reader);
            result.FeatureSpecification = _featureSpecifications.GetFeature(result.Specification);
            if (result.FeatureSpecification == null)
            {

            }
            return result;
        }


        private readonly FeatureSpecifications _featureSpecifications;


    }


}
