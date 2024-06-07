using Bb.TypeDescriptors;
using System.ComponentModel;

namespace Bb.Modules.Sgbd.Models
{
    public static class TypeDescriptorExtension
    {

        public static string Value(this Column self, string propertyName)
        {

            var properties = TypeDescriptor.GetProperties(self);
            var property = properties.FirstOrDefault(c => c.Name == propertyName);

            if (property == null)
                throw new Exception($"Property {propertyName} not found with TypeDescriptor");

            var value = property.GetValue(self);

            if (value == null)
                return default;

            return value.ToString();

        }

        public static T? Value<T>(this object self, string propertyName)
        {

            var properties = TypeDescriptor.GetProperties(self);
            var property = properties.FirstOrDefault(c => c.Name == propertyName);
            
            if (property == null)
                throw new Exception($"Property {propertyName} not found with TypeDescriptor");

            var value = property.GetValue(self);

            if (value == null)
                return default;

            return (T)value;
        }


    }


}
