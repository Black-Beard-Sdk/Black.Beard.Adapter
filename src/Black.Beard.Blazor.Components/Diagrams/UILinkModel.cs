using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using Bb.TypeDescriptors;
using Bb.ComponentModel.Attributes;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.ComponentModel;

namespace Bb.Diagrams
{

    public class LinkProperties
    : IDynamicDescriptorInstance
    , IValidationService
    , INotifyPropertyChanged

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

            _excludes = new HashSet<string>()
            {
                "Source",
                "UILink",
                "Diagram",
                "Model"
            };

        }


        public LinkProperties(SerializableRelationship relationship, LinkModel link)
        {

            _options = new JsonSerializerOptions
            {
                Converters = { new DynamicDescriptorInstanceJsonConverter() },
                // Other options as required
                IncludeFields = true,  // You must set this if MyClass.Id and MyClass.Data are really fields not properties.
                WriteIndented = true
            };

            this._container = new DynamicDescriptorInstanceContainer(this);
            this.Source = relationship;
            this.UILink = link;

            this.Source.PropertyChanged += Source_PropertyChanged;

            var properties2 = this._container
                .Properties()
                .Where(c => !c.IsReadOnly && !_excludes.Contains(c.Name))
                .ToArray();

            foreach (var item in properties2)
                item.Map(this, relationship.Properties.PropertyExists(item.Name), relationship.GetProperty(item.Name), _options);

        }

        protected virtual void Source_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        public void SynchronizeSource()
        {
            Source.CopyFrom(_container);
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
            foreach (UILinkLabelModel item in this.UILink.Labels)
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
        public Guid Uuid => Source.Uuid;

        internal void SetLabels(List<LabelCreator> models)
        {
            foreach (var item in models)
                this.UILink.Labels.Add(item.Create(this.UILink));
        }


        #region INotifyPropertyChanged

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion INotifyPropertyChanged


        private static List<Type> _typeToExcludes;
        private static readonly HashSet<string> _excludes;
        protected JsonSerializerOptions _options;
    }

}
