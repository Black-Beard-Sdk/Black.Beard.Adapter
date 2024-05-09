using Bb.ComponentModel.Attributes;
using Bb.Diagrams;
using Bb.UIComponents.Glyphs;

namespace Bb.Modules.Etl.DiagramTools
{
    [ExposeClass(EtlDiagramFeature.Filter, ExposedType = typeof(DiagramSpecificationBase))]
    public class CallRelationshipTool : DiagramSpecificationRelationshipBase
    {

        public CallRelationshipTool()
            : base(new Guid(Key),
                   "Call",
                   "Call a WebService",
                   GlyphFilled.ArrowUpward)
        {
            
            IsDefaultLink = true;

        }

        public const string Key = "802E6C01-E521-40CB-AE28-B35375974F22";

    }

}
