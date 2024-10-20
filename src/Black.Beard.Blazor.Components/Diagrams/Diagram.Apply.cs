using Blazor.Diagrams;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;

namespace Bb.Diagrams
{

    public partial class Diagram : IDisposable
    {


        #region Load graphicalModel

        public void Apply(BlazorDiagram diagram)
        {

            _diagram = diagram;

            _diagram.Nodes.Added += Nodes_Added;
            _diagram.Links.Added += Links_Added;

            _diagram.Nodes.Removed += Nodes_Removed;
            _diagram.Links.Removed += Links_Removed;

            Apply();

        }

        private void Apply()
        {

            // Register components
            foreach (var item in this.Toolbox)
                if (item is DiagramToolNode specModel)
                    if (item.TypeUI != null)
                        _diagram.RegisterComponent(specModel.TypeModel, specModel.TypeUI, true);

            // Create nodes
            var dicPort = new Dictionary<Guid, PortModel>();
            var dicNodes = new Dictionary<Guid, UIModel>();
            ApplyNodes(dicPort, dicNodes);

            // Create links
            ApplyLinks(dicPort);

            // Maps groups
            ApplyGroups(dicNodes);

            CleanUnused();

        }

        internal void Prepare()
        {
            if (this._diagram != null)
                foreach (var item in this._diagram.Nodes)
                    _diagram.SelectModel(item, true);
        }

        private void ApplyGroups(Dictionary<Guid, UIModel> dicNodes)
        {

            foreach (var item in this.Models.Where(c => c.UuidParent != null))
            {

                var p = dicNodes[item.Uuid];

                if (dicNodes.TryGetValue(item.UuidParent.Value, out UIModel? parent))
                {
                    if (parent is UIGroupModel group)
                        group.Attach(p);
                    else
                    {
                        CleanChild(dicNodes, item);
                    }
                }
                else
                {
                    CleanChild(dicNodes, item);
                }
            }
        }


        private void ApplyLinks(Dictionary<Guid, PortModel> dicPort)
        {
            foreach (SerializableRelationship item in this.Relationships)
                if (this.Toolbox.TryGetLinkTool(item.Type, out var toolLink))
                    if (dicPort.TryGetValue(item.Source, out PortModel source))
                        if (dicPort.TryGetValue(item.Target, out PortModel target))
                        {
                            var link = CreateLink(toolLink, item, source, target);
                            var linkUI = _diagram.Links.Add(link.UILink);
                        }
        }

        public LinkProperties CreateLink(DiagramToolRelationshipBase toolLink, SerializableRelationship item, PortModel source, PortModel target)
        {

            var link = toolLink
                .CreateLink(item, source, target);

            this.Append(link);
            toolLink.Customize(link);

            return link;

        }

        public LinkProperties CreateLink(DiagramToolRelationshipBase toolLink, ILinkable source, Anchor target)
        {

            var sourceAnchor = source.ConvertToAnchor(toolLink);

            var link = toolLink
                .CreateLink(Guid.NewGuid(), sourceAnchor, target);

            this.Append(link);
            toolLink.Customize(link);

            return link;

        }


        private void ApplyNodes(Dictionary<Guid, PortModel> dicPort, Dictionary<Guid, UIModel> dicNodes)
        {

            var items = this.Models.ToList();
            int max = items.Count + 1;
            int count = 0;

            while (items.Any() && count <= max)
            {

                count++;
                var items2 = items.Where(c => c.UuidParent == null || dicNodes.ContainsKey(c.UuidParent.Value)).ToList();

                foreach (var item in items2)
                    CreateNodes(dicPort, dicNodes, item);

                foreach (var item in items2)
                    items.Remove(item);
            }

            foreach (var item in items)
                CreateNodes(dicPort, dicNodes, item);

        }

        private void CreateNodes(Dictionary<Guid, PortModel> dicPort, Dictionary<Guid, UIModel> dicNodes, SerializableDiagramNode? item)
        {
            if (this.Toolbox.TryGetNodeTool(item.Type, out DiagramToolNode? specModel))
            {
                var ui = specModel. CreateUI(item, this);
                dicNodes.Add(new Guid(ui.Id), ui);
                foreach (var port in ui.Ports)
                    dicPort.Add(new Guid(port.Id), port);
            }
        }

        private static void CleanChild(Dictionary<Guid, UIModel> dicNodes, IDiagramNode? item)
        {
            if (dicNodes.TryGetValue(item.UuidParent.Value, out UIModel? child))
                child.SetParent(null);
        }

