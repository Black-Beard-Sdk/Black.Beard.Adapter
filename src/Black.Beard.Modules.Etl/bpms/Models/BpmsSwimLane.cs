using Bb.Diagrams;
using Bb.TypeDescriptors;

namespace Bb.Modules.Bpms.Models
{


    public class BpmsSwimLane : UIGroupModel
    {

        static BpmsSwimLane()
        {

            DynamicTypeDescriptionProvider.Configure<BpmsSwimLane>(c =>
            {

                c.RemoveProperties
                (
                    nameof(BpmsSwimLane.ControlledSize),
                    nameof(BpmsSwimLane.Parent),
                    nameof(BpmsSwimLane.CanBeOrphaned),
                    nameof(BpmsSwimLane.Selected),
                    nameof(BpmsSwimLane.Id),
                    nameof(BpmsSwimLane.Locked),
                    nameof(BpmsSwimLane.Visible),
                    nameof(BpmsSwimLane.Title)

                );
            });

        }

        public BpmsSwimLane(SerializableDiagramGroupNode source)
            : base(source)
        {
            
        }

    }

}
