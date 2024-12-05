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
                    nameof(BpmsStartingEvent.ControlledSize),
                    nameof(BpmsStartingEvent.Parent),
                    nameof(BpmsStartingEvent.CanBeOrphaned),
                    nameof(BpmsStartingEvent.Selected),
                    nameof(BpmsStartingEvent.Id),
                    nameof(BpmsStartingEvent.Locked),
                    nameof(BpmsStartingEvent.Visible),
                    nameof(BpmsStartingEvent.Title)
                );
            });

        }

        public BpmsStartingEvent(SerializableDiagramNode source)
            : base(source)
        {

        }

    }

}
