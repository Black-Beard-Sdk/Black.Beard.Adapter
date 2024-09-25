using Bb.ComponentModel.Attributes;
using Bb.Diagrams;
using Bb.UIComponents.Glyphs;

namespace Bb.Modules.Etl
{

    public class ConstraintRelationship : DiagramToolRelationshipBase
    {

        public ConstraintRelationship()
            : base(Key,
                  Bb.ComponentConstants.Relationships,
                   "Call",
                   "Call a WebService",
                   GlyphFilled.ArrowUpward)
        {

            IsDefaultLink = true;

        }

        public static Guid Key = new Guid("802E6C01-E521-40CB-AE28-B35375974F22");

    }

}
