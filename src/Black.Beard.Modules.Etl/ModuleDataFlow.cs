using Bb.ComponentModel.Attributes;

namespace Bb.Modules.Etl
{

    [ExposeClass(ComponentModel.ConstantsCore.Plugin, ExposedType = typeof(ModuleSpecification), LifeCycle = IocScopeEnum.Singleton)]
    public class ModuleDataFlow : ModuleSpecification
    {

        public ModuleDataFlow() :
            base(
                new Guid("C9119B69-5DD9-45D2-A28A-617D6CB9D7F9"),
                "Data flows",
                "Etl module")
        {

        }

    }

}
