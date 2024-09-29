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

        public BpmsSwitch(SerializableDiagramNode source)
            : base(source)
        {

        }

    }

}
