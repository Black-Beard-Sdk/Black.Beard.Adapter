using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Geometry;

namespace Bb.Diagrams
{
    public interface IDiagramNode
    {

        T? GetDiagram<T>() where T : Diagram;
        void SetDiagram<T>(T diagram) where T : Diagram;


        string Title { get; set; }
        List<Port> Ports { get; set; }
        
        Position Position { get; set; }

        Size? Size { get; set; }

        Properties Properties { get; set; }

        Guid Type { get; set; }
        Guid Uuid { get; set; }
        Guid? UuidParent { get; set; }

        Port AddPort(PortAlignment alignment, Guid id);
        Port GetPort(Guid id);
        Port GetPort(PortAlignment alignment);
        object? GetProperty(string name);
        void SetProperty(string name, object? value);
    }
}