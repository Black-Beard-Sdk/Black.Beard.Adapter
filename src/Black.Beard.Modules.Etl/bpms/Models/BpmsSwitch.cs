using Bb.Diagrams;
using Bb.TypeDescriptors;

namespace Bb.Modules.Bpms.Models
{

    public class BpmsSwitch : UIModel
    {

        static BpmsSwitch()
        {

            DynamicTypeDescriptionProvider.Configure<BpmsSwitch>(c =>
            {

                c.RemoveProperties
                (
                    nameof(BpmsSwitch.ControlledSize),
                    nameof(BpmsSwitch.Parent),
                    nameof(BpmsSwitch.CanBeOrphaned),
                    nameof(BpmsSwitch.Selected),
                    nameof(BpmsSwitch.Id),
                    nameof(BpmsSwitch.Locked),
                    nameof(BpmsSwitch.Visible),
                    nameof(BpmsSwitch.Title)
                );
            });

        }

        public BpmsSwitch(SerializableDiagramNode source)
            : base(source)
        {

        }

    }

}
