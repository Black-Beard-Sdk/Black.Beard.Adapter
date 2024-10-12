using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using Bb.TypeDescriptors;
using Bb.ComponentModel.Attributes;
using System.Text.Json.Serialization;

namespace Bb.Diagrams
{

    public class LinkProperties
    : IDynamicDescriptorInstance
    , IValidationService
    {


        static LinkProperties()
        {
            _typeToExcludes = new List<Type>() 
            { 
                typeof(Model), 
                typeof(LinkModel), 
                typeof(SelectableModel), 
                typeof(BaseLinkModel)
            };
        }


        public LinkProperties(SerializableRelationship relationship, LinkModel link)
        {

            this._container = new DynamicDescriptorInstanceContainer(this);
            this.Source = relationship;
            this.UILink = link;

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

        public void SetProperty(string name, object? value)
        {
            this._container.SetProperty(name, value);
        }

        public void Refresh()
        {

            bool hasChanged = false;
            foreach (CustomizedLinkLabelModel item in this.UILink.Labels)
            {
                if (item.Update())
                    hasChanged = true;
            }

            if (hasChanged)
            {
                this.UILink.Refresh();
                this.UILink.RefreshLinks();
            }

        }

        public virtual void Validate(DiagramDiagnostics Diagnostics)
        {

        }

        private readonly DynamicDescriptorInstanceContainer _container;

        [EvaluateValidation(false)]
        public SerializableRelationship Source { get; }

        public LinkModel UILink { get; }


        [JsonIgnore]
        public Blazor.Diagrams.Core.Diagram Diagram { get; internal set; }

        public Diagram Model { get; internal set; }

        [JsonIgnore]
        public Guid Uuid => Source.Uuid;

        internal void SetLabels(List<LabelCreator> models)
        {
            foreach (var item in models)
                this.UILink.Labels.Add(item.Create(this.UILink));
        }

        private static List<Type> _typeToExcludes;
    }

}
