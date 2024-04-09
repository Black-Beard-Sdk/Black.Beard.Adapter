namespace Bb.Modules.Etl
{


    public class EtlDiagramFeature : FeatureSpecification
    {

        public EtlDiagramFeature()
            : base(new Guid("22B9A8DB-0F38-4C2E-82B0-F853CB57D646"),
            "data flow diagram",
            "Design data flow for your module",
            typeof(ModuleDataFlow),
            typeof(EtlDiagramModel)
                  )
        {

        }


    }

}
