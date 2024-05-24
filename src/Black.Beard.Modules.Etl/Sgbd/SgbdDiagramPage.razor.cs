using Bb.Diagrams;
using Bb.Generators;
using Bb.Modules.Sgbd.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bb.Modules.Sgbd
{
    public partial class SgbdDiagramPage : ComponentBase, IDisposable
    {

        public SgbdDiagramPage()
        {

        }


        [Parameter]
        public Guid Uuid { get; set; }

        [Inject]
        public FeatureInstances FeatureInstances { get; set; }

        [Inject]
        public SgbdTechnologies SgbdTechnologies { get; set; }


        public SgbdDiagram DiagramModel { get => _diagramModel ?? (_diagramModel = Initialize((SgbdDiagram)FeatureInstance.GetModel())); }

        private SgbdDiagram? Initialize(SgbdDiagram sgbdDiagram)
        {

            sgbdDiagram.SgbdTechnologies = SgbdTechnologies;

            sgbdDiagram.OnModelSaved += ModelSaved;

            return sgbdDiagram;
        }

        private void ModelSaved(object? sender, Diagram e)
        {

            var technology = DiagramModel.GetTechnology();
            if (technology != null)
            {
                var ctx = new ContextGenerator()
                {
                    RootPath = "c:\\temp",
                };

                var gen = DiagramModel.GetTechnology().GetGenerator(ctx);

                var items = gen.Generate(e).ToList();

                ctx.WriteOnDisk(items);
            }
        }

        public FeatureInstance FeatureInstance => _featureInstance ?? (_featureInstance = FeatureInstances.GetFeature(Uuid));


        private FeatureInstance _featureInstance;
        private SgbdDiagram _diagramModel;
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (DiagramModel != null)
                        DiagramModel.OnModelSaved -= ModelSaved;
                }

                disposedValue = true;
            }
        }

        // // TODO: substituer le finaliseur uniquement si 'Dispose(bool disposing)' a du code pour libérer les ressources non managées
        // ~SgbdDiagramPage()
        // {
        //     // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}
