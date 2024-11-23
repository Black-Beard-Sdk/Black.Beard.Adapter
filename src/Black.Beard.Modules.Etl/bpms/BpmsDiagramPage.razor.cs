using Microsoft.AspNetCore.Components;


namespace Bb.Modules.bpms
{

    public partial class BpmsDiagramPage : ComponentBase
    {

        [Parameter]
        public Guid Uuid { get; set; }


        [Inject]
        public Documents FeatureInstances { get; set; }


        public Document FeatureInstance => _featureInstance ?? (_featureInstance = FeatureInstances.GeDocument(Uuid));


        public Diagrams.Diagram DiagramModel
        {
            get
            {
                if (_diagramModel == null)
                {


                    var f = FeatureInstance;
                    if (f != null)
                    {
                        var model = FeatureInstance.Load();
                        _diagramModel = (Diagrams.Diagram)model;
                    }
                }

                return _diagramModel;

            }
        }

        private Diagrams.Diagram _diagramModel;

        private Document _featureInstance;

    }

}
