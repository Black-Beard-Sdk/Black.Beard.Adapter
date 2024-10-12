using Bb.TypeDescriptors;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
namespace Bb.Diagrams
{
    public class SerializableDiagramNodeJsonConverter : JsonConverter<SerializableDiagramNode>
    {

        public override SerializableDiagramNode? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {

            SerializableDiagramNode value = null;

            JsonObject jsonObject = JsonNode.Parse(ref reader).AsObject();
            JsonNode? typename = jsonObject["TypeNode"];
            if (typename == null)
                value = jsonObject.Deserialize<SerializableDiagramNode>(options);

            else
            {

                var type = Type.GetType(typename.AsValue().GetValue<string>());

                if (type != null)
                    value = (SerializableDiagramNode)jsonObject.Deserialize(type);

            }

            if (value != null)
                foreach (var item in options.Converters.OfType<IInitializer>())
                    item.OnInitializing(value);        

            return value;

        }

        public override void Write(Utf8JsonWriter writer, SerializableDiagramNode value, JsonSerializerOptions options)
        {

            JsonSerializer.Serialize(writer, value, value.GetType());

        }
    }

}
