using Bb.ComponentModel.Attributes;
using Bb.Toolbars;
using Bb.TypeDescriptors;
using Blazor.Diagrams;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models.Base;
using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

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
            _models = new List<SerializableDiagramNode>();
            Relationships = new List<SerializableRelationship>();
            _links = new Dictionary<Guid, LinkProperties>();
        }

        public string Name { get; set; }

        public string Description { get; set; }



        #region models

        public List<SerializableDiagramNode> Models
        {
            get => _models;
            set
            {

                if (value != null)
                    foreach (var item in value)
                        AddModel(item);
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
                var ui = specification.CreateUI(result, this);
                var parent = this.GetParentByPosition(ui);
                if (parent != null)
                    parent.AddChildren(ui);

            }

            return result;

        }

        public SerializableDiagramNode AddModel(SerializableDiagramNode child)
        {

            var m = _models.Where(c => c.Uuid == child.Uuid && c != child).ToList();

            if (m.Count > 0)
                foreach (var item in m)
                    this._models.Remove(item);

            this._models.Add(child);
            this._models.Sort((x, y) => x.Uuid.CompareTo(y.Uuid));

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

        public IEnumerable<UIModel> GetChildren(Guid guid)
        {
            return _diagram.Nodes
                   .OfType<UIModel>()
                   .Where(c => c.Parent == guid).ToList();
        }


        #endregion models


        #region links 

        public List<SerializableRelationship> Relationships { get; set; }

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

        private readonly List<SerializableDiagramNode> _models;

        //[JsonIgnore]
        //public IEnumerable<DiagramToolBase> Specifications { get; private set; }

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


        public event EventHandler<Diagram>? OnModelSaved;

        private readonly DynamicDescriptorInstanceContainer _container;
        private readonly DiagramToolbox _toolbox;
        private ToolbarList _list;

    }

}
