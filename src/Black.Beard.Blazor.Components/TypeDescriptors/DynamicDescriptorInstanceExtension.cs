using Bb.Expressions;
using System.ComponentModel;
using System.Text.Json;

namespace Bb.TypeDescriptors
{
    public static class DynamicDescriptorInstanceExtension
    {


        public static void Map(this PropertyDescriptor self, object instance, bool exists, object data, JsonSerializerOptions? options)
        {

            object value = data;

            if (data == null)
                value = self.GetDefaultValue();

            else if (self.PropertyType != typeof(string) && data is string serializedData)
            {

                if (options == null)
                    options = new JsonSerializerOptions
                    {
                        Converters = { new DynamicDescriptorInstanceJsonConverter() },
                        // Other options as required
                        IncludeFields = true,  // You must set this if MyClass.Id and MyClass.Data are really fields not properties.
                        WriteIndented = true
                    };

                if (!string.IsNullOrEmpty(serializedData))
                    value = serializedData.Deserialize(self.PropertyType, options);

            }
            
            else if (self.PropertyType != data.GetType())
                value = ConverterHelper.ToObject(data, self.PropertyType);

            if (value != null)
                self.SetValue(instance, value);

        }

    }

}


