using Bb.ComponentModel.Translations;

namespace Bb.Diagrams
{
    public class DiagramToolRelationshipBase : DiagramToolBase
    {

        public DiagramToolRelationshipBase(Guid uuid, TranslatedKeyLabel name, TranslatedKeyLabel description, string icon)
            : base(uuid, name, description, icon)
        {
            Category = Bb.ComponentConstants.Relationships;
        }


    }


}
