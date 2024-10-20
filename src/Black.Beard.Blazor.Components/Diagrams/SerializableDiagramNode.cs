﻿using Bb.ComponentModel.Accessors;
using Bb.ComponentModel.Attributes;
using Bb.TypeDescriptors;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using static MudBlazor.Colors;

namespace Bb.Diagrams
{


    public class SerializableDiagramNode : IDiagramNode
    {

        #region ctor

        public SerializableDiagramNode(Guid Type)
            : this()
        {
            this.Type = Type;
        }

        public SerializableDiagramNode()
        {
            Position = new Position();
            Title = string.Empty;
            Ports = new List<Port>();
            Properties = new Properties();
            TypeNode = GetType().AssemblyQualifiedName;

            _realProperties = PropertyAccessor.GetProperties(this.GetType(), AccessorStrategyEnum.ConvertSettingIfDifferent);

            _options = new JsonSerializerOptions
            {
                Converters = { new DynamicDescriptorInstanceJsonConverter() },
                // Other options as required
                IncludeFields = true,  // You must set this if MyClass.Id and MyClass.Data are really fields and not a property.
                WriteIndented = true
            };

        }

        public virtual void Initialize(DiagramToolNode source, bool newNode)
        {

            this.Locked = source.Locked;
            this.ControlledSize = source.ControlledSize;

            if (newNode)
            {

                foreach (var port in source.InitializingPorts)
                    AddPort(port, Guid.NewGuid());

            }

        }

        #endregion ctor

        /// <summary>
        /// Unique identifier
        /// </summary>
        [Required]
        public Guid Uuid { get; set; }

        /// <summary>
        /// Identifier of the parent
        /// </summary>
        public Guid? UuidParent { get; set; }

        /// <summary>
        /// Name of the node
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Type of node
        /// </summary>
        [Required]
        public Guid Type { get; set; }


        #region Position / size

        [EvaluateValidation(false)]
        public Position Position { get; set; }

        public Size? Size { get; set; }

        public bool Locked { get; internal set; }

        public bool ControlledSize { get; internal set; }

        #endregion Position / size


        #region Dynamic properties

        [EvaluateValidation(false)]
        public Properties Properties { get; set; }

        public string? TypeNode { get; set; }

        private readonly AccessorList _realProperties;
        private readonly JsonSerializerOptions _options;

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

        #endregion Dynamic properties


        #region ports

        public List<Port> Ports { get; set; }

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

        #endregion diagram


        // public ExternalDiagramReference ExternalReference { get; set; }

    }


}