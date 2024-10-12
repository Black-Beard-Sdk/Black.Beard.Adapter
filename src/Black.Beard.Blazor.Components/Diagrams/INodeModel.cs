using Bb.TypeDescriptors;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using System.ComponentModel;

namespace Bb.Diagrams
{
    public interface INodeModel
        : IDynamicDescriptorInstance
        , IValidationService
        , INotifyPropertyChanged
    {

        SerializableDiagramNode Source { get; }

        Point Position { get; set; }

        Size? Size { get; set; }

        void SynchronizeSource();

        bool ContainsPoint(Point position);
        bool ContainsPoint(Position position);
        PortModel CreatePort(Port port);
        void OnPropertyChanged(string propertyName);


        String Id { get; }


        IReadOnlyList<PortModel> Ports { get; }

        IReadOnlyList<BaseLinkModel> Links { get; }

        PortModel AddPort(PortAlignment alignment = PortAlignment.Bottom);

        PortModel? GetPort(PortAlignment alignment);

        T? GetPort<T>(PortAlignment alignment) where T : PortModel;

        bool RemovePort(PortModel port);

        void RefreshAll();

        void RefreshLinks();

        void ReinitializePorts();

        void SetPosition(double x, double y);

        void UpdatePositionSilently(double deltaX, double deltaY);

        bool CanAttachTo(ILinkable other);


        GroupModel? Group { get; }

        void SetAvailableParents(HashSet<Type> parentTypes, bool canBeWithoutParent);
        
        bool CanAcceptLikeParent(INodeModel parent);
        void InitializeFirst(DiagramToolNode diagramToolNodeBase);

        bool CanBeOrphaned { get; }



    }
}