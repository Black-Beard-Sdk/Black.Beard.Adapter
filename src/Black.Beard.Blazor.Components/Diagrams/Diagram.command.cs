using Bb.Commands;
using Blazor.Diagrams.Core.Geometry;
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

        [RestoreIgnore]
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

            Prepare(context);

            Transaction command = context.Transaction;
            var lastModel = (Diagram)this._load(command.StreamReader, GetType());
            _diagram.SuspendRefresh = false;

            using (var trans = CommandManager.BeginTransaction(Mode.Restoring, "Restoring"))
            {

                Relationships.RestoreRemove(lastModel.Relationships, context);
                var result = context.ApplyUpdate(this, lastModel);

                while (context.Remove() | context.Add() | context.Update())
                {
                    // do nothing
                }

                Relationships.RestoreUpdate(lastModel.Relationships, context);
                trans.Commit();

            }

            _diagram.SuspendRefresh = true;
            _diagram.Refresh();

        }

        private void Prepare(RefreshContext context)
        {

            context.AddTypeToCopy(typeof(Position));
            context.AddTypeToCopy(typeof(Size));

            context.ApplyAfterUpdate<SerializableDiagramNode>(RefreshStrategy.Removed, item =>
            {
                var ui = item.GetUI();
                if (ui != null)
                    this._diagram.Nodes.Remove(ui);
            });
            context.ApplyAfterUpdate<List<SerializableDiagramNode>>(RefreshStrategy.Added, added =>
            {
                CreateNodes(added);
                AssociateGroups(added);
            });
            context.ApplyAfterUpdate<SerializableDiagramNode>(RefreshStrategy.Updated, updated =>
            {
                var right = Models[updated.Uuid];
                var ui = right.GetUI();



            });

            context.ApplyAfterUpdate<SerializableRelationship>(RefreshStrategy.Removed, item =>
            {
                _diagram.Links.Remove(item.GetUI());
            });
        }

        public event EventHandler<Diagram>? OnModelHasChanged;

    }


}
