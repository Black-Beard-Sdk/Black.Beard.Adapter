using Bb.ComponentModel.Attributes;
using Bb.Diagrams;
using Bb.UIComponents.Glyphs;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models;

namespace Bb.Modules.Sgbd.DiagramTools
{

    [ExposeClass(SgbdDiagramFeature.Filter, ExposedType = typeof(DiagramSpecificationBase))]
    public class ConstraintRelationship : DiagramSpecificationRelationshipBase
    {

        public ConstraintRelationship()
            : base(new Guid(Key),
                   "Contraint",
                   "Create a constraint",
                   GlyphFilled.ArrowUpward)
        {

            IsDefaultLink = true;

        }


        public override Anchor CreateAnchor(NodeModel model)
        {
            return base.CreateAnchor(model);
        }

        public override Anchor CreateAnchor(PortModel model)
        {
            return base.CreateAnchor(model);
        }

        public override CustomizedLinkModel CreateLink(DiagramRelationship link, Anchor source, Anchor target)
        {
            return base.CreateLink(link, source, target);
        }

        public override CustomizedLinkModel CreateLink(DiagramRelationship link, PortModel source, PortModel target)
        {
            return base.CreateLink(link, source, target);
        }

        public override CustomizedLinkModel CreateLink(Guid uuid, Anchor source, Anchor target)
        {
            return base.CreateLink(uuid, source, target);
        }

        public override CustomizedLinkModel CreateLink(Guid uuid, PortModel source, PortModel target)
        {
            return base.CreateLink(uuid, source, target);
        }

        public override void SetTypeModel<T>()
        {
            base.SetTypeModel<T>();
        }

        public override void SetTypeUI<T>()
        {
            base.SetTypeUI<T>();
        }


        public const string Key = "0A385005-4391-42A9-B538-2C33E1266801";

    }

}
