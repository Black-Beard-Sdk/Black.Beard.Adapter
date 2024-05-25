﻿using Bb.ComponentModel.Accessors;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bb.TypeDescriptors
{

    public class ConfigurationPropertyDescriptor
    {

        public ConfigurationPropertyDescriptor()
        {
            this.SetValue = _setValue;
            this.GetValue = _getValue;
            this._attributes = new List<Attribute>();
        }

        public ConfigurationPropertyDescriptor AddAttributes(params Attribute[] attributes)
        {
            this._attributes.AddRange(attributes);
            return this;
        }

        public bool AddedProperty { get; internal set; }

        public string Name { get; set; }

        public Type Type { get; set; }

        public Type ComponentType { get; set; }

        public bool CanResetValue { get; set; }

        public bool IsReadOnly { get; set; }

        public Action<object, object> SetValue { get; set; }

        public Func<object, object> GetValue { get; set; }

        public bool ShouldSerializeValue { get; set; }

        public string Description { get; set; }

        public string DisplayName { get; set; }

        public string Category { get; set; }

        public int? PropertyOrder { get; set; }

        public void _setValue(object component, object value)
        {

            var property = ResolveAccessor();

            if (property != null)
                property.SetValue(this, value);

            if (component is IDynamicDescriptorInstance i)
                i.SetProperty(Name, value);

            else
            {

            }

        }

        public object _getValue(object component)
        {

            var property = ResolveAccessor();
            if (property != null)
                return property.GetValue(component);

            if (component is IDynamicDescriptorInstance i)
                return i.GetProperty(Name);

            return null;

        }

        private AccessorItem? ResolveAccessor()
        {

            if (!_resolved)
                lock (_lock)
                    if (!_resolved)
                    {
                        var properties = PropertyAccessor.GetProperties(GetType(), true);
                        _property = properties.Where(c => c.Name == this.Name).FirstOrDefault();
                        _resolved = true;
                    }

            return _property;

        }

        internal void Merge(ConfigurationPropertyDescriptor property)
        {

            if (!string.IsNullOrEmpty(property.Name))
                Name = property.Name;

            if (!string.IsNullOrEmpty(property.Description))
                Description = property.Description;

            if (!string.IsNullOrEmpty(property.DisplayName))
                DisplayName = property.DisplayName;

            if (!string.IsNullOrEmpty(property.Category))
                Category = property.Category;

            if (property.CanResetValue)
                CanResetValue = property.CanResetValue;

            foreach (var item in property._attributes)
            {
                _attributes.Add(item);
            }

        }

        public Attribute[] Attributes => _attributes.ToArray();


        private readonly List<Attribute> _attributes;
        private AccessorItem? _property;
        private bool _resolved;
        private volatile object _lock = new object();

    }
}
