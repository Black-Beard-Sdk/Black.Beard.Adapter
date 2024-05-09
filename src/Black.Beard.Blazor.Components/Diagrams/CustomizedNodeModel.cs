using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Geometry;
using Bb.ComponentModel.Accessors;
using Bb.Expressions;
using Blazor.Diagrams.Core.Anchors;
using ICSharpCode.Decompiler.Metadata;
using static MudBlazor.CategoryTypes;
using static MudBlazor.Colors;
using Blazor.Diagrams.Core.Models.Base;

namespace Bb.Diagrams
{


    public class CustomizedLinkModel : LinkModel
    {


        static CustomizedLinkModel()
        {
            _typeToExcludes = new List<Type>() { typeof(Model), typeof(LinkModel), typeof(SelectableModel), typeof(BaseLinkModel), typeof(CustomizedLinkModel) };
        }





        public CustomizedLinkModel(DiagramRelationship relationship, Anchor source, Anchor target)
            : base(relationship.Uuid.ToString(), source, target)
        {
            
            this.Source = relationship;

            var properties = PropertyAccessor.GetProperties(GetType(), true).Where(c => !_typeToExcludes.Contains(c.DeclaringType)).ToList();
            foreach (var item in properties.Where(c => c.Name != "Source"))
            {
                var value = relationship.GetProperty(item.Name);
                if (!string.IsNullOrEmpty(value))
                    item.SetValue(this, ConverterHelper.ToObject(value, item.Type));
            }

        }

        public DiagramRelationship Source { get; }

        private static List<Type> _typeToExcludes;

    }


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

            foreach (var port in source.Ports)
                AddPort(new PortModel(port.Uuid.ToString(), this, port.Alignment));
            
            var properties = PropertyAccessor.GetProperties(GetType(), true).Where(c => !_typeToExcludes.Contains(c.DeclaringType)).ToList();
            foreach (var item in properties.Where(c => c.Name != "Source"))
            {
                var value = source.GetProperty(item.Name);
                if (!string.IsNullOrEmpty(value))
                    item.SetValue(this, ConverterHelper.ToObject(value, item.Type));
            }

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
        
            var properties = PropertyAccessor.GetProperties(GetType());
            foreach (var item in properties.Where(c => c.Name != "Source"))
                Source.SetProperty(item.Name, MyConverter.Serialize(item.GetValue(this)));
        
            Source.Position.X = this.Position.X;
            Source.Position.Y = this.Position.Y;
            Source.Name = this.Title;

        }

        public DiagramNode Source { get; }

        private static List<Type> _typeToExcludes;

    }

}
