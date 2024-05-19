using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Bb.TypeDescriptors
{

    /// <summary>
    /// Configuration descriptor
    /// </summary>
    public class ConfigurationDescriptor
    {

        /// <summary>
        /// initializes a new instance of the <see cref="ConfigurationDescriptor"/> class.
        /// </summary>
        public ConfigurationDescriptor(Type componentType)
        {
            ComponentType = componentType;
            this._customs = new List<PropertyDescriptor>();
            this._excludedProperties = new HashSet<string>();
        }

        /// <summary>
        /// Add a property to the configuration descriptor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="initializer"></param>
        /// <returns></returns>
        public ConfigurationDescriptor AddProperty(string name, Type type, Action<ConfigurationPropertyDescriptor> initializer = null)
        {

            var property = new ConfigurationPropertyDescriptor()
            {
                Name = name,
                Type = type,
                ComponentType = ComponentType
            };

            if (initializer != null)
                initializer(property);

            AddProperties(property);

            return this;
        }

        /// <summary>
        /// add properties to the configuration descriptor
        /// </summary>
        /// <param name="customs"></param>
        /// <returns></returns>
        public ConfigurationDescriptor AddProperties(params ConfigurationPropertyDescriptor[] customs)
        {
            
            foreach (var item in customs)
                if (_excludedProperties.Contains(item.Name))
                    _excludedProperties.Remove(item.Name);


            this._customs.AddRange(customs.Select(c => new DynamicPropertyDescriptor(c))
                .Cast<PropertyDescriptor>());
            return this;
        }

        /// <summary>
        /// remove properties from the configuration descriptor and mark the property as excluded
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public ConfigurationDescriptor RemoveProperties(params string[] names)
        {

            foreach (var item in names)
            {
                _excludedProperties.Add(item);
                this._customs.Where(c => c.Name == item).ToList().ForEach(c => this._customs.Remove(c));
            }

            return this;

        }


        public IEnumerable<string> ExcludedProperties => _excludedProperties;


        public PropertyDescriptor[] Properties => _customsArray ?? (_customsArray = _customs.ToArray());

        public Type ComponentType { get; }
        public Func<object, bool> Filter { get; internal set; }

        private List<PropertyDescriptor> _customs;
        private HashSet<string> _excludedProperties;
        private PropertyDescriptor[] _customsArray;

    }

}
