﻿using Bb.Diagrams;
using Bb.UIComponents.Glyphs;

namespace Bb.Modules.Sgbd.DiagramTools
{

    public class ConstraintRelationship : DiagramToolRelationshipBase
    {

        public ConstraintRelationship()
            : base(Key,
                   Bb.ComponentConstants.Relationship,
                   "Constraint",
                   "Create a constraint",
                   GlyphFilled.ArrowUpward)
        {

            IsDefaultLink = true;

        }

        public static Guid Key = new Guid("0A385005-4391-42A9-B538-2C33E1266801");

    }

}
