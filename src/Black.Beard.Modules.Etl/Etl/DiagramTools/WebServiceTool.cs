using Bb.ComponentModel.Attributes;
using Bb.Diagrams;
using Bb.UIComponents.Glyphs;
using Blazor.Diagrams.Core.Models;

namespace Bb.Modules.Etl.Etl
{


    [ExposeClass(EtlDiagramFeature.Filter, ExposedType = typeof(DiagramSpecificationBase))]
    public class WebServiceTool : DiagramSpecificationNodeBase
    {

        public WebServiceTool()
            : base(new Guid(Key),
                  "WebService",
                  "Append a new WebService",
                  GlyphFilled.Service)
        {

            AddPort(PortAlignment.Right);

        }

        public override string GetDefaultName()
        {
            return $"ws";
        }

        public const string Key = "802E6C01-E521-40CB-AE28-B35375974F21";

    }

}
