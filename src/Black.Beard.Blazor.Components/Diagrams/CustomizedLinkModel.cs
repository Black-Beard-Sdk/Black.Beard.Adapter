using Blazor.Diagrams.Core.Models;
using Bb.ComponentModel.Accessors;
using Bb.Expressions;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models.Base;
using System.Text.Json.Serialization;

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

            var properties = PropertyAccessor.GetProperties(GetType(), true)
                .Where(c => !_typeToExcludes.Contains(c.DeclaringType) && c.CanRead && c.CanWrite)
                .ToList();

            foreach (var item in properties)
            {
                var value = relationship.GetProperty(item.Name);
                if (!string.IsNullOrEmpty(value))
                    item.SetValue(this, ConverterHelper.ToObject(value, item.Type));
            }

        }

        public DiagramRelationship Source { get; }

        private static List<Type> _typeToExcludes;

    }

}
