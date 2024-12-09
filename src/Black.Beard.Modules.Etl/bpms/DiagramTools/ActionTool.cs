using Bb.Diagrams;
using Bb.Modules.Bpms.Models;
using Bb.UIComponents.Glyphs;
using Blazor.Diagrams.Core.Models;

namespace Bb.Modules.Bpms
{

    public class ActionTool : DiagramToolNode
    {

        public ActionTool()
            : base(Key,
                  Bb.ComponentConstants.Tools,
                  "Action",
                  "Append a new action in the process",
                  GlyphFilled.CropSquare)
        {
            this.AddParentType<BpmsSwimLane>();
            this.WithModel<BpmsAction>();
            //this.SetTypeUI<SwimLaneComponent>();
            //this.IsControlled(true);
            //this.IsLocked(false);

            this.AddPort(PortAlignment.Bottom);
            this.AddPort(PortAlignment.Top);
            this.AddPort(PortAlignment.Left);
            this.AddPort(PortAlignment.Right);

        }

        // public override string GetDefaultName() => $"Action";

        public static Guid Key = new Guid("ED597924-4809-4715-BEFF-9ADF954496FA");


    }

}
