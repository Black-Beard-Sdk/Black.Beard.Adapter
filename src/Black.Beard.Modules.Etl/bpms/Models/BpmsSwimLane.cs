﻿using Bb.Diagrams;
using Bb.TypeDescriptors;

namespace Bb.Modules.Bpms.Models
{


    public class BpmsSwimLane : UIModel
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

        public BpmsSwimLane(SerializableDiagramNode source)
            : base(source)
        {

        }

    }

}
