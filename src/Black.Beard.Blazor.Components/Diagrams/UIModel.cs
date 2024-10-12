using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models.Base;
using Bb.TypeDescriptors;
using Bb.ComponentModel.Attributes;
using System.ComponentModel;

namespace Bb.Diagrams
{

    /// <summary>
    /// Override NodeModel.
    /// NodeModel is graphical model
    /// </summary>
    public class UIModel
        : NodeModel
        , INodeModel
    {

        static UIModel()
        {
            _typeToExcludes = new List<Type>() { typeof(Model), typeof(NodeModel), typeof(SelectableModel), typeof(MovableModel), typeof(UIModel) };
        }

        public UIModel(SerializableDiagramNode source)
            : base(source.Uuid.ToString(), new Point(source.Position.X, source.Position.Y))
        {

            this.ControlledSize = source.ControlledSize;
            this.Locked = source.Locked;
            this._parents = new HashSet<Type>();
            Uuid = source.Uuid.ToString();
            this._container = new DynamicDescriptorInstanceContainer(this);
            this.Source = source;
            if (source.Size != null)
                this.Size = new Size(source.Size.Width, source.Size.Height);
            this.Title = Source.Name;

            CreatePort();

            var properties = _container.Properties().Where(c => !c.IsReadOnly).OrderBy(c => c.Name).ToList();
            foreach (var item in properties)
                item.Map(this, Source.Properties.PropertyExists(item.Name), Source.GetProperty(item.Name));

        }


        protected virtual void CreatePort()
        {
            foreach (var port in Source.Ports)
                AddPort(CreatePort(port));
        }

        public virtual PortModel CreatePort(Port port)
        {
            return new PortModel(port.Uuid.ToString(), this, port.Alignment);
        }


        public virtual void InitializeFirst(DiagramToolNode diagramToolNodeBase)
        {

        }

        public virtual void SynchronizeSource()
        {

            Source.Properties.CopyFrom(_container);
            Source.Position.X = this.Position.X;
            Source.Position.Y = this.Position.Y;
            Source.Name = this.Title ?? "No name";
            foreach (PortModel item in Ports)
                Source.AddPort(item.Alignment, new Guid(item.Id));

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

        public string Uuid { get; }

        private readonly DynamicDescriptorInstanceContainer? _container;
        private static List<Type> _typeToExcludes;
        private HashSet<Type> _parents;
        private bool _canBeWithoutParent;
    }

}

