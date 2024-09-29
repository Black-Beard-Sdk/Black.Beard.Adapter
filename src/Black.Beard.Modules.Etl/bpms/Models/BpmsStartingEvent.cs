using Bb.Diagrams;
using Bb.TypeDescriptors;

namespace Bb.Modules.Bpms.Models
{
    public class BpmsStartingEvent : UIModel
    {

        static BpmsStartingEvent()
        {

            DynamicTypeDescriptionProvider.Configure<BpmsStartingEvent>(c =>
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

        public BpmsStartingEvent(SerializableDiagramNode source)
            : base(source)
        {

        }

    }

}
