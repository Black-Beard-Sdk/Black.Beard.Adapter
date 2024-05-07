using Blazor.Diagrams;
using Blazor.Diagrams.Core;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using System.Text.Json.Serialization;
using static MudBlazor.CategoryTypes;

namespace Bb.Diagrams
{

    public partial class Diagram
    {


        public void ApplyToUI(DiagramSpecificationModelBase specification, DiagramItemBase model)
        {
            this._internalTransaction = true;
            var ui = specification.CreateUI(model);
            var firstNode = _diagram.Nodes.Add(ui);
            this._internalTransaction = false;
        }

        public void Apply(BlazorDiagram diagram)
        {

            _diagram = diagram;

            var dic = new Dictionary<Guid, PortModel>();
            foreach (var item in this.Models)
            {
                if (_dicModels.TryGetValue(item.Type, out var specModel))
                {

                    var firstNode = diagram.Nodes.Add(specModel.CreateUI(item));
                    foreach (var port in firstNode.Ports)
                        dic.Add(new Guid(port.Id), port);
                }
                else
                {

                }
            }

            foreach (var item in this.Relationships)
            {

                if (_dicLinks.TryGetValue(item.Type, out var specLink))
                {
                    var source = dic[item.Source];
                    var target = dic[item.Target];
                    var link = specLink.CreateLink(source, target);
                    var linkUI = diagram.Links.Add(link);
                }
                else
                {

                }
            }

            _diagram.Nodes.Added += Nodes_Added;
            _diagram.Nodes.Removed += Nodes_Removed;
            diagram.Links.Added += Links_Added;
            diagram.Links.Removed += Links_Removed;

        }

        private void Links_Removed(BaseLinkModel link)
        {
            var m = this.Relationships.Where(c => c.Uuid.ToString() == link.Id).FirstOrDefault();
            if (m != null)
                this.Relationships.Remove(m);
        }


        private void Links_Added(BaseLinkModel link)
        {

            var m = this.Relationships.Where(c => c.Uuid.ToString() == link.Id).FirstOrDefault();
            if (m == null)
            {

                var m2 = link.Source.Model as PortModel;

                if (m2 != null)
                {

                    var sourceId = m2.Id;
                    //var sourceAlignment = m2.Alignment;

                    this.Relationships.Add(new DiagramRelationship()
                    {
                        Uuid = new Guid(link.Id),
                        Source = new Guid(sourceId),
                        //Type = link.GetType().Name,
                    });


                    link.TargetAttached += Links_TargetMapped;

                }

            }

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


        private BlazorDiagram _diagram;
        private bool _internalTransaction;

    }


}
