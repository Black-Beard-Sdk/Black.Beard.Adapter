using Bb.ComponentModel.Attributes;
using Bb.Diagrams;
using Bb.UIComponents.Glyphs;

namespace Bb.Modules.Etl.DiagramTools
{


    [ExposeClass(EtlDiagramFeature.Filter, ExposedType = typeof(DiagramItemSpecificationBase))]
    public class WebServiceTool : DiagramModelSpecificationBase
    {

        public WebServiceTool()
            : base(new Guid(Key),
                  "WebService",
                  "Append a new WebService",
                  GlyphSharp.CleaningServices.Value)
        {
            
        }

        public const string Key = "802E6C01-E521-40CB-AE28-B35375974F21";

    }

}
