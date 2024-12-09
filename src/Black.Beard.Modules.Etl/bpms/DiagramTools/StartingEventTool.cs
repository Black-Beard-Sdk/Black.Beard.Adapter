using Bb.Diagrams;
using Bb.Modules.Bpms.Models;
using Bb.UIComponents.Glyphs;
using Blazor.Diagrams.Core.Models;

namespace Bb.Modules.Bpms
{
    public class StartingEventTool : DiagramToolNode
    {

        public StartingEventTool()
            : base(Key,
                  Bb.ComponentConstants.Tools,
                  "Event",
                  "Append a new starting event",
                  GlyphFilled.SwipeRightAlt)
        {
            this.AddParentType<BpmsSwimLane>();
            this.WithModel<BpmsStartingEvent>();
            //this.SetTypeUI<SwimLaneComponent>();
            //this.IsControlled(true);
            //this.IsLocked(false);

            this.AddPort(PortAlignment.Bottom);
            this.AddPort(PortAlignment.Top);
            this.AddPort(PortAlignment.Left);
            this.AddPort(PortAlignment.Right);

        }

        // public override string GetDefaultName() => $"Event";

        public static Guid Key = new Guid("D28C2545-4B6E-4F40-8E2B-01CA062DC832");


    }

}
