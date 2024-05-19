using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using static MudBlazor.CategoryTypes;

namespace Bb.TypeDescriptors
{

    class DynamicTypeDescriptor : CustomTypeDescriptor
    {

        public DynamicTypeDescriptor(ICustomTypeDescriptor parent, object instance, ConfigurationDescriptorSelector configuration)
            : base(parent)
        {
            this._instance = instance;
            _configurationSelector = configuration;
        }


        public override PropertyDescriptorCollection GetProperties()
        {

            var initialList = base.GetProperties();

            if (_configurationSelector != null)
                return BuildNewListOfProperty(initialList);

            return initialList;

        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {

            var initialList = base.GetProperties(attributes);

            if (_configurationSelector != null)
                return BuildNewListOfProperty(initialList);

            return initialList;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private PropertyDescriptorCollection BuildNewListOfProperty(PropertyDescriptorCollection initialList)
        {

            var toExcluded = _configurationSelector.ExcludedProperties;

            var customFields = initialList
                .Cast<PropertyDescriptor>()
                .Where(c => !toExcluded.Contains(c.Name))
                .ToDictionary(c => c.Name);

            foreach (var item in _configurationSelector.Get(_instance))
                if (item.ComponentType.IsInstanceOfType(_instance))
                    foreach (var property in item.Properties)
                        if (!toExcluded.Contains(property.Name))
                        {
                            if (!customFields.ContainsKey(property.Name))
                                customFields.Add(property.Name, property);
                            else
                                customFields[property.Name] = property;
                        }

            return new PropertyDescriptorCollection(customFields.Values.ToArray());

        }

        private ConfigurationDescriptorSelector _configurationSelector;
        private object _instance;
    }


}
