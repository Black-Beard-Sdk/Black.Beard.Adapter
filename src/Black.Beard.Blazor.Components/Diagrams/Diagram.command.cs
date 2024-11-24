using Bb.Commands;
using Blazor.Diagrams;
using Blazor.Diagrams.Core;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Bb.Diagrams
{

    public partial class Diagram : IDisposable
    {

        protected virtual void Diagram_Changed()
        {
            // CommandManager?.BeginTransaction("Diagram_Changed");
        }

        protected virtual void Node_Changed(Model model)
        {
            
        }

        protected virtual void Node_OrderChanged(SelectableModel model)
        {

        }

        protected virtual void Node_SizeChanged(NodeModel model)
        {

        }

        protected virtual void Node_Moved(MovableModel obj)
        {
            var items = GetUIChildren(new Guid(obj.Id));
            if (items.Any())
                foreach (var item in items)
                    item.TriggerParentMoved(obj);

            if (obj is UIModel ui && ui.Parent.HasValue)
                 GetUI(ui.Parent.Value)?.UpdateDimensions();

        }

        protected virtual void Node_Moving(NodeModel obj)
        {
            var items = GetUIChildren(new Guid(obj.Id));
            if (items.Any())
                foreach (var item in items)
                    item.TriggerParentMoving(obj);
        }



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



        private void _relayCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(sender, e);
        }

        private void N_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }



        private void N_PropertyChanging(object? sender, PropertyChangingEventArgs e)
        {

            if (CommandManager?.Status == StatusTransaction.Waiting)
                throw new InvalidOperationException("Transaction not initialized");

            PropertyChanging?.Invoke(sender, e);

        }

        #endregion OnChange



        public ICommandTransactionManager CommandManager { get; private set; }

        public bool CanMemorize => this._memorize != null && CommandManager != null;

        public MemorizerEnum Mode => MemorizerEnum.Global;



        protected void SetMemorize(Action<object, Stream> memorize, Func<Stream, Type, object> restore)
        {
            this._memorize = memorize;
            this._restore = restore;
            CommandManager = new CommandTransactionManager(this);
            CommandManager.Pause();
        }

        public virtual void Memorize(Stream stream)
        {
            this._memorize(this, stream);
        }

        public void Restore(CommandTransaction command)
        {

            var model = this._restore(command.Stream, GetType());


            // Load the diagram
            //var options = GetJsonSerializerOptions() ?? new JsonSerializerOptions();
            //try
            //{
            //    var result = payload.Deserialize(GetType(), options);
            //}
            //catch (NotSupportedException ex)
            //{
            //    System.Diagnostics.Trace.Fail(ex.Message, ex.ToString());
            //}

            // Compare with current and obtain differences

            // remove new links
            // Add removed nodes
            // removed new nodes
            // Add removed links

            // Restore changed nodes
            // Restore changed links

        }

    }


}
