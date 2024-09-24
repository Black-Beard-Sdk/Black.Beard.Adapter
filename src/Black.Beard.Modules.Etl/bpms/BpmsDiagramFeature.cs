using Bb.Addons;
using Bb.ComponentModel.Attributes;
using Bb.Modules.bpms;

namespace Bb.Modules.Bpms.Models
{

    [ExposeClass(Bb.ComponentModel.ConstantsCore.Plugin, ExposedType = typeof(Feature), LifeCycle = IocScopeEnum.Transiant)]
    public class BpmsDiagramFeature : BaseDiagramFeature<BpmsDiagram>
    {
        private const string Description = "Business process management";

        public BpmsDiagramFeature()
            : base(new Guid(FeatureFilter),
                "bpms diagram",
                Description,
                new Guid(ModuleDatas.Filter)
            )
        {
            this.Page = typeof(BpmsDiagramPage);
        }


        public const string FeatureFilter = "ABEA097C-FB59-42DB-B5CB-F667C3F19752";

    }


}
