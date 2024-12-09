using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models.Base;
using Bb.TypeDescriptors;
using Bb.ComponentModel.Attributes;
using System.ComponentModel;
using System.Text.Json;
using Bb.Commands;

namespace Bb.Diagrams
{

    /// <summary>
    /// Override NodeModel.
    /// NodeModel is graphical model
    /// </summary>
    public class UIModel
        : NodeModel
        , INodeModel
        , IDisposable
        //, INotifyPropertyChanged
    {

        static UIModel()
        {

            //DynamicTypeDescriptionProvider.Configure<UIModel>(c =>
            //{

            //});


            _typeToExcludes = new List<Type>()
            {
                typeof(Model),
                typeof(NodeModel),
                typeof(SelectableModel),
                typeof(MovableModel),
                typeof(UIModel),
            };


        }

        public UIModel(SerializableDiagramNode source)
            : base(source.Uuid.ToString(), new Point(source.Position.X, source.Position.Y))
        {

            _options = new JsonSerializerOptions
            {
                Converters = { new DynamicDescriptorInstanceJsonConverter() },
                // Other options as required
                IncludeFields = true,  // You must set this if MyClass.Id and MyClass.Data are really fields not properties.
                WriteIndented = true
            };

            this.ControlledSize = source.ControlledSize;
            this.Locked = source.Locked;
            this._parents = new HashSet<Type>();
            this._container = new DynamicDescriptorInstanceContainer(this);
            this.Source = source;
            this.Source.PropertyChanged += Source_PropertyChanged;


            if (source.Position != null)
                this.Position = new Point(source.Position.X, source.Position.Y);
            if (source.Size != null)
                this.Size = new Size(source.Size.Width, source.Size.Height);
            this.Title = Source.Label;

            CreatePort();

            var properties = _container.Properties().Where(c => !c.IsReadOnly).OrderBy(c => c.Name).ToList();
            foreach (var item in properties)
                item.Map(this, Source.Properties.PropertyExists(item.Name), Source.GetProperty(item.Name), _options);


            base.SizeChanged += UIModel_SizeChanged;
            //base.Moving += UIModel_Moving;
            base.Moved += UIModel_Moved;
            base.OrderChanged += UIModel_OrderChanged;


        }

        protected virtual void Source_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {

            if (e.PropertyName == nameof(Position))
            {
                this.SetPosition(Source.Position.X, Source.Position.Y);
            }

            if (e.PropertyName == nameof(Size))
            {
                this.Size = new Size(Source.Size.Width, Source.Size.Height);
            }


            this.PropertyChanged?.Invoke(this, e);
        }

        public virtual bool UpdateDimensions()
        {
            return false;
        }


        #region events

        protected virtual void UIModel_OrderChanged(SelectableModel model)
        {

        }

        protected virtual void UIModel_Moved(MovableModel model)
        {

            
            var d = this.Source.GetDiagram<Diagram>();
            if (d.CommandManager != null)
            {

                using (var trans = d.CommandManager.BeginTransaction(Mode.Recording, $"{model.GetLabel()}"))
                {

                    if (Position != null || Source.Position.Y != Position.Y || Source.Position.X != Position.X)
                        Source.Position = new Position(Position.X, Position.Y);

                    trans.Commit();

                }

            }

        }

        //protected virtual void UIModel_Moving(NodeModel model)
        //{
        //    //var d = this.Source.GetDiagram<Diagram>();
        //    //if (d.CommandManager != null)
        //    //{
        //    //    if (d.CommandManager.Status == StatusTransaction.Waiting)
        //    //    {
        //    //        d.CommandManager.BeginTransaction($"move {this.Source.Title}");
        //    //    }
        //    //}
        //}

        protected virtual void UIModel_SizeChanged(NodeModel obj)
        {
            if (Size != null || Source.Size.Height != Size.Height || Source.Size.Width != Size.Width)
            {
                using (var trans = this.Source.GetDiagram<Diagram>().CommandManager.BeginTransaction(Mode.Recording, $"{obj.Title} has resized".Trim()))
                {
                    Source.Size = new Size(Size.Width, Size.Height);
                    trans.Commit();
                }

            }

        }
            
        public virtual void InitializeFirst(DiagramToolNode diagramToolNodeBase)
        {

        }

        #endregion events


        public virtual void SynchronizeSource()
        {
            Source.CopyFrom(_container);
            foreach (PortModel item in Ports)
                Source.AddPort(item.Alignment, new Guid(item.Id));
        }

        protected virtual void CreatePort()
        {
            foreach (var port in Source.Ports)
            {
                var uiPort = CreatePort(port);
                if (!this.Ports.Any(c => c.Id == uiPort.Id))
                    if (!this.Ports.Any(c => c.Alignment == uiPort.Alignment))
                        AddPort(uiPort);
            }
        }

        public virtual PortModel CreatePort(Port port)
        {
            return new PortModel(port.Uuid.ToString(), this, port.Alignment);
        }


        public bool ContainsPoint(Position position)
        {
            var p = new Point(position.X, position.Y);
            var bound = new Rectangle(base.Position, Size);
            return bound.ContainsPoint(p);
        }

        public bool ContainsPoint(Point position)
        {
            var bound = new Rectangle(base.Position, Size);
            return bound.ContainsPoint(position);
        }


        public object? GetProperty(string name) => this._container?.GetProperty(name);

        public void SetProperty(string name, object? value) => this._container?.SetProperty(name, value);

        public virtual void Validate(DiagramDiagnostics Diagnostics)
        {

        }


        #region Parent


        public string Label
        {
            get => Source.Label;
            set
            {
                if (Source.Label != value)
                {
                    Source.Label = value;
                    OnPropertyChanged(nameof(Label));
                }
            }
        }

        public Guid? Parent => this.Source.UuidParent;

        /// <summary>
        /// Return true if the parent is changed
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public bool SetParent(Guid? parent)
        {
            if (Parent != parent)
            {
                this.Source.UuidParent = parent;
                return true;
            }

            return false;

        }

        public void SetAvailableParents(HashSet<Type> parentTypes, bool canBeWithoutParent)
        {
            this._parents = new HashSet<Type>(parentTypes);
            this._canBeWithoutParent = canBeWithoutParent;
        }

        public virtual bool CanAcceptLikeParent(INodeModel parent)
        {
            return _parents.Contains(parent.GetType());
        }

        public bool CanBeOrphaned => _canBeWithoutParent;

        #endregion Parent


        #region INotifyPropertyChanged

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion INotifyPropertyChanged

        public void TriggerParentMoved(MovableModel parent)
        {
            ParentMoved?.Invoke(parent, this);
        }

        public void TriggerParentMoving(MovableModel parent)
        {
            ParentMoved?.Invoke(parent, this);
        }

        public event Action<MovableModel, MovableModel>? ParentMoved;
        public event Action<MovableModel, MovableModel>? ParentMovingd;

        [EvaluateValidation(false)]
        public SerializableDiagramNode Source { get; private set; }

        private readonly DynamicDescriptorInstanceContainer _container;
        private static List<Type> _typeToExcludes;
        protected JsonSerializerOptions _options;
        private HashSet<Type> _parents;
        private bool _canBeWithoutParent;
        private bool disposedValue;


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    base.SizeChanged -= UIModel_SizeChanged;
                    //base.Moving -= UIModel_Moving;
                    base.Moved -= UIModel_Moved;
                    base.OrderChanged -= UIModel_OrderChanged;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}

