using Bb.ComponentModel.Accessors;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Bb.TypeDescriptors
{

    [JsonConverter(typeof(DynamicDescriptorInstanceJsonConverter))]
    public class DynamicDescriptorInstanceContainer : IDynamicDescriptorInstance
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDescriptorInstanceContainer"/> class.
        /// </summary>
        /// <param name="parent"></param>
        public DynamicDescriptorInstanceContainer(object parent)
        {
            this._instance = parent;
            _realProperties = PropertyAccessor.GetProperties(parent.GetType(), AccessorStrategyEnum.Direct);
            _properties = new Dictionary<string, object>();
        }

        /// <summary>
        /// return the value of the property in string format
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string? GetPropertyToString(string name)
        {

            var value = GetProperty(name);

            if (value == null)
                return null;

            if (value is string s)
                return s;

            return value.Serialize(false);

        }

        /// <summary>
        /// return the value of the property specified by the name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetProperty(string name)
        {

            if (_realProperties.TryGetValue(name, out var accessor))
                return accessor.GetValue(this._instance);

            if (_properties.TryGetValue(name, out object value))
                return value;

            var property = DynamicProperties().Where(c => c.Name == name).FirstOrDefault();
            if (property != null)
                return property.GetDefaultValue();

            return null;

        }

        /// <summary>
        /// Set the value of the property specified by the name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetProperty(string name, object? value)
        {

            if (_realProperties.TryGetValue(name, out var accessor))
                accessor.SetValue(this._instance, value);
            
            else if (_properties.TryGetValue(name, out object? v))
            {
                if (v == value)
                    _properties.Remove(name);
                else
                    _properties[name] = value;
            }
            else
                _properties.Add(name, value);
        }

        public IEnumerable<PropertyDescriptor> Properties()
        {
            var properties = TypeDescriptor.GetProperties(Instance).OfType<PropertyDescriptor>().ToList();
            return properties;
        }

        public IEnumerable<PropertyDescriptor> DynamicProperties()
        {
            var properties = TypeDescriptor.GetProperties(Instance).OfType<DynamicPropertyDescriptor>().ToList();
            return properties;
        }

        public object Instance => _instance;

        private readonly object _instance;
        private readonly Dictionary<string, object> _properties;
        private readonly AccessorList _realProperties;
    }

}


