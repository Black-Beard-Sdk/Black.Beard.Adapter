using Bb.Modules.Bpms;
using Microsoft.AspNetCore.Components;


namespace Bb.Modules.bpms
{

    public partial class BpmsDiagramPage : ComponentBase
    {

        [Parameter]
        public Guid Uuid { get; set; }


        [Inject]
        public Documents FeatureInstances { get; set; }


        public Document FeatureInstance => _featureInstance 
            ?? (_featureInstance = FeatureInstances.GeDocument(Uuid));


        public Diagrams.Diagram DiagramModel { get => (Diagrams.Diagram)FeatureInstance.Load(); }


        private Document _featureInstance;

    }

}
