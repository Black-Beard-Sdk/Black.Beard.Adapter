﻿using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Geometry;
using Bb.ComponentModel.Accessors;
using Bb.Expressions;
using Blazor.Diagrams.Core.Models.Base;
using ICSharpCode.Decompiler.Metadata;
using System.ComponentModel;
using System.Text.Json.Serialization;
using Bb.TypeDescriptors;
using static MudBlazor.CategoryTypes;
using System.Text.Json;

namespace Bb.Diagrams
{


    public class CustomizedNodeModel : NodeModel, IDynamicDescriptorInstance
    {

        static CustomizedNodeModel()
        {

            _typeToExcludes = new List<Type>() { typeof(Model), typeof(NodeModel), typeof(SelectableModel), typeof(MovableModel), typeof(CustomizedNodeModel) };

        }

        public CustomizedNodeModel(DiagramNode source)
            : this(source.Uuid.ToString(), new Point(source.Position.X, source.Position.Y))
        {

            this._container = new DynamicDescriptorInstanceContainer(this);
            this.Source = source;
            this.Title = source.Name;

            CreatePort();

            var properties = _container.Properties().Where(c => !c.IsReadOnly).OrderBy(c => c.Name).ToList();
            foreach (var item in properties)
                item.Map(this, source.Properties.PropertyExists(item.Name), source.GetProperty(item.Name));

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


        public CustomizedNodeModel(string id, Point? position = null)
            : base(id, position)
        {

        }

        public override void SetPosition(double x, double y)
        {
            base.SetPosition(x, y);
        }


        public void SynchronizeSource()
        {
            Source.Properties.CopyFrom(_container);
            Source.Position.X = this.Position.X;
            Source.Position.Y = this.Position.Y;
            Source.Name = this.Title;
            foreach (PortModel item in Ports)
                Source.AddPort(item.Alignment, new Guid(item.Id));
        }


        public object GetProperty(string name) => this._container.GetProperty(name);

        public void SetProperty(string name, object value) => this._container.SetProperty(name, value);

        private readonly DynamicDescriptorInstanceContainer _container;

        public DiagramNode Source { get; }

        private static List<Type> _typeToExcludes;

    }

}
