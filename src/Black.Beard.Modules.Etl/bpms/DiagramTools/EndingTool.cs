using Bb.Diagrams;
using Bb.Modules.Bpms.Models;
using Bb.UIComponents.Glyphs;

namespace Bb.Modules.Bpms
{
    public class EndingTool : DiagramToolNode
    {

        public EndingTool()
            : base(Key,
                  Bb.ComponentConstants.Tools,
                  "End",
                  "Append a new ending process",
                  GlyphFilled.StopCircle)
        {
            
            this.AddParentType<BpmsSwimLane>();
            this.WithModel<BpmsEnding>();
            //this.SetTypeUI<SwimLaneComponent>();
            //this.IsControlled(true);
            //this.IsLocked(false);
        }

        // public override string GetDefaultName() => $"End";

        public static Guid Key = new Guid("D11D1F73-43B0-48CA-84C2-7B8F58CB4F30");


    }

}
