using Bb.ComponentModel.Accessors;
using Bb.ComponentModel.Attributes;
using Bb.TypeDescriptors;
using Blazor.Diagrams;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;

namespace Bb.Diagrams
{

    /// <summary>
    /// diagram node object specialized for serialization
    /// </summary>
    public class SerializableDiagramNode
        : INotifyPropertyChanging
        , INotifyPropertyChanged
    {

        #region ctor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Type"></param>
        public SerializableDiagramNode(Guid Type)
            : this()
        {
            this.ToolType = Type;
        }

        public SerializableDiagramNode()
        {
            Position = new Position();
            Label = string.Empty;
            Ports = new Ports();
            TypeNode = GetType().AssemblyQualifiedName;

            _realProperties = this.GetType().GetAccessors(AccessorStrategyEnum.ConvertSettingIfDifferent);

            _options = new JsonSerializerOptions
            {
                Converters = { new DynamicDescriptorInstanceJsonConverter() },
                // Other options as required
                IncludeFields = true,  // You must set this if MyClass.Id and MyClass.Data are really fields and not a property.
                WriteIndented = true
            };

            Properties = new Properties();

        }

        public virtual void Initialize(DiagramToolNode source, bool newNode)
        {
            if (newNode)
            {
                foreach (var port in source.InitializingPorts)
                    AddPort(port, Guid.NewGuid());
            }

        }

        #endregion ctor


        #region properties

        /// <summary>
        /// Unique identifier
        /// </summary>
        [Required, Key]
        public Guid Uuid
        {
            get => _uid;
            set
            {
                if (_uid != value)
                {
                    OnPropertyChanging(nameof(Uuid));
                    _uid = value;
                    this.Properties.SetUuid(value);
                    OnPropertyChanged(nameof(Uuid));
                }
            }
        }

        /// <summary>
        /// Identifier of the parent
        /// </summary>
        public Guid? UuidParent
        {
            get => _uuidParent;
            set
            {
                if (_uuidParent != value)
                {
                    OnPropertyChanging(nameof(UuidParent));
                    _uuidParent = value;
                    OnPropertyChanged(nameof(UuidParent));
                }
            }
        }

        /// <summary>
        /// Name of the node
        /// </summary>
        [Required]
        public string Label
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    OnPropertyChanging(nameof(Label));
                    _title = value;
                    OnPropertyChanged(nameof(Label));
                }
            }
        }

        /// <summary>
        /// guid of the tools that create the node
        /// </summary>
        [Required]
        public Guid ToolType
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    OnPropertyChanging(nameof(ToolType));
                    _type = value;
                    OnPropertyChanged(nameof(ToolType));
                }
            }
        }

        public string? TypeNode
        {
            get => _typeNode;
            set
            {
                if (_typeNode != value)
                {
                    OnPropertyChanging(nameof(TypeNode));
                    _typeNode = value;
                    OnPropertyChanged(nameof(TypeNode));
                }
            }
        }

        private Guid _type;
        private Guid _uid;
        private string _title;
        private Guid? _uuidParent;

        #endregion properties


        #region Position / size

        [EvaluateValidation(false)]
        public Position Position
        {
            get => _position;
            set
            {
                if (!object.Equals(_position, value))
                {
                    OnPropertyChanging(nameof(Position));
                    _position = value;
                    OnPropertyChanged(nameof(Position));
                }
            }
        }

        public Size? Size
        {
            get => _size;
            set
            {
                if (_size != value)
                {
                    OnPropertyChanging(nameof(Size));
                    _size = value;
                    OnPropertyChanged(nameof(Size));
                }
            }
        }

        [JsonIgnore, EvaluateValidation(false)]
        public bool Locked { get; internal set; }

        [JsonIgnore, EvaluateValidation(false)]
        public bool ControlledSize { get; internal set; }

        private bool _ControlledSize;
        private Position _position;
        private Size? _size;
        private bool _Locked;

        #endregion Position / size


        #region Dynamic properties

        [EvaluateValidation(false)]
        public Properties Properties
        {
            get => _properties;
            set
            {
                if (_properties != value)
                {

                    if (_properties != null)
                        _properties.Merge(value);

                    else
                    {
                        OnPropertyChanging(nameof(Properties));
                        _properties = value;
                        OnPropertyChanged(nameof(Properties));
                        _properties.PropertyChanging += _properties_PropertyChanging;
                        _properties.PropertyChanged += _properties_PropertyChanged;
                    }

                }
            }
        }

        private void _properties_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        private void _properties_PropertyChanging(object? sender, PropertyChangingEventArgs e)
        {
            PropertyChanging?.Invoke(this, e);
        }

        public void SetProperty(string name, object? value)
        {

            if (_realProperties.TryGetValue(name, out var accessor))
            {
                accessor.SetValue(this, value);
            }
            else
            {
                var valueString = value?.Serialize(_options);
                Properties.SetProperty(name, valueString);
            }

        }

        public object? GetProperty(string name)
        {

            if (_realProperties.TryGetValue(name, out var accessor))
                return accessor.GetValue(this);

            return Properties.GetProperty(name);
        }


        private readonly AccessorList _realProperties;
        private readonly JsonSerializerOptions _options;
        private string? _typeNode;
        private Properties _properties;


        #endregion Dynamic properties


        #region ports

        public Ports Ports
        {
            get => _ports;
            set
            {
                if (_ports != value)
                {
                    OnPropertyChanging(nameof(Ports));
                    _ports = value;
                    OnPropertyChanged(nameof(Ports));
                }
            }
        }

        public Port AddPort(PortAlignment alignment, Guid id)
        {

            var p = Ports.FirstOrDefault(c => c.Uuid == id);
            if (p == null)
            {
                p = new Port() { Uuid = id, Alignment = alignment };
                Ports.Add(p);
            }
            else if (p.Alignment != alignment)
                p.Alignment = alignment;

            return p;
        }

        public Port GetPort(PortAlignment alignment)
        {
            return Ports.FirstOrDefault(c => c.Alignment == alignment);
        }

        public Port GetPort(Guid id)
        {
            return Ports.FirstOrDefault(c => c.Uuid == id);
        }

        #endregion ports


        #region diagram

        public virtual void CopyFrom(DynamicDescriptorInstanceContainer container)
        {

            var properties = container.Properties()
                .Where(c => !c.IsReadOnly)
                .OrderBy(c => c.Name)
                .ToList();

            foreach (var item in properties)
            {
                var v = item.GetValue(container.Instance);
                SetProperty(item.Name, v);
            }

        }

        public T? GetDiagram<T>()
            where T : Diagram
        {
            return (T)_diagram;
        }

        public void SetDiagram<T>(T diagram)
            where T : Diagram
        {
            _diagram = diagram;
        }

        public bool DiagramIs<T>()
        {
            if (_diagram is T)
                return true;
            return false;
        }

        private Diagram? _diagram;
        private Ports _ports;

        #endregion diagram


        #region OnChange

        public event PropertyChangingEventHandler? PropertyChanging;
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanging(string propertyName)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion OnChange


        #region UI

        public UIModel SetUI(UIModel ui)
        {
            _ui = ui;
            return _ui;
        }
        public UIModel GetUI()
        {
            return _ui;
        }
        private UIModel _ui;

        #endregion UI


        public UIModel ResolveParentUI(BlazorDiagram diagram)
        {

            if (_ui == null)
            {

                if (_diagram == null)
                    throw new NullReferenceException(nameof(diagram));

                _ui = diagram.Nodes.OfType<UIModel>().Where(c => c.Id == this.UuidParent.ToString()).FirstOrDefault();

            }

            return _ui;
        }

        private UIModel _uiParent;

    }


}
