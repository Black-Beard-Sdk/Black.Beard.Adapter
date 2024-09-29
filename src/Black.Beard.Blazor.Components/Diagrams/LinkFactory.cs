using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;

namespace Bb.Diagrams
{

    public class LinkFactory
    {

        public LinkFactory(DiagramToolbox toolbox)
        {
            this._toolbox = toolbox;
            //this._specifications = toolbox.Where(c => c.Kind == ToolKind.Link).ToDictionary(c => c.Uuid.ToString());
        }


        public virtual LinkModel CreateLinkModel
        (
            DiagramToolRelationshipBase toolLink,
            Blazor.Diagrams.Core.Diagram diagram,
            ILinkable source,
            Anchor targetAnchor
        )
        {

            if (toolLink != null)
            {

                Anchor source2;

                if (source is NodeModel model3)
                    source2 = toolLink.CreateAnchor(model3);

                else
                {

                    if (source is PortModel port2)
                        source2 = toolLink.CreateAnchor(port2);

                    else
                        throw new NotImplementedException();

                }

                var result = toolLink.CreateLink(Guid.NewGuid(), source2, targetAnchor);

                toolLink.Customize(result);

                return result;
            }

            return null;

        }

        private readonly DiagramToolbox _toolbox;
       
    }

}
