using Bb.Addons;
using Bb.ComponentModel.Attributes;
using Bb.Modules.Sgbd.Models;

namespace Bb.Modules.Sgbd
{

    [ExposeClass(ComponentModel.ConstantsCore.Plugin, ExposedType = typeof(Feature), LifeCycle = IocScopeEnum.Transiant)]
    public class SgbdDiagramFeature : BaseDiagramFeature<SgbdDiagram>
    {

        public SgbdDiagramFeature()
            : base(new Guid(Filter),
                "Sgbd diagram",
                "Design database structure",
                new Guid(ModuleDatas.Filter)
            )
        {
            Page = typeof(DiagramPage);
        }

        public const string Filter = "487E5B0F-C9E8-47A2-B72C-11DABCAE9C00";

    }


}
