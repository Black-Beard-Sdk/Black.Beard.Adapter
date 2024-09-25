﻿using Blazor.Diagrams;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using static MudBlazor.CategoryTypes;

namespace Bb.Diagrams
{

    public partial class Diagram : IDisposable
    {


        #region Load graphicalModel

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

        private void ApplyGroups(Dictionary<Guid, UIModel> dicNodes)
        {
            foreach (var item in this.Models.Where(c => c.UuidParent != null))
            {
                if (dicNodes.TryGetValue(item.UuidParent.Value, out UIModel? parent))
                {
                    if (parent is UIGroupModel group)
                        group.Attach(item as NodeModel);
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
                if (_dicLinks.TryGetValue(item.Type, out var specLink))
                    if (dicPort.TryGetValue(item.Source, out PortModel source))
                        if (dicPort.TryGetValue(item.Target, out PortModel target))
                        {
                            var link = specLink.CreateLink(item, source, target);
                            var linkUI = _diagram.Links.Add(link);
                        }
        }

        private void ApplyNodes(Dictionary<Guid, PortModel> dicPort, Dictionary<Guid, UIModel> dicNodes)
        {
            foreach (var item in this.Models)
                if (_dicModels.TryGetValue(item.Type, out var specModel))
                {
                    // Register ports
                    var ui = specModel.CreateUI(item, this);
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

            CleanUnused();

        }

        private void Links_Added(BaseLinkModel link)
        {

            var m = this.Relationships.Where(c => c.Uuid.ToString() == link.Id).FirstOrDefault();
            if (m == null)
                if (link is CustomizedLinkModel c)
                {
                    c.Source.Diagram = this;
                    this.Relationships.Add(c.Source);
                    link.TargetAttached += Links_TargetMapped;
                }

            CleanUnused();

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
            if (obj is UIModel m)
            {

                var p = this.Models.FirstOrDefault(c => c.Uuid == m.Source.Uuid);
                if (p == null)
                {

                }
            }
        }

        private void Nodes_Removed(NodeModel model)
        {
            if (model is UIModel m)
            {
                var p = this.Models.FirstOrDefault(c => c.Uuid == m.Source.Uuid);
                if (p != null)
                    this.Models.Remove(p);
            }
        }

        #endregion add/Remove


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

        public UIGroupModel? GetParentByPosition(INodeModel model)
        {

            UIGroupModel parent = null;

            var list = _diagram.Groups
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

    }


}
