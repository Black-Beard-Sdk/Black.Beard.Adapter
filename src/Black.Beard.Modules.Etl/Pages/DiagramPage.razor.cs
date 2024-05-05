using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bb.Modules.Etl.Pages
{
    public partial class DiagramPage : ComponentBase
    {

        [Parameter]
        public Guid Uuid { get; set; }

        [Inject]
        public FeatureInstances FeatureInstances { get; set; }

        public FeatureInstance FeatureInstance => _featureInstance ?? (_featureInstance = FeatureInstances.GetFeature(Uuid));


        private FeatureInstance _featureInstance;

    }
}
