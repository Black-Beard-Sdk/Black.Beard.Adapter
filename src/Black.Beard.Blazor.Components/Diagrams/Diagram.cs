using Bb.ComponentModel.Attributes;
using Bb.Toolbars;
using Bb.TypeDescriptors;
using Blazor.Diagrams;
using System.ComponentModel;
using System.Text.Json.Serialization;
using static MudBlazor.CategoryTypes;

namespace Bb.Diagrams
{

    public partial class Diagram
        : IValidationService
        , ISave<Diagram>
        , IDynamicDescriptorInstance
        , IDiagramToolBoxProvider
    {

        static Diagram()
        {
            DynamicTypeDescriptionProvider.Configure<BlazorDiagram>(c =>
            {

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

        }

        // If true, the toolbox is dynamic and will always be created.

        public Diagram(Guid typeModelId, bool dynamicToolbox)
        {

            _toolbox = new DiagramToolbox().AppendInitializer(this);
            this.DynamicToolbox = dynamicToolbox;
            this._container = new DynamicDescriptorInstanceContainer(this);
            this.TypeModelId = typeModelId;
            //Specifications = new List<DiagramToolBase>();
            Models = new List<IDiagramNode>();
            Relationships = new List<SerializableRelationship>();
            _dicModels = new Dictionary<Guid, DiagramToolNode>();
            _dicLinks = new Dictionary<Guid, DiagramToolRelationshipBase>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<IDiagramNode> Models { get; set; }

        public List<SerializableRelationship> Relationships { get; set; }


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
            var p = GetModel(parent);
            if (p != null)
                child.UuidParent = parent;
            return child;
        }

        public SerializableDiagramNode AddModel(DiagramToolNode specification, double x, double y, string? name = null, Guid? uuid = null)
        {

            if (string.IsNullOrEmpty(name))
            {
                var currentNames = new HashSet<string>(Models.Select(c => c.Name));
                int count = 1;
                while (currentNames.Contains(name = $"{specification.GetDefaultName()}{count}"))
                    count++;
            }

            var result = AddModel(specification.CreateModel(x, y, name, uuid));

            if (_diagram != null)
            {
                var r = specification.CreateUI(result, this);

                var parent = this.GetParentByPosition(r);
                if (parent != null)
                    parent.AddChildren(r);

            }

            return result;

        }

        public SerializableDiagramNode AddModel(SerializableDiagramNode child)
        {
            this.Models.Add(child);
            this.Models.Sort((x, y) => x.Uuid.CompareTo(y.Uuid));
            return child;
        }

        public IDiagramNode GetModel(Guid id)
        {
            return this.Models.FirstOrDefault(c => c.Uuid == id);
        }

        public bool TryGetModel(Guid id, out IDiagramNode? result)
        {
            result = this.Models.FirstOrDefault(c => c.Uuid == id);
            return result != null;
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
            this.Models.Sort((x, y) => x.Uuid.CompareTo(y.Uuid));
            return link;
        }

        public IDiagramNode? GetModelByPort(Guid id)
        {

            foreach (var item in this.Models)
                foreach (var port in item.Ports)
                    if (port.Uuid == id)
                        return item;

            return null;

        }

        public virtual void Validate(Diagnostics Diagnostics)
        {

            // Diagnostics.EvaluateModel(this);

            //foreach (var item in this.Models)
            //    Diagnostics.EvaluateModel(item);

            //foreach (var item in this.Relationships)
            //    Diagnostics.EvaluateModel(item);

        }

        public Guid TypeModelId { get; }

        //[JsonIgnore]
        //public IEnumerable<DiagramToolBase> Specifications { get; private set; }

        [EvaluateValidation(false)]
        [JsonIgnore]
        public Action<Diagram> Save { get; private set; }

        [Browsable(false)]
        [JsonIgnore]
        [EvaluateValidation(false)]
        public Diagnostics LastDiagnostics { get; internal set; }


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

                    if (!groups.TryGetValue(item.Category.DefaultDisplay, out var group))
                        groups.Add(item.Category, group = new ToolbarGroup(Guid.NewGuid(), item.Category));

                    group.Add
                    (
                        new Tool(item.Name, item.ToolTip, item.Icon, item,
                            item.Kind == ToolKind.Link, // withToggle
                            item.Kind == ToolKind.Node  // draggable
                        )
                    );

                }

                _list = new ToolbarList(Guid.NewGuid(), this.Name, groups.Values);

            }

            return _list;

        }

        #endregion propagate toolbox




        public void SetSave(Action<Diagram> save)
        {

            Action<Diagram> _save = model =>
            {

                save(model);

                if (model.OnModelSaved != null)
                    model.OnModelSaved.Invoke(model, model);

            };

            Save = _save;


        }

        public void SetSave<T>(Action<T> save)
        {
            SetSave(model => save((T)(object)model));
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


        //public void Getii(double value)
        //{
        //    var p = _diagram.Pan;
        //}

        public event EventHandler<Diagram>? OnModelSaved;

        private readonly DynamicDescriptorInstanceContainer _container;
        private readonly Dictionary<Guid, DiagramToolNode> _dicModels;
        private readonly Dictionary<Guid, DiagramToolRelationshipBase> _dicLinks;
        private readonly DiagramToolbox _toolbox;
        private ToolbarList _list;

    }

}
