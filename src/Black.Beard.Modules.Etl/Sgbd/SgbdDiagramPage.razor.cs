using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bb.Modules.Sgbd
{
    public partial class SgbdDiagramPage : ComponentBase
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
