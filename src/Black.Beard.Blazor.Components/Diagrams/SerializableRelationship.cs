using System.Text.Json.Serialization;

namespace Bb.Diagrams
{


    public class SerializableRelationship
    {

        public SerializableRelationship()
        {
            Properties = new Properties();
        }

        public Guid Uuid { get; set; }

        public string Name { get; set; }

        public Guid Type { get; set; }

        public Guid Source { get; set; }

        public Guid Target { get; set; }


        public Properties Properties { get; set; }

        public void SetProperty(string name, string value) => Properties.SetProperty(name, value);

        public string GetProperty(string name) => Properties.GetProperty(name);        

        [JsonIgnore]
        public Diagram Diagram { get; internal set; }

    }

}
