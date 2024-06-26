﻿using Microsoft.AspNetCore.Components;


namespace Bb.Modules.Etl
{

    public partial class EtlDiagramPage : ComponentBase
    {

        [Parameter]
        public Guid Uuid { get; set; }

        [Inject]
        public FeatureInstances FeatureInstances { get; set; }

        public Diagrams.Diagram DiagramModel { get => (Diagrams.Diagram)FeatureInstance.GetModel(); }


        public FeatureInstance FeatureInstance => _featureInstance ?? (_featureInstance = FeatureInstances.GetFeature(Uuid));


        private FeatureInstance _featureInstance;

    }

}