        private void CleanUnused()
        {
            List<SerializableRelationship> _toRemove = new List<SerializableRelationship>();
            foreach (var item in Relationships)
                if (!this._diagram.Links.Where(c => c.Id == item.Uuid.ToString()).Any())
                    _toRemove.Add(item);

            foreach (var item in _toRemove)
                Relationships.Remove(item);
        }


        #endregion Load graphicalModel


        #region add/Remove

        public void AddNode(INodeModel ui)
        {

            if (ui is NodeModel group)
                _diagram.Nodes.Add(group);

            else
                throw new Exception("Invalid type");

        }

        private void Links_Removed(BaseLinkModel link)
        {
            var m = this.Relationships.Where(c => c.Uuid.ToString() == link.Id).FirstOrDefault();
            if (m != null)
                this.Relationships.Remove(m);

            if (this._links.TryGetValue(m.Uuid, out var c))
            {
                link.TargetAttached -= Links_TargetMapped;
                _links.Remove(m.Uuid);
            }

            CleanUnused();

        }

        private void Links_Added(BaseLinkModel link)
        {

            var m = this.Relationships.Where(c => c.Uuid.ToString() == link.Id).FirstOrDefault();
            if (m == null)
                if (this._links.TryGetValue(new Guid(link.Id), out var c))
                    this.Relationships.Add(c.Source);

            CleanUnused();

        }

        internal void Append(LinkProperties link)
        {

            var key = link.Uuid;
            bool test = false;

            if (this._links.ContainsKey(key))
            {
                var c = this._links[key];
                if (c != link)
                {
                    test = true;
                    c.UILink.TargetAttached -= Links_TargetMapped;
                    this._links[key] = link;
                }
            }
            else
            {
                test = true;
                this._links.Add(key, link);
            }

            if (test)
            {
                link.Diagram = _diagram;
                link.Model = this;
                link.Source.Diagram = this;
                link.UILink.TargetAttached += Links_TargetMapped;
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

            CleanUnused();

        }

        private void Nodes_Added(NodeModel model)
        {

            model.Moving += Node_Moving;
            model.Moved += Node_Moved;

            if (model is UIModel m)
            {

                var p = this.Models.FirstOrDefault(c => c.Uuid == m.Source.Uuid);
                if (p == null)
                {

                }
            }
        }

        private void Nodes_Removed(NodeModel model)
        {

            model.Moving -= Node_Moving;
            model.Moved -= Node_Moved;

            if (model is UIModel m)
            {
                var p = this.Models.FirstOrDefault(c => c.Uuid == m.Source.Uuid);
                if (p != null)
                    this.Models.Remove(p);
            }
        }

        #endregion add/Remove



        protected virtual void Node_Moved(MovableModel obj)
        {
            var items = GetChildren(new Guid(obj.Id));
            if (items.Any())
                foreach (var item in items)
                    item.TriggerParentMoved(obj);
        }

        protected virtual void Node_Moving(NodeModel obj)
        {
            var items = GetChildren(new Guid(obj.Id));
            if (items.Any())
                foreach (var item in items)
                    item.TriggerParentMoving(obj);
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

                        foreach (var ui in _diagram.Nodes)
                        {
                            ui.Moving -= Node_Moving;
                            ui.Moved -= Node_Moved;
                        }

                    }

                }
                disposedValue = true;
            }
        }

        //public UIGroupModel? GetParentByPosition(Point point)
        //{
        //    UIGroupModel parent = null;
        //    var list = _diagram.Nodes
        //        .OfType<UIGroupModel>()
        //        .Where(c => c.ContainsPoint(point)
        //                 && model.CanAcceptLikeParent(c))
        //        .ToList();
        //    if (list.Any())
        //        parent = list[0];
        //    return parent;
        //}

        public UIGroupModel? GetParentByPosition(INodeModel model)
        {

            UIGroupModel parent = null;

            var list = _diagram.Nodes
                .OfType<UIGroupModel>()
                .Where(c => c.ContainsPoint(model.Position)
                         && model.CanAcceptLikeParent(c))
                .ToList();

            if (list.Any())
                parent = list[0];

            return parent;

        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


        public bool TryGetUIModel(Guid id, out NodeModel? result)
        {
            var i = id.ToString().ToUpper();
            result = this._diagram.Nodes.FirstOrDefault(c => c.Id == i);
            return result != null;
        }

        private BlazorDiagram _diagram;
        private bool disposedValue;
        private Dictionary<Guid, LinkProperties> _links;
    }


}
