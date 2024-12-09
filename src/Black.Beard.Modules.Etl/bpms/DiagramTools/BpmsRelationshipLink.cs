using Bb.Diagrams;
using Bb.UIComponents.Glyphs;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;

namespace Bb.Modules.Bpms
{

    public class BpmsRelationshipLink : DiagramToolRelationshipBase
    {

        public BpmsRelationshipLink()
            : base(Key,
                   Bb.ComponentConstants.Relationship,
                   "Relation",
                   "bpms relation",
                   GlyphFilled.ArrowUpward)
        {

            IsDefaultLink = true;
            this.Hidden = true;

            SetTargetMarker(LinkMarker.Arrow);

            AddLabel("Arrow", -40, new Point(0, -30));

        }

        protected override LinkProperties Customize(LinkProperties link)
        {
            return base.Customize(link);
        }

        public static Guid Key = new Guid("76B74FC3-6F7C-4327-86F3-1D5310DE27F0");

    }

}
