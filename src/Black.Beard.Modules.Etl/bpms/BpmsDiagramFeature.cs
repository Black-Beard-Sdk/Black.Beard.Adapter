using Bb.Addons;
using Bb.ComponentModel.Attributes;

namespace Bb.Modules.Bpms.Models
{

    [ExposeClass(Bb.ComponentModel.ConstantsCore.Plugin, ExposedType = typeof(Feature), LifeCycle = IocScopeEnum.Transiant)]
    public class BpmsDiagramFeature : BaseDiagramFeature<BpmsDiagram>
    { 

        public BpmsDiagramFeature()
            : base(new Guid(FeatureFilter),
                "bpms diagram",
                Description,
                new Guid(ModuleDatas.Filter)
            )
        {
            this.Page = typeof(DiagramPage);
        }


        public const string FeatureFilter = "ABEA097C-FB59-42DB-B5CB-F667C3F19752";
        private const string Description = "Business process management";

    }


}
