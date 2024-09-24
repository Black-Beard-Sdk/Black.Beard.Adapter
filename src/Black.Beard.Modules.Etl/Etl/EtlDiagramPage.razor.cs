using Microsoft.AspNetCore.Components;


namespace Bb.Modules.Etl
{

    public partial class EtlDiagramPage : ComponentBase
    {

        [Parameter]
        public Guid Uuid { get; set; }


        [Inject]
        public Documents FeatureInstances { get; set; }

        public Diagrams.Diagram DiagramModel { get => (Diagrams.Diagram)FeatureInstance.Load(); }


        public Document FeatureInstance => _featureInstance ?? (_featureInstance = FeatureInstances.GeDocument(Uuid));


        private Document _featureInstance;

    }

}
