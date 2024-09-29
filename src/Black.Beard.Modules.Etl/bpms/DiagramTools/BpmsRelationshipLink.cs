using Bb.Diagrams;
using Bb.UIComponents.Glyphs;
using Blazor.Diagrams.Core.Models;
using static MudBlazor.CategoryTypes;

namespace Bb.Modules.Bpms
{


    public class BpmsRelationshipLink : DiagramToolRelationshipBase
    {

        public BpmsRelationshipLink()
            : base(Key,
                   Bb.ComponentConstants.Relationships,
                   "Relation",
                   "bpms relation",
                   GlyphFilled.ArrowUpward)
        {

            IsDefaultLink = true;
            this.Hidden = true;
        }


        protected override void Customize(CustomizedLinkModel link)
        {
            //link.SourceMarker = LinkMarker.Arrow;
            //link.TargetMarker = LinkMarker.Arrow;
            //link.Labels.Add(new LinkLabelModel(link, "Arrow"));
        }

        public static Guid Key = new Guid("76B74FC3-6F7C-4327-86F3-1D5310DE27F0");

    }

}
