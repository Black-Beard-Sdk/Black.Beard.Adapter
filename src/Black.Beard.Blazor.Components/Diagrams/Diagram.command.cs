using Bb.Commands;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Bb.Diagrams
{

    public partial class Diagram : IDisposable
    {

        #region UI change

        protected virtual void Links_Removed(BaseLinkModel linkBase)
        {

            string label = linkBase.Labels.Any() ? linkBase.Labels[0].Content : string.Empty;

            var key = new Guid(linkBase.Id);
            if (Relationships.TryGetValue(key, out var link))
                using (var trans = this.CommandManager.BeginTransaction(Mode.Recording, $"link {label} has been removed"))
                {
                    this.Relationships.TryRemove(key);
                    linkBase.TargetAttached -= Links_TargetMapped;
                    _diagram.Links.Remove(link.GetUI());
                    CleanUnusedLinksIfNotInDocument();

                    trans.Commit();

                }

        }

        protected virtual void Links_Added(BaseLinkModel linkBase)
        {

            //var key = new Guid(linkBase.Id);
            //if (!Relationships.TryGetValue(key, out var link))
            //    using (var trans = this.CommandManager.BeginTransaction($"link {label} has been removed"))
            //    {
            //        this.Relationships.Add(key);
            //        linkBase.TargetAttached -= Links_TargetMapped;
            //        _diagram.Links.Remove(link.GetUI());
            //        CleanUnusedLinksIfNotInDocument();
            //    }


            //var m = this.Relationships.Where(c => c.Uuid.ToString() == linkBase.Id).FirstOrDefault();
            //if (m == null)
            //{
            //    using (var trans = this.CommandManager.BeginTransaction(""))
            //    {
            //        if (this._links.TryGetValue(new Guid(linkBase.Id), out var c))
            //        {
            //            this.Relationships.Add(c.Source);
            //        }

            //        CleanUnusedLinksIfNotInDocument();
            //    }
            //}

        }

        protected virtual void Diagram_Changed()
        {

            //using (var trans = this.CommandManager.BeginTransaction(""))
            //{


            //}

        }

        protected virtual void Node_Changed(Model model)
        {

            //using (var trans = this.CommandManager.BeginTransaction(""))
            //{


            //}

        }

        protected virtual void Node_OrderChanged(SelectableModel model)
        {

            //using (var trans = this.CommandManager.BeginTransaction(""))
            //{


            //}

        }

        protected virtual void Node_SizeChanged(NodeModel model)
        {

            //using (var trans = this.CommandManager.BeginTransaction(""))
            //{


            //}

        }

        protected virtual void Node_Moved(MovableModel obj)
        {

            var o = obj as UIModel;

            using (var trans = this.CommandManager.BeginTransaction(Mode.Recording, $"{o.Title} has moved"))
            {

                var items = GetUIChildren(new Guid(obj.Id));
                if (items.Any())
                    foreach (var item in items)
                        item.TriggerParentMoved(obj);

                if (obj is UIModel ui && ui.Parent.HasValue)
                    GetUI(ui.Parent.Value)?.UpdateDimensions();

                trans.Commit();

            }
        }

        protected virtual void Node_Moving(NodeModel obj)
        {

            var items = GetUIChildren(new Guid(obj.Id));
            if (items.Any())
                foreach (var item in items)
                    item.TriggerParentMoving(obj);

        }

        protected virtual void Links_TargetMapped(BaseLinkModel link)
        {

            var l = link.Source.GetLabel();
            var r = link.Target.GetLabel();
            string label = $"Link {l} to {r}";
            var s = (link.Source.Model as PortModel).Parent as UIModel;

            using (var trans = this.CommandManager.BeginTransaction(Mode.Recording, label))
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

                CleanUnusedLinksIfNotInDocument();

                trans.Commit();

            }

        }

        protected virtual void Nodes_Added(NodeModel model)
        {
            model.Moving += Node_Moving;
            model.Moved += Node_Moved;
            model.SizeChanged += Node_SizeChanged;
            model.OrderChanged += Node_OrderChanged;
            model.Changed += Node_Changed;
        }

        protected virtual void Nodes_Removed(NodeModel model)
        {

            using (var trans = this.CommandManager.BeginTransaction(Mode.Recording, $"{model.Title} has removed"))
            {

                model.Moving -= Node_Moving;
                model.Moved -= Node_Moved;
                model.SizeChanged -= Node_SizeChanged;
                model.OrderChanged -= Node_OrderChanged;

                if (model is UIModel m)
                {
                    var p = this.Models.FirstOrDefault(c => c.Uuid == m.Source.Uuid);
                    if (p != null)
                        this.Models.Remove(p);
                }

                trans.Commit();

            }
        }


        #endregion UI change

        #region OnChange

        public event PropertyChangingEventHandler? PropertyChanging;
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<Diagram>? OnModelSaved;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;


        protected void OnPropertyChanged(string propertyName)
        {

            if (CommandManager?.Status == StatusTransaction.Waiting)
                throw new InvalidOperationException("Transaction not initialized");

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

        protected void OnPropertyChanging(string propertyName)
        {

            if (CommandManager?.Status == StatusTransaction.Waiting)
                throw new InvalidOperationException("Transaction not initialized");

            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));

        }

        protected void RelayCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(sender, e);
        }

        protected void RelayPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        private void RelayPropertyChanging(object? sender, PropertyChangingEventArgs e)
        {
            PropertyChanging?.Invoke(sender, e);
        }

        #endregion OnChange


        public ITransactionManager CommandManager { get; private set; }

        public bool CanMemorize => this._memorize != null && CommandManager != null;

        protected void SetMemorize(Action<object, Stream> memorize, Func<Stream, Type, object> restore)
        {
            this._memorize = memorize;
            this._load = restore;
            CommandManager = new CommandTransactionManager(this);
        }

        public virtual void Memorize(Stream stream)
        {
            this._memorize(this, stream);
        }

        public void Restore(RefreshContext context)
        {

            Transaction command = context.Transaction;
            var lastModel = (Diagram)this._load(command.StreamReader, GetType());
            _diagram.SuspendRefresh = false;

            using (var trans = CommandManager.BeginTransaction(Mode.Restoring, "Restoring"))
            {
                Restore(lastModel, context, RefreshStrategy.All);
                trans.Commit();
            }

            _diagram.SuspendRefresh = true;
            _diagram.Refresh();

        }

        public event EventHandler<Diagram>? OnModelHasChanged;

        public bool Restore(object model, RefreshContext context, RefreshStrategy strategy = RefreshStrategy.All)
        {

            var lastModel = (Diagram)model;
            List<Guid> links = new List<Guid>();
            List<Guid> models = new List<Guid>();

            if (strategy.HasFlag(RefreshStrategy.Remove))
            {
                links.AddRange(RemoveLinksFromDiagram(lastModel, context));          // remove new links added after last memorize
                models.AddRange(RemoveModelsFromDiagram(lastModel, context));        // Add removed nodes added after last memorize
            }

            if (strategy.HasFlag(RefreshStrategy.Add))
            {
                models.AddRange(AddModelsFromDiagram(lastModel, context));           // removed new nodes added after last memorize
                links.AddRange(AddLinksFromDiagram(lastModel, context));             // Add removed links added after last memorize
            }

            if (strategy.HasFlag(RefreshStrategy.Update))
            {
                UpdateModelsFromDiagram(lastModel.Models, models, context);          // Restore nodes changed after last memorize
                UpdateLinksFromDiagram(lastModel.Relationships, links, context);     // Restore links changed after last memorize
            }

            return context.HasChange;

        }

        private IEnumerable<Guid> AddModelsFromDiagram(Diagram lastModel, RefreshContext context)
        {

            var items = Models.FindMissingFrom(lastModel.Models).ToArray();

            if (items.Any())
            {
                Models.AddRange(items);
                CreateNodes(items);
                AssociateGroups(items);
                foreach (var item in items)
                    context.Add(item.Uuid, item, RefreshStrategy.Add);
            }

            return items.Select(c => c.Uuid).ToArray();

        }

        private IEnumerable<Guid> RemoveModelsFromDiagram(Diagram lastModel, RefreshContext context)
        {

            var items = lastModel.Models.FindMissingFrom(Models).ToArray();

            if (items.Any())
            {
                Models.RemoveRange(items);
                foreach (var item in items)
                {
                    var ui = item.GetUI();
                    if (ui != null)
                        this._diagram.Nodes.Remove(ui);
                    context.Add(item.Uuid, item, RefreshStrategy.Remove);
                }
            }

            return items.Select(c => c.Uuid).ToArray();

        }

        private void UpdateModelsFromDiagram(DiagramList<Guid, SerializableDiagramNode> models, List<Guid> notToApply, RefreshContext context)
        {

            // && !notToApply.Contains(c.Uuid)
            var items = Models
                .Find(models)
                .Where(c => !context.Evaluate(c.Uuid, d => 
                {
                    return true; 
                }) )
                .ToArray();

            if (items.Any())
                foreach (var model in items)
                    model.Apply(models[model.Uuid], nameof(SerializableDiagramNode.Uuid), context);


        }

        private IEnumerable<Guid> RemoveLinksFromDiagram(Diagram lastModel, RefreshContext context)
        {
            var items = lastModel.Relationships.FindMissingFrom(Relationships).ToArray();
            if (items.Any())
            {
                RemoveLinks(items);
                foreach (var item in items)
                    context.Add(item.Uuid, item, RefreshStrategy.Remove);
            }

            return items.Select(c => c.Uuid).ToArray();
        }

        private IEnumerable<Guid> AddLinksFromDiagram(Diagram lastModel, RefreshContext context)
        {
            var items = Relationships.FindMissingFrom(lastModel.Relationships).ToArray();
            if (items.Any())
            {
                Relationships.AddRange(items);
                foreach (var item in items)
                {
                    context.Add(item.Uuid, item, RefreshStrategy.Add);
                    yield return item.Uuid;
                }
            }
        }

        private bool UpdateLinksFromDiagram(DiagramList<Guid, SerializableRelationship> links, List<Guid> notToApply, RefreshContext context)
        {
            bool result = false;
            var items = Relationships.FindMissingFrom(links).Where(c => !notToApply.Contains(c.Uuid)).ToArray();
            if (items.Any())
            {
                foreach (var link in items)
                {
                    link.Apply(links[link.Uuid], nameof(SerializableRelationship.Uuid), context);
                }
            }

            return result;

        }

    }


}
