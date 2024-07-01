using Bb.ComponentModel.Attributes;
using Bb.Modules;
using System.Reflection;

namespace Bb.Storage.SqlLite
{


    //[ExposeClass("Service", ExposedType = typeof(IStore<Guid, FeatureInstance>), LifeCycle = IocScopeEnum.Scoped)]
    public class FeatureInstanceSqlLiteStore : SqlLiteStoreBase<Guid, FeatureInstance>
    {


        public FeatureInstanceSqlLiteStore(IConfiguration configuration, FeatureSpecifications featureSpecifications)
            : base(configuration, "Modules", "Features")
        {

            //var baseName = Assembly.GetEntryAssembly().GetName().Name.Replace(".", "_");
            //var _path = Path.GetTempPath().Combine(baseName + ".db").AsFile();
            //if (!_path.Directory.Exists)
            //    _path.Directory.Create();
            //var _connectionString = $"Data Source={_path.FullName}";

            _featureSpecifications = featureSpecifications;

        }


        //protected override FeatureInstance MapInstance(IDataReader reader)
        //{
        //    var result = base.MapInstance(reader);
        //    result.FeatureSpecification = _featureSpecifications.GetFeature(result.Specification);
        //    if (result.FeatureSpecification == null)
        //    {

        //    }
        //    return result;
        //}


        private readonly FeatureSpecifications _featureSpecifications;


    }


}
