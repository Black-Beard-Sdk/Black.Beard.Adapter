using Bb.ComponentModel.Translations;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;

namespace Bb.Diagrams
{
    public class DiagramSpecificationRelationshipBase : DiagramSpecificationBase
    {

        public DiagramSpecificationRelationshipBase(Guid uuid, TranslatedKeyLabel name, TranslatedKeyLabel description, string icon)
            : base(uuid, name, description, icon)
        {
            Category = Bb.ComponentConstants.Relationships;
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

            DiagramRelationship relationship = new DiagramRelationship()
            {
                Name = string.Empty,
                Uuid = uuid,
                Type = Uuid,
                Source = sourceId,
                Target = targetId,
            };

            return CreateLink(relationship, source, target);

        }

        public virtual CustomizedLinkModel CreateLink(DiagramRelationship link, PortModel source, PortModel target)
        {
            var l = CreateLink(link, CreateAnchor(source), CreateAnchor(target));
            return l;
        }

        public virtual CustomizedLinkModel CreateLink(DiagramRelationship link, Anchor source, Anchor target)
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
