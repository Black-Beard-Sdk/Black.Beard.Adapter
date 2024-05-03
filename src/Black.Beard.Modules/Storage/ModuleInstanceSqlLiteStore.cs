using Bb.ComponentModel.Accessors;
using Bb.ComponentModel.Attributes;


namespace Bb.Modules.Storage
{



    [ExposeClass("Service", ExposedType = typeof(IStore<Guid, ModuleInstance>), LifeCycle = IocScopeEnum.Singleton)]
    public class ModuleInstanceSqlLiteStore : SqlLiteStoreBase<Guid, ModuleInstance>
    {


        public ModuleInstanceSqlLiteStore()
            : base("Modules") 
        {


        }

        //protected override void MapParameter(ModuleInstance value, List<(string, object)> parameters)
        //{
        //    parameters.Add((_name.ToLower(), value.Label));
        //    parameters.Add((_specification.ToLower(), value.Specification));
        //    parameters.Add((_data.ToLower(), value.Serialize(false)));
        //}

        private const string _name = "Name";
        private const string _specification = "Specification";

    }


}
