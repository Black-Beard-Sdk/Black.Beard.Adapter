using Bb.ComponentModel.Attributes;
using Bb.Diagrams;
using Bb.UIComponents.Glyphs;

namespace Bb.Modules.Etl
{

    [ExposeClass(EtlDiagramFeature.Filter, ExposedType = typeof(DiagramToolBase))]
    public class ConstraintRelationship : DiagramToolRelationshipBase
    {

        public ConstraintRelationship()
            : base(new Guid(Key),
                  Bb.ComponentConstants.Relationships,
                   "Call",
                   "Call a WebService",
                   GlyphFilled.ArrowUpward)
        {

            IsDefaultLink = true;

        }

        public const string Key = "802E6C01-E521-40CB-AE28-B35375974F22";

    }

}
