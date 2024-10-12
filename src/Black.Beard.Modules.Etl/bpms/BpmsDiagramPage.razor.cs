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


        public Diagrams.Diagram DiagramModel
        {
            get
            {
                var f = FeatureInstance;
                if (f != null)
                {
                    var model = FeatureInstance.Load();
                    return (Diagrams.Diagram)model;
                }

                return null;

            }
        }

        private Document _featureInstance;

    }

}
