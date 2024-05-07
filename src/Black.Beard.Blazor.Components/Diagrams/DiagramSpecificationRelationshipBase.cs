using Bb.ComponentModel.Translations;
using Bb.UIComponents;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models;

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


        public virtual LinkModel CreateLink(PortModel source, PortModel target)
        {
            var sourceAnchor = new SinglePortAnchor(source);
            var targetAnchor = new SinglePortAnchor(target);

            var link = new LinkModel(sourceAnchor, targetAnchor);
            return link;
        }

    }

}
