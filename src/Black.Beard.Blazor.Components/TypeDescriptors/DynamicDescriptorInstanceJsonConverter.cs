using System.Text.Json.Serialization;
using System.Text.Json;
using System.Reflection;

namespace Bb.TypeDescriptors
{

    public partial class DynamicDescriptorInstanceJsonConverter : JsonConverterFactory
    {

        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(IDynamicDescriptorInstance).IsAssignableFrom(typeToConvert);
        }

        public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
        {

            JsonConverter converter = null;

            var ctor = type.GetConstructor(new Type[0]);
            if (ctor != null)
            {

                converter = (JsonConverter)Activator.CreateInstance
                (
                    typeof(CustomSubPropertyJsonConverter<>).MakeGenericType(type),
                    BindingFlags.Instance | BindingFlags.Public,
                    binder: null,
                    args: null,
                    culture: null
                )!;

            }

            converter = (JsonConverter)Activator.CreateInstance
            (
                typeof(CustomSubPropertyJsonConverter<>).MakeGenericType(type),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: null,
                culture: null
            )!;

            return converter;
        }

    }

}
