using System.Text.Json.Serialization;
using System.Text.Json;
using System.ComponentModel;
using System.Reflection;
using ICSharpCode.Decompiler.TypeSystem;

namespace Bb.TypeDescriptors
{


    public static class CustomSubPropertyJsonConverter
    {

        static CustomSubPropertyJsonConverter()
        {
            _method = typeof(CustomSubPropertyJsonConverter)
                .GetMethod(nameof(CreateInstance)
                , BindingFlags.Public | BindingFlags.Static);
        }

        public static JsonSerializerOptions AppendConverterFor(this JsonSerializerOptions self, Type type, Action<object> initializer)
        {
            self.Converters.Add(type.Create(initializer));
            return self;
        }

        public static JsonConverter Create(this Type type, Action<object> initializer)
        {
            var result = (JsonConverter)_method
                .MakeGenericMethod(type)
                .Invoke(null, new object[] { initializer });
            return result;
        }


        public static CustomSubPropertyJsonConverter<T> CreateInstance<T>(Action<object> initializer)
            where T : IDynamicDescriptorInstance, new()
        {
            var converter = new CustomSubPropertyJsonConverter<T>();
            converter.OnInitializing = initializer;
            return converter;
        }

        private static readonly MethodInfo? _method;

    }

    public interface IInitializer
    {

        Action<object> OnInitializing { get; set; }

    }

    public class CustomSubPropertyJsonConverter<T> : JsonConverter<T>, IInitializer
        where T : IDynamicDescriptorInstance, new()
    {

        public CustomSubPropertyJsonConverter()
        {

        }


        public Action<object> OnInitializing { get; set; }

        public override bool CanConvert(Type typeToConvert)
        {
            return base.CanConvert(typeToConvert);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {

            var properties = TypeDescriptor.GetProperties(value)
                .OfType<PropertyDescriptor>()
                .Where(c => Include(c))
                .OrderBy(c => c.Name)
                .ToList();

            writer.WriteStartObject();

            foreach (var item in properties)
            {
                var propertyValue = item.GetValue(value);
                if (propertyValue != null)
                {
                    writer.WritePropertyName(item.Name);
                    JsonSerializer.Serialize(writer, propertyValue, options);
                }
            }

            writer.WriteEndObject();

        }

        private bool Include(PropertyDescriptor c)
        {

            if (c.IsReadOnly)
                return false;

            if (c.Attributes.OfType<JsonIgnoreAttribute>().Any())
                return false;

            return true;
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {

            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            var instance = new T();

            var properties = TypeDescriptor.GetProperties(instance)
                .OfType<PropertyDescriptor>()
                .ToDictionary(c => c.Name);

            while (reader.Read())
            {

                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    if (OnInitializing != null)
                        OnInitializing(instance);
                    return instance;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException();

                // Get the property name.
                string? propertyName = reader.GetString();

                if (properties.TryGetValue(propertyName, out PropertyDescriptor? property)) // Get the value.
                {
                    var value = JsonSerializer.Deserialize(ref reader, property.PropertyType, options);
                    property.SetValue(instance, value);
                }
                else
                {

                    reader.Read();

                    switch (reader.TokenType)
                    {
                        case JsonTokenType.None:
                            break;
                        case JsonTokenType.StartObject:
                            break;
                        case JsonTokenType.EndObject:
                            break;
                        case JsonTokenType.StartArray:
                            break;
                        case JsonTokenType.EndArray:
                            break;
                        case JsonTokenType.PropertyName:
                            break;
                        case JsonTokenType.Comment:
                            break;
                        case JsonTokenType.String:
                            break;
                        case JsonTokenType.Number:
                            break;
                        case JsonTokenType.True:
                            break;
                        case JsonTokenType.False:
                            break;
                        case JsonTokenType.Null:
                            break;
                        default:
                            break;
                    }
                }

            }

            throw new JsonException();


        }

    }

}
