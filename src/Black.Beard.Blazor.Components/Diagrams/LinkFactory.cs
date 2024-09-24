using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;

namespace Bb.Diagrams
{

    public class LinkFactory
    {

        public LinkFactory(IEnumerable<DiagramToolBase> specifications)
        {
            this._specifications = specifications.Where(c => c.Kind == ToolKind.Link).ToDictionary(c => c.Uuid.ToString());
        }


        public virtual LinkModel CreateLinkModel
        (
            DiagramToolRelationshipBase current,
            Blazor.Diagrams.Core.Diagram diagram,
            ILinkable source,
            Anchor targetAnchor
        )
        {

            if (current != null)
            {

                Anchor source2;

                if (source is NodeModel model3)
                    source2 = current.CreateAnchor(model3);

                else
                {

                    if (source is PortModel port2)
                        source2 = current.CreateAnchor(port2);

                    else
                        throw new NotImplementedException();

                }

                return current.CreateLink(Guid.NewGuid(), source2, targetAnchor);

            }

            return null;

        }

        private readonly Dictionary<string, DiagramToolBase> _specifications;

    }

}
