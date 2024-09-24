using Bb.ComponentModel.Translations;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;

namespace Bb.Diagrams
{
    public class DiagramToolRelationshipBase : DiagramToolBase
    {

        public DiagramToolRelationshipBase(Guid uuid, TranslatedKeyLabel category, TranslatedKeyLabel name, TranslatedKeyLabel description, string icon)
            : base(uuid, category, name, description, icon)
        {
            Kind = ToolKind.Link;
        }

        public override string GetDefaultName()
        {
            return $"Link";
        }


        public bool IsDefaultLink { get; protected set; }

        public virtual Anchor CreateAnchor(NodeModel model)
        {
            return new ShapeIntersectionAnchor(model);
        }

        public virtual Anchor CreateAnchor(PortModel model)
        {
            return new SinglePortAnchor(model);
        }

        public virtual CustomizedLinkModel CreateLink(Guid uuid, PortModel source, PortModel target)
        {
            return CreateLink(uuid, CreateAnchor(source), CreateAnchor(target));
        }

        public virtual CustomizedLinkModel CreateLink(Guid uuid, Anchor source, Anchor target)
        {

            Guid sourceId = source.Model.GetId();
            Guid targetId = target.Model.GetId();

            SerializableRelationship relationship = new SerializableRelationship()
            {
                Name = string.Empty,
                Uuid = uuid,
                Type = Uuid,
                Source = sourceId,
                Target = targetId,
            };

            return CreateLink(relationship, source, target);

        }

        public virtual CustomizedLinkModel CreateLink(SerializableRelationship link, PortModel source, PortModel target)
        {
            var l = CreateLink(link, CreateAnchor(source), CreateAnchor(target));
            return l;
        }

        public virtual CustomizedLinkModel CreateLink(SerializableRelationship link, Anchor source, Anchor target)
        {
            var l = new CustomizedLinkModel(link, source, target);
            return l;
        }
    }


    internal static class LinkableExtensions
    {

        public static Guid GetId(this ILinkable source)
        {

            if (source is NodeModel model)
                return new Guid(model.Id);

            if (source is PortModel port)
                return new Guid(port.Id);

            return Guid.Empty;

        }

    }


}
