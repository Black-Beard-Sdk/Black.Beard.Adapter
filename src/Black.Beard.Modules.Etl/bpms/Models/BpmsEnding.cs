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
                    nameof(BpmsEnding.ControlledSize),
                    nameof(BpmsEnding.Parent),
                    nameof(BpmsEnding.CanBeOrphaned),
                    nameof(BpmsEnding.Selected),
                    nameof(BpmsEnding.Id),
                    nameof(BpmsEnding.Locked),
                    nameof(BpmsEnding.Visible),
                    nameof(BpmsEnding.Title)

                );
            });

        }

        public BpmsEnding(SerializableDiagramNode source)
            : base(source)
        {

        }

    }

}
