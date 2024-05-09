using Blazor.Diagrams;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;

namespace Bb.Diagrams
{

    public partial class Diagram : IDisposable
    {


        public void ApplyToUI(DiagramSpecificationNodeBase specification, DiagramNode model)
        {
            var ui = specification.CreateUI(model);
            var firstNode = _diagram.Nodes.Add(ui);
        }

        public void Apply(BlazorDiagram diagram)
        {

            _diagram = diagram;

            Apply();

            _diagram.Nodes.Added += Nodes_Added;
            _diagram.Nodes.Removed += Nodes_Removed;
            _diagram.Links.Added += Links_Added;
            _diagram.Links.Removed += Links_Removed;

        }

        private void Apply()
        {

            foreach (var item in this.Specifications)
                if (item is DiagramSpecificationNodeBase specModel)
                    if (item.TypeUI != null)
                        _diagram.RegisterComponent(item.TypeModel, item.TypeUI, true);

            var dic = new Dictionary<Guid, PortModel>();
            foreach (var item in this.Models)
                if (_dicModels.TryGetValue(item.Type, out var specModel))
                {

                    var firstNode = _diagram.Nodes.Add(specModel.CreateUI(item));
                    foreach (var port in firstNode.Ports)
                        dic.Add(new Guid(port.Id), port);
                }
                else
                {

                }

            foreach (DiagramRelationship item in this.Relationships)
                if (_dicLinks.TryGetValue(item.Type, out var specLink))
                {
                    PortModel source = dic[item.Source];
                    PortModel target = dic[item.Target];
                    var link = specLink.CreateLink(item, source, target);
                    var linkUI = _diagram.Links.Add(link);
                }
                else
                {

                }

            CleanUnused();

        }

        private void Links_Removed(BaseLinkModel link)
        {
            var m = this.Relationships.Where(c => c.Uuid.ToString() == link.Id).FirstOrDefault();
            if (m != null)
                this.Relationships.Remove(m);

            CleanUnused();

        }


        private void Links_Added(BaseLinkModel link)
        {

            var m = this.Relationships.Where(c => c.Uuid.ToString() == link.Id).FirstOrDefault();
            if (m == null)
                if (link is CustomizedLinkModel c)
                {
                    this.Relationships.Add(c.Source);
                    link.TargetAttached += Links_TargetMapped;
                }

            CleanUnused();

        }

        private void CleanUnused()
        {
            List<DiagramRelationship> _toRemove = new List<DiagramRelationship>();
            foreach (var item in Relationships)
                if (!this._diagram.Links.Where(c => c.Id == item.Uuid.ToString()).Any())
                    _toRemove.Add(item);

            foreach (var item in _toRemove)
                Relationships.Remove(item);
        }

        private void Links_TargetMapped(BaseLinkModel link)
        {

            link.TargetAttached -= Links_TargetMapped;

            var m = this.Relationships
                .Where(c => c.Uuid.ToString() == link.Id)
                .FirstOrDefault();

            if (m != null)
            {

                var m3 = link.Target.Model as PortModel;
                var targetId = m3.Id;
                m.Target = new Guid(targetId);
            }

            CleanUnused();

        }




        private void Nodes_Added(NodeModel obj)
        {
            if (obj is CustomizedNodeModel m)
            {

                var p = this.Models.FirstOrDefault(c => c.Uuid == m.Source.Uuid);
                if (p == null)
                {

                }
            }
        }


        private void Nodes_Removed(NodeModel model)
        {
            if (model is CustomizedNodeModel m)
            {
                var p = this.Models.FirstOrDefault(c => c.Uuid == m.Source.Uuid);
                if (p != null)
                    this.Models.Remove(p);
            }
        }





        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_diagram != null)
                    {
                        _diagram.Nodes.Added -= Nodes_Added;
                        _diagram.Nodes.Removed -= Nodes_Removed;
                        _diagram.Links.Added -= Links_Added;
                        _diagram.Links.Removed -= Links_Removed;
                    }
                }
                disposedValue = true;
            }
        }

        // // TODO: substituer le finaliseur uniquement si 'Dispose(bool disposing)' a du code pour libérer les ressources non managées
        // ~Diagram()
        // {
        //     // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private BlazorDiagram _diagram;
        private bool disposedValue;

    }


}
