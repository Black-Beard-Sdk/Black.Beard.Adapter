using Bb.Modules.Sgbd.Models;
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

        [Inject]
        public SgbdTechnologies SgbdTechnologies { get; set; }


        public SgbdDiagram DiagramModel { get => _diagramModel ?? (_diagramModel = Initialize( (SgbdDiagram)FeatureInstance.GetModel())); }

        private SgbdDiagram? Initialize(SgbdDiagram sgbdDiagram)
        {

            sgbdDiagram.SgbdTechnologies = SgbdTechnologies;

            return sgbdDiagram;
        }

        public FeatureInstance FeatureInstance => _featureInstance ?? (_featureInstance = FeatureInstances.GetFeature(Uuid));


        private FeatureInstance _featureInstance;
        private SgbdDiagram _diagramModel;
    }
}
