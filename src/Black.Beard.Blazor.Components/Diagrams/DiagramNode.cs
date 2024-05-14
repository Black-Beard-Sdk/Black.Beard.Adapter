using Blazor.Diagrams.Core.Models;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Bb.Diagrams
{

    public class DiagramNode
    {

        public DiagramNode()
        {
            Ports = new List<Port>();
            Properties = new List<Property>();
        }

        public Guid Uuid { get; set; }

        public string Name { get; set; }

        public Guid Type { get; set; }

        public Position Position { get; set; }

        public List<Port> Ports { get; set; }

        public List<Property> Properties { get; set; }

        public void SetProperty(string name, string value)
        {
            var property = Properties.FirstOrDefault(c => c.Name == name);
            if (property == null)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Properties.Add(new Property() { Name = name, Value = value });
                    this.Properties.Sort((x, y) => x.Name.CompareTo(y.Name));
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(value))
                    property.Value = value;
                else
                    Properties.Remove(property);
            }
        }

        public string GetProperty(string name)
        {
            var property = Properties.FirstOrDefault(c => c.Name == name);
            if (property != null)
                return property.Value;
            return null;
        }

        public Port AddPort(PortAlignment alignment, Guid id)
        {

            var p = Ports.FirstOrDefault(c => c.Uuid == id);
            if (p == null)
            {
                p = new Port() { Uuid = id, Alignment = alignment };
                Ports.Add(p);
            }
            else
            {
                if (p.Alignment != alignment)
                {
                    p.Alignment = alignment;
                }

            }
            return p;
        }

        public Port GetPort(PortAlignment alignment)
        {
            return Ports.FirstOrDefault(c => c.Alignment == alignment);
        }

        public Port GetPort(Guid id)
        {
            return Ports.FirstOrDefault(c => c.Uuid == id);
        }


        [JsonIgnore]
        public Diagram Diagram { get; internal set; }

    }


}
