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
                    "ControlledSize",
                    "Parent",
                    "CanBeOrphaned",
                    "Selected",
                    "Uuid",
                    "Id",
                    "Locked",
                    "Visible",
                    "DynamicToolbox"

                );
            });

        }

        public BpmsSwimLane(SerializableDiagramGroupNode source)
            : base(source)
        {

        }

    }

}
