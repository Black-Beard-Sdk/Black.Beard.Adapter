﻿using Bb.ComponentModel.Loaders;
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

            using (CommandManager.BeginTransaction(Commands.Mode.Paused, "Paused"))
            {
                Apply();
            }

        }

        public void SubscribesUIChanges()
        {
            _diagram.Changed += Diagram_Changed;

            _diagram.Nodes.Added += Nodes_Added;
            _diagram.Links.Added += Links_Added;

            _diagram.Nodes.Removed += Nodes_Removed;
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
            CreateNodes(this.Models);

            // Create links
            CreateLinks(this.Relationships);

            // Maps groups
            AssociateGroups(this.Models);

            CleanUnusedLinksIfNotInDocument();

        }

        internal void Prepare()
        {
            if (this._diagram != null)
                foreach (var item in this._diagram.Nodes)
                    _diagram.SelectModel(item, true);

            CommandManager.Initialize();

        }

        private void AssociateGroups(IEnumerable<SerializableDiagramNode> items)
        {

            foreach (var item in items.Where(c => c.UuidParent != null))
            {

                var ui = item.GetUI(); // Get child

                if (Models.TryGetValue(item.UuidParent.Value, out var parent))
                {
                    var parentui = parent.GetUI();  // Get child
                    if (parentui is UIGroupModel group)
                        group.Attach(ui);

                    else
                        ui.SetParent(null); // Remove parent link
                }
                else
                    ui.SetParent(null);     // Remove parent link

            }

        }

        private void CreateLinks(IEnumerable<SerializableRelationship> i)
        {

            var dicPort = new Dictionary<Guid, PortModel>(this.Models.Count * 4);
            foreach (var model in this.Models)
            {
                var ui = model.GetUI();
                foreach (var port in ui.Ports)
                    dicPort.Add(new Guid(port.Id), port);
            }

            var l = i.ToList();
            foreach (SerializableRelationship item in l)
            {
                if (Relationships.TryGetValue(item.Uuid, out var oldItem))
                    if (!object.Equals(oldItem, item))
                    {
                        Relationships.Remove(oldItem);
                        Relationships.Add(item);
                    }

                if (item.GetUI() == null)
                    if (this.Toolbox.TryGetLinkTool(item.Type, out var toolLink))
                        if (dicPort.TryGetValue(item.Source, out PortModel source))
                            if (dicPort.TryGetValue(item.Target, out PortModel target))
                                CreateLink(toolLink, item, source, target);

                            else
                                Relationships.Remove(item);
            }

        }

        public LinkProperties CreateLink(DiagramToolRelationshipBase toolLink, SerializableRelationship item,
            PortModel source, PortModel target)
        {
            LinkProperties link = toolLink.CreateLink(item, source, target);
            link.Source.Diagram = this;
            var linkUI = _diagram.Links.Add(link.UILink);
            link.UILink.TargetAttached += Links_TargetMapped;
            return toolLink.Customize(link);
        }

        public LinkProperties CreateLink(DiagramToolRelationshipBase toolLink, ILinkable source, Anchor target)
        {
            var sourceAnchor = source.ConvertToAnchor(toolLink);
            var link = toolLink.CreateLink(Guid.NewGuid(), sourceAnchor, target);
            link.Source.Diagram = this;
            link.UILink.TargetAttached += Links_TargetMapped;
            toolLink.Customize(link);

            this.Relationships.Add(link.Source);

            return link;
        }

        private void CreateNodes(IEnumerable<SerializableDiagramNode> i)
        {

            List<SerializableDiagramNode> items = i.ToList();
            var dicNodes = this._diagram.Nodes.ToDictionary(c => new Guid(c.Id));

            int max = items.Count() + 1;
            int count = 0;

            while (items.Any() && count <= max)
            {

                count++;
                var items2 = items.Where(c => c.UuidParent == null || dicNodes.ContainsKey(c.UuidParent.Value)).ToList();
                if (items2.Count == 0)
                    items2 = items;

                foreach (var item in items2)
                {

                    if (Models.TryGetValue(item.Uuid, out var oldModel))
                    {
                        if (!object.Equals(oldModel, item))
                        {
                            Models.Remove(oldModel);
                            Models.Add(item);
                        }
                    }

                    if (item.GetUI() == null)
                        if (CreateNodes(item, out var ui))
                            dicNodes.Add(new Guid(ui.Id), ui);

                }


                foreach (var item in items2)
                    items.Remove(item);

            }

        }


        private bool CreateNodes(SerializableDiagramNode? item, out UIModel result)
        {
            result = null;

            if (this.Toolbox.TryGetNodeTool(item.Type, out DiagramToolNode? specModel))
            {
                result = specModel.CreateUI(item, this);
            }

            return result != null;
        }

        private void RemoveLinks(SerializableRelationship[] list)
        {
            RemoveLinks((IEnumerable<SerializableRelationship>)list);
        }

        private void RemoveLinks(IEnumerable<SerializableRelationship> list)
        {
            Relationships.RemoveRange(list);
            foreach (var item in list)
                _diagram.Links.Remove(item.GetUI());
        }

        private void CleanUnusedLinksIfNotInDocument()
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

        public void AddNode(UIModel ui)
        {
            _diagram.Nodes.Add(ui);
        }

        #endregion add/Remove

        private BlazorDiagram _diagram;

    }


}
