using Bb.Addons;
using Bb.ComponentModel.Attributes;
using Bb.Modules.Etl.Models;

namespace Bb.Modules.Etl
{


    [ExposeClass(Bb.ComponentModel.ConstantsCore.Plugin, ExposedType = typeof(Feature), LifeCycle = IocScopeEnum.Transiant)]
    public class EtlDiagramFeature : BaseDiagramFeature<EtlDiagram>
    {
        private const string Description = "Design data flow for your solution";

        public EtlDiagramFeature()
            : base(new Guid(Filter),
                "data flow diagram",
                Description,
                new Guid(ModuleDatas.Filter)
            )
        {
            this.Page = typeof(DiagramPage);
        }

        public const string Filter = "C9119B69-5DD9-45D2-A28A-617D6CB9D7F9";

    }


}
