using Bb.Diagrams;
using Bb.Modules.Bpms.Models;
using Bb.UIComponents.Glyphs;

namespace Bb.Modules.Bpms
{
    public class SwitchTool : DiagramToolNode
    {

        public SwitchTool()
            : base(Key,
                  Bb.ComponentConstants.Tools,
                  "Switch",
                  "Append a new switch in the process",
                  GlyphFilled.SwitchAccessShortcut)
        {
            
            this.AddParentType<BpmsSwimLane>();
            this.WithModel<BpmsSwitch>();
            //this.SetTypeUI<SwimLaneComponent>();
            //this.IsControlled(true);
            //this.IsLocked(false);
        }

        // public override string GetDefaultName() => $"Switch";

        public static Guid Key = new Guid("1AE9D5F5-7502-4C89-9EA6-926E9F7C3D9E");


    }

}
