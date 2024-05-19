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
            Properties = new Properties();
        }

        public Guid Uuid { get; set; }

        public string Name { get; set; }

        public Guid Type { get; set; }

        public Position Position { get; set; }

        public List<Port> Ports { get; set; }

        public Properties Properties { get; set; }

        public void SetProperty(string name, string value) => Properties.SetProperty(name, value);

        public string GetProperty(string name) => Properties.GetProperty(name);

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
