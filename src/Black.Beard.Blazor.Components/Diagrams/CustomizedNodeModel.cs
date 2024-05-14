using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Geometry;
using Bb.ComponentModel.Accessors;
using Bb.Expressions;
using Blazor.Diagrams.Core.Models.Base;
using ICSharpCode.Decompiler.Metadata;
using System.ComponentModel;

namespace Bb.Diagrams
{


    public class CustomizedNodeModel : NodeModel
    {

        static CustomizedNodeModel()
        {

            _typeToExcludes = new List<Type>() { typeof(Model), typeof(NodeModel), typeof(SelectableModel), typeof(MovableModel), typeof(CustomizedNodeModel) };

        }

        public CustomizedNodeModel(DiagramNode source)
            : this(source.Uuid.ToString(), new Point(source.Position.X, source.Position.Y))
        {

            this.Source = source;
            this.Title = source.Name;

            CreatePort();

            var properties = PropertyAccessor.GetProperties(GetType(), true)
                .Where(c => !_typeToExcludes.Contains(c.DeclaringType) && c.CanRead && c.CanWrite)
                .ToList();

            foreach (var item in properties)
            {
                var value = source.GetProperty(item.Name);
                 if (!string.IsNullOrEmpty(value))
                {
                    var r = value.Deserialize(item.Type);
                    item.SetValue(this, r);
                }
            }

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


        public void Synchronize()
        {

            var properties = PropertyAccessor.GetProperties(GetType(), true)
                .Where(c => !_typeToExcludes.Contains(c.DeclaringType) && c.CanRead && c.CanWrite)
                .ToList();

            foreach (var item in properties)
            {
                var value = item.GetValue(this);
                var txt = value.Serialize(false);
                Source.SetProperty(item.Name, txt);
            }

            Source.Position.X = this.Position.X;
            Source.Position.Y = this.Position.Y;
            Source.Name = this.Title;


            foreach (PortModel item in Ports)
                Source.AddPort(item.Alignment, new Guid(item.Id));

        }

        public DiagramNode Source { get; }

        private static List<Type> _typeToExcludes;

    }

}
