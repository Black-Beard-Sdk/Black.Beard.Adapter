using Bb.Diagrams;
using Bb.TypeDescriptors;

namespace Bb.Modules.Bpms.Models
{
    public class BpmsEnding : UIModel
    {

        static BpmsEnding()
        {

            DynamicTypeDescriptionProvider.Configure<BpmsEnding>(c =>
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

        public BpmsEnding(SerializableDiagramNode source)
            : base(source)
        {

        }

    }

}
