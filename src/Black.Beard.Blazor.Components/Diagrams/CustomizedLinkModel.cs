using Blazor.Diagrams.Core.Models;
using Bb.ComponentModel.Accessors;
using Bb.Expressions;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models.Base;
using System.Text.Json.Serialization;
using Bb.TypeDescriptors;
using System.Text.Json;
using Bb.CustomComponents;

namespace Bb.Diagrams
{
    public class CustomizedLinkModel : LinkModel, IDynamicDescriptorInstance, IValidationService
    {


        static CustomizedLinkModel()
        {
            _typeToExcludes = new List<Type>() { typeof(Model), typeof(LinkModel), typeof(SelectableModel), typeof(BaseLinkModel), typeof(CustomizedLinkModel) };
        }


        public CustomizedLinkModel(DiagramRelationship relationship, Anchor source, Anchor target)
            : base(relationship.Uuid.ToString(), source, target)
        {

            this._container = new DynamicDescriptorInstanceContainer(this);
            this.Source = relationship;

            var properties2 = this._container.Properties();
            foreach (var item in properties2)
                item.Map(this, relationship.Properties.PropertyExists(item.Name), relationship.GetProperty(item.Name));

        }


        public void SynchronizeSource()
        {
            Source.Properties.CopyFrom(_container);
        }

        public object GetProperty(string name)
        {
            return this._container.GetProperty(name);
        }


        public void SetProperty(string name, object value)
        {
            this._container.SetProperty(name, value);
        }

        public virtual void Validate(Diagnostics Diagnostics)
        {

        }

        private readonly DynamicDescriptorInstanceContainer _container;

        [EvaluateValidation(false)]
        public DiagramRelationship Source { get; }

        private static List<Type> _typeToExcludes;

    }

}
