using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;

namespace Bb.Diagrams
{
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


        /// <summary>
        /// Convert ILinkable item to anchor
        /// </summary>
        /// <param name="sourceLink">link to convert</param>
        /// <param name="toolLink">tool that contains the method to create the anchor</param>
        /// <returns><see cref="Anchor"/></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Anchor ConvertToAnchor(this ILinkable sourceLink, DiagramToolRelationshipBase toolLink)
        {

            Anchor result;

            if (sourceLink is NodeModel model3)
                result = toolLink.CreateAnchor(model3);

            else
            {

                if (sourceLink is PortModel port2)
                    result = toolLink.CreateAnchor(port2);

                else
                    throw new NotImplementedException();

            }

            return result;

        }

    }


}
