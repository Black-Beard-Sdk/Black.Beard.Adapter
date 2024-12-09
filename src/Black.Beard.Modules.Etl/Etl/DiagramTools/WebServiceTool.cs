using Bb.ComponentModel.Attributes;
using Bb.Diagrams;
using Bb.Modules.Etl.Models;
using Bb.UIComponents.Glyphs;
using Blazor.Diagrams.Core.Models;

namespace Bb.Modules.Etl
{


    public class WebServiceTool : DiagramToolNode
    {

        public WebServiceTool()
            : base(Key,
                  "WebService",
                  Bb.ComponentConstants.Tools,
                  "Append a new WebService",
                  GlyphFilled.Service)
        {

            AddPort(PortAlignment.Right);

            this.WithModel<EtlWebService>();
            //this.SetTypeUI<CustomGroupWidget>();
            this.IsControlled(false);
        }

        public override string GetDefaultName()
        {
            return $"ws";
        }

        public static Guid Key = new Guid("802E6C01-E521-40CB-AE28-B35375974F21");

    }

}
