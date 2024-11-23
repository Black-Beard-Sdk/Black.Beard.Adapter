using Bb.ComponentModel.Attributes;
using Bb.Toolbars;
using Bb.TypeDescriptors;
using Blazor.Diagrams;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models.Base;
using MudBlazor;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Bb.Diagrams
{

    public partial class Diagram
        : IValidationService
        , ICommandMemorizer
        , IDynamicDescriptorInstance
        , IDiagramToolBoxProvider
        , INotifyPropertyChanging
        , INotifyPropertyChanged
        , INotifyCollectionChanged
    {

        static Diagram()
        {

            DynamicTypeDescriptionProvider.Configure<BlazorDiagram>(c =>
            {

                c.RemoveProperties
               (


               );

                c.Property(u => u.SuspendRefresh, i =>
                {
                    i.DisableValidation();
                });

                c.Property(u => u.SuspendSorting, i =>
                {
                    i.DisableValidation();
                });

                c.Property(u => u.Container, i =>
                {
                    i.DisableValidation();
                });

                c.Property(u => u.Controls, i =>
                {
                    i.DisableValidation();
                });

                c.Property(u => u.Options, i =>
                {
                    i.DisableValidation();
                });

                c.Property(u => u.Pan, i =>
                {
                    i.DisableValidation();
                });

                c.Property(u => u.Zoom, i =>
                {
                    i.DisableValidation();
                });

                c.Property(u => u.OrderedSelectables, i =>
                {
                    i.DisableValidation();
                });


            });

            DynamicTypeDescriptionProvider.Configure<Diagram>(c =>
            {

                c.RemoveProperties
                (
                    "DynamicToolbox"
                );

            });

        }

        // If true, the toolbox is dynamic and will always be created.

        public Diagram(Guid typeModelId, bool dynamicToolbox)
        {
            _toolbox = new DiagramToolbox().AppendInitializer(this);
            this.DynamicToolbox = dynamicToolbox;
            this._container = new DynamicDescriptorInstanceContainer(this);
            this.TypeModelId = typeModelId;
            Models = new DiagramList<Guid, SerializableDiagramNode>();
            Relationships = new DiagramList<Guid, SerializableRelationship>();
            _links = new Dictionary<Guid, LinkProperties>();

        }

        /// <summary>
        /// Name / label of the link
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    OnPropertyChanging(nameof(Name));
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }


        /// <summary>
        /// Description of the diagram
        /// </summary>
        public string Description
        {
            get => _descriptions;
            set
            {
                if (_descriptions != value)
                {
                    OnPropertyChanging(nameof(Description));
                    _descriptions = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        #region models

        public DiagramList<Guid, SerializableDiagramNode> Models
        {
            get => _models;
            set
            {

                if (_models != null)
                {
                    _models.CollectionChanged -= _relayCollectionChanged;
                    _models.PropertyChanging -= N_PropertyChanging;
                    _models.PropertyChanged -= N_PropertyChanged;
                }

                _models = value;

                _models.CollectionChanged += _relayCollectionChanged;
                _models.PropertyChanging += N_PropertyChanging;
                _models.PropertyChanged += N_PropertyChanged;

            }
        }

        public SerializableDiagramNode AddModel(Guid specification, double x, double y, string? name = null, Guid? uuid = null)
        {
            var spec = Toolbox.OfType<DiagramToolNode>().Where(c => c.Uuid == specification).FirstOrDefault();
            return AddModel(spec, x, y, name, uuid);
        }

        public SerializableDiagramNode AddModel(Guid specification, Guid parent, double x, double y, string? name = null, Guid? uuid = null)
        {
            var spec = Toolbox.OfType<DiagramToolNode>().Where(c => c.Uuid == specification).FirstOrDefault();
            return AddModel(spec, parent, x, y, name, uuid);
        }

        public SerializableDiagramNode AddModel(DiagramToolNode specification, Guid parent, double x, double y, string? name = null, Guid? uuid = null)
        {
            var child = AddModel(specification, x, y, name, uuid);
            if (_models.TryGetValue(parent, out var p))
            {
                child.UuidParent = parent;
            }

            return child;
        }

        public SerializableDiagramNode AddModel(DiagramToolNode specification, double x, double y, string? name = null, Guid? uuid = null)
        {

            if (string.IsNullOrEmpty(name))
            {
                var currentNames = new HashSet<string>(Models.Select(c => c.Title));
                int count = 1;
                while (currentNames.Contains(name = $"{specification.GetDefaultName()}{count}"))
                    count++;
            }

            var result = specification.CreateModel(this, x, y, name, uuid);
            _models.Add(result);

            if (_diagram != null)
            {
                var ui = specification.CreateUI(result, this);
                var parent = this.GetParentByPosition(ui);
                if (parent != null)
                    parent.AddChildren(ui);

            }

            return result;

        }

        public IEnumerable<UIModel> GetUIChildren(Guid guid)
        {
            return _diagram.Nodes
                   .OfType<UIModel>()
                   .Where(c => c.Parent == guid).ToList();
        }

        public UIModel GetUI(Guid guid)
        {

            var uuid = guid.ToString();

            return _diagram.Nodes
                   .OfType<UIModel>()
                   .Where(c => c.Id == uuid).FirstOrDefault();

        }

        public SerializableDiagramNode? GetModelByPort(Guid id)
        {

            foreach (var item in this.Models)
                foreach (var port in item.Ports)
                    if (port.Uuid == id)
                        return item;

            return null;

        }

        #endregion models


        #region links 

        public DiagramList<Guid, SerializableRelationship> Relationships
        {
            get => _relationships;
            set
            {

                if (_relationships != null)
                {
                    _relationships.CollectionChanged -= _relayCollectionChanged;
                    _relationships.PropertyChanging -= N_PropertyChanging;
                    _relationships.PropertyChanged -= N_PropertyChanged;
                }

                _relationships = value;
                _relationships.CollectionChanged += _relayCollectionChanged;
                _relationships.PropertyChanging += N_PropertyChanging;
                _relationships.PropertyChanged += N_PropertyChanged;

            }
        }

        public SerializableRelationship AddLink(Guid specification, Port left, Port right, string name, Guid? uuid = null)
        {
            var spec = Toolbox.OfType<DiagramToolRelationshipBase>().Where(c => c.Uuid == specification).FirstOrDefault();
            return AddLink(spec, left, right, name, uuid);
        }

        public SerializableRelationship AddLink(DiagramToolRelationshipBase specification, Port left, Port right, string name, Guid? uuid = null)
        {
            var link = new SerializableRelationship()
            {
                Uuid = uuid.HasValue ? uuid.Value : Guid.NewGuid(),
                Name = name,
                Type = specification.Uuid,
                Source = left.Uuid,
                Target = right.Uuid
            };
            this.Relationships.Add(link);
            return link;
        }

        #endregion links 


        public virtual void Validate(DiagramDiagnostics Diagnostics)
        {

            // Diagnostics.EvaluateModel(this);

            //foreach (var item in this.Models)
            //    Diagnostics.EvaluateModel(item);

            //foreach (var item in this.Relationships)
            //    Diagnostics.EvaluateModel(item);

        }

        public Guid TypeModelId { get; }


        [EvaluateValidation(false)]
        [JsonIgnore]
        public Action<Diagram> Save { get; private set; }

        [Browsable(false)]
        [JsonIgnore]
        [EvaluateValidation(false)]
        public DiagramDiagnostics LastDiagnostics { get; internal set; }


        #region propagate toolbox

        public bool DynamicToolbox { get; }

        public DiagramToolbox Toolbox => _toolbox;


        public virtual void InitializeToolbox(DiagramToolbox toolbox)
        {

        }

        public Toolbars.ToolbarList GetToolbar()
        {

            if (_list == null)
            {

                Dictionary<string, ToolbarGroup> groups = new Dictionary<string, ToolbarGroup>();
                foreach (var item in this.Toolbox)
                {

                    var categoryKey = item.Category.DefaultDisplay;
                    if (!groups.TryGetValue(categoryKey, out var group))
                        groups.Add(categoryKey, group = new ToolbarGroup(Guid.NewGuid(), item.Category));

                    group.Add
                    (
                        new Tool(item.Name, item.ToolTip, item.Icon, item,
                            item.Kind == ToolKind.Link, // withToggle
                            item.Kind == ToolKind.Node,  // draggable
                            !item.Hidden
                        )
                    );

                }

                _list = new ToolbarList(Guid.NewGuid(), this.Name, groups.Values);

            }

            return _list;

        }

        #endregion propagate toolbox


        protected void SetSave(Action<Diagram> save)
        {

            Save = model =>
            {

                if (model != null)
                {

                    save(model);

                    if (model.OnModelSaved != null)
                        model.OnModelSaved.Invoke(model, model);

                }
            };

        }


        public ICommandTransactionManager CommandManager { get; private set; }

        public bool CanMemorize => this._memorize != null && CommandManager != null;

        public MemorizerEnum Mode => MemorizerEnum.Global;

        protected void SetMemorize(Action<object, Stream> save)
        {
            this._memorize = save;
            CommandManager = new CommandTransactionManager(this);
            CommandManager.Pause();
        }



        public virtual void Memorize(Stream stream)
        {
            this._memorize(this, stream);
        }

        public void Restore(CommandTransaction command)
        {

            // Load the diagram
            // Compare with current and obtain differences

            // remove new links
            // Add removed nodes
            // removed new nodes
            // Add removed links

            // Restore changed nodes
            // Restore changed links

        }

        #region DynamicDescriptorInstance

        public object GetProperty(string name)
        {
            return this._container.GetProperty(name);
        }


        public void SetProperty(string name, object value)
        {
            this._container.SetProperty(name, value);
        }

        #endregion DynamicDescriptorInstance


        public Point GetRelativeMousePoint(double clientX, double clientY)
        {
            return _diagram.GetRelativeMousePoint(clientX, clientY);
        }


        public Point GetRelativePoint(double clientX, double clientY)
        {
            return _diagram.GetRelativePoint(clientX, clientY);
        }


        public Point GetScreenPoint(double clientX, double clientY)
        {
            return _diagram.GetScreenPoint(clientX, clientY);
        }

        public IEnumerable<SelectableModel> GetSelectedModels()
        {
            return _diagram.GetSelectedModels();
        }

        #region OnChange

        public event PropertyChangingEventHandler? PropertyChanging;
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<Diagram>? OnModelSaved;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        protected void OnPropertyChanging(string propertyName)
        {

            if (CommandManager?.Status == StatusTransaction.Waiting)
                throw new InvalidOperationException("Transaction not initialized");

            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));

        }

        protected void OnPropertyChanged(string propertyName)
        {

            if (CommandManager?.Status == StatusTransaction.Waiting)
                throw new InvalidOperationException("Transaction not initialized");

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

        private void _relayCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (CommandManager?.Status == StatusTransaction.Waiting)
                throw new InvalidOperationException("Transaction not initialized");

            CollectionChanged?.Invoke(sender, e);

        }

        private void N_PropertyChanging(object? sender, PropertyChangingEventArgs e)
        {
            if (CommandManager?.Status == StatusTransaction.Waiting)
                throw new InvalidOperationException("Transaction not initialized");

            PropertyChanging?.Invoke(sender, e);

        }

        private void N_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {

            if (CommandManager?.Status == StatusTransaction.Waiting)
                throw new InvalidOperationException("Transaction not initialized");

            PropertyChanged?.Invoke(sender, e);

        }

        #endregion OnChange


        private readonly DynamicDescriptorInstanceContainer _container;
        private readonly DiagramToolbox _toolbox;
        private ToolbarList _list;
        private ICommandMemorizer _command;
        private Action<object, Stream> _memorize;
        private string _name;
        private string _descriptions;
        private DiagramList<Guid, SerializableDiagramNode> _models;
        private DiagramList<Guid, SerializableRelationship> _relationships;
    }

}

/*
 
            if (_manager != null)
            {

                if (!_manager.TransactionInitialized)
                    throw new InvalidOperationException("Transaction not initialized");

            }
 */
