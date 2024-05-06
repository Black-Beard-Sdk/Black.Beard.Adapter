using System.Text.Json.Serialization;

namespace Bb.Diagrams
{

    public class Diagram
    {

        public Diagram()
        {
            Specifications = new List<DiagramToolBase>();
            Models = new List<Diagram>();
            Relationships = new List<DiagramRelationship>();

        }

        public string Name { get; set; }

        public string Description { get; set; }

        [JsonIgnore]
        public List<DiagramToolBase> Specifications { get; set; }

        public List<Diagram> Models { get; set; }

        public List<DiagramRelationship> Relationships { get; set; }

    }


}
