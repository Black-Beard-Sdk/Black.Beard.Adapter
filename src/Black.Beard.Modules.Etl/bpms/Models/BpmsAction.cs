using Bb.Diagrams;
using Bb.TypeDescriptors;
using Blazor.Diagrams.Core.Models;

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
                    nameof(BpmsAction.ControlledSize),
                    nameof(BpmsAction.Parent),
                    nameof(BpmsAction.CanBeOrphaned),
                    nameof(BpmsAction.Selected),
                    nameof(BpmsAction.Id),
                    nameof(BpmsAction.Locked),
                    nameof(BpmsAction.Visible),
                    nameof(BpmsAction.Title)

                );
            });


        }

        public BpmsAction(SerializableDiagramNode source)
            : base(source)
        {

        }

    }

}
