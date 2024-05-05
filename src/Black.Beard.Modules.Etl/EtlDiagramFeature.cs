using Bb.ComponentModel.Attributes;
using Bb.Modules.Etl.Pages;

namespace Bb.Modules.Etl
{


    [ExposeClass(Bb.ComponentModel.ConstantsCore.Plugin, ExposedType = typeof(FeatureSpecification), LifeCycle = IocScopeEnum.Transiant)]
    public class EtlDiagramFeature : FeatureSpecification
    {

        public EtlDiagramFeature()
            : base(new Guid("22B9A8DB-0F38-4C2E-82B0-F853CB57D646"),
            "data flow diagram",
            "Design data flow for your module",
            new Guid("C9119B69-5DD9-45D2-A28A-617D6CB9D7F9"),
            typeof(EtlDiagramModel)
                  )
        {

            this.Page = typeof(DiagramPage);

        }


    }

}
