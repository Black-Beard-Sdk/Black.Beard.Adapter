using Bb.ComponentModel.Attributes;
using Bb.Diagrams;
using Bb.Modules.Etl.Models;
using Bb.UIComponents.Glyphs;
using Blazor.Diagrams.Core.Models;

namespace Bb.Modules.Etl
{


    [ExposeClass(EtlDiagramFeature.Filter, ExposedType = typeof(DiagramToolBase))]
    public class WebServiceTool : DiagramToolNode
    {

        public WebServiceTool()
            : base(new Guid(Key),
                  "WebService",
                  Bb.ComponentConstants.Tools,
                  "Append a new WebService",
                  GlyphFilled.Service)
        {

            AddPort(PortAlignment.Right);

            this.SetTypeModel<EtlWebService>();
            //this.SetTypeUI<CustomGroupWidget>();
            this.IsControlled(false);
        }

        public override string GetDefaultName()
        {
            return $"ws";
        }

        public const string Key = "802E6C01-E521-40CB-AE28-B35375974F21";

    }

}
