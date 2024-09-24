using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;

namespace Bb.Diagrams
{
    public class AnchorFactory
    {

        public AnchorFactory()
        {

        }

        public virtual Anchor CreateLinkModel
        (
            DiagramToolRelationshipBase current,
            Blazor.Diagrams.Core.Diagram diagram,
            BaseLinkModel source,
            ILinkable model
        )
        {

            if (model is NodeModel model2)
                return current.CreateAnchor(model2);

            if (!(model is PortModel port))
                throw new ArgumentOutOfRangeException("model", model, null);

            return current.CreateAnchor(port);

        }

    }

}
