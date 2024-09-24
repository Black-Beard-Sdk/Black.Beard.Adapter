using Bb.ComponentModel.Attributes;
using Bb.Diagrams;
using Bb.Modules.Etl.Models;
using Bb.UIComponents.Glyphs;
using Blazor.Diagrams.Core.Models;

namespace Bb.Modules.Etl
{


    [ExposeClass(EtlDiagramFeature.Filter, ExposedType = typeof(DiagramToolBase))]
    public class WebServiceMethodTool : DiagramToolNode
    {

        public WebServiceMethodTool()
            : base(new Guid(Key),
                  Bb.ComponentConstants.Tools,
                  "WebService method",
                  "Append a new method in WebService",
                  GlyphFilled.Message)
        {

            AddPort(PortAlignment.Right);

            this.AddParentType<BpmsSwimLane>();
            this.SetTypeModel<EtlWebMethodService>();
            //this.SetTypeUI<TableNode>();

        }

        public override string GetDefaultName()
        {
            return $"ws_method";
        }

        public const string Key = "071E9894-836E-435A-ADDC-FA7748609AC3";

    }

}
