using Bb.Diagrams;
using Bb.TypeDescriptors;

namespace Bb.Modules.Bpms.Models
{
    public class BpmsAction : UIModel
    {

        static BpmsAction()
        {

            DynamicTypeDescriptionProvider.Configure<BpmsAction>(c =>
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

        public BpmsAction(SerializableDiagramNode source)
            : base(source)
        {

        }

    }

}
