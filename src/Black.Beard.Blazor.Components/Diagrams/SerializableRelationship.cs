using Bb.ComponentModel.Accessors;
using Bb.TypeDescriptors;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bb.Diagrams
{


    public class SerializableRelationship
    {

        public SerializableRelationship()
        {
            Properties = new Properties();
            _realProperties = this.GetType().GetAccessors(AccessorStrategyEnum.Direct);
            _options = new JsonSerializerOptions
            {
                Converters = { new DynamicDescriptorInstanceJsonConverter() },
                // Other options as required
                IncludeFields = true,  // You must set this if MyClass.Id and MyClass.Data are really fields and not a property.
                WriteIndented = true
            };
        }

        public Guid Uuid { get; set; }

        public string Name { get; set; }

        public Guid Type { get; set; }

        public Guid Source { get; set; }

        public Guid Target { get; set; }


        public Properties Properties { get; set; }

        public void SetProperty(string name, object? value)
        {

            if (_realProperties.TryGetValue(name, out var accessor))
                accessor.SetValue(this, value);

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

        [JsonIgnore]
        public Diagram Diagram { get; internal set; }

        private readonly AccessorList _realProperties;
        protected JsonSerializerOptions _options;
    
    }

}
