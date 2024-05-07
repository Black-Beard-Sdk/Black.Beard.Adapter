using Bb.ComponentModel.Translations;
using Blazor.Diagrams.Core.Models;
using System.Text.Json.Serialization;

namespace Bb.Diagrams
{

    public class DiagramItemBase
    {

        public DiagramItemBase()
        {
            Ports = new List<Port>();
        }

        public Guid Uuid { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Guid Type { get; set; }

        public Position Position { get; set; }

        public List<Port> Ports { get; set; }

        public Port AddPort(PortAlignment alignment, Guid id)
        {
            var p = new Port() { Uuid = id, Alignment = alignment };
            Ports.Add(p);
            return p;
        }

        public Port GetPort(PortAlignment alignment)
        {
            return Ports.FirstOrDefault(c => c.Alignment == alignment);
        }

        public Port GetPort( Guid id)
        {
            return Ports.FirstOrDefault(c => c.Uuid == id);
        }


    }

    public class Port
    {

        public Port()
        {

        }

        public Guid Uuid { get; set; }

        public PortAlignment Alignment { get; set; }

    }

    public class Position
    {

        public Position()
        {

        }

        public Position(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }

        public double Y { get; set; }

    }

}
