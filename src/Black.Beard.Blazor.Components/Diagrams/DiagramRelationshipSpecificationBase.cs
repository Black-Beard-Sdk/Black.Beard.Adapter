using Bb.ComponentModel.Translations;

namespace Bb.Diagrams
{
    public class DiagramRelationshipSpecificationBase : DiagramItemSpecificationBase
    {

        public DiagramRelationshipSpecificationBase(Guid uuid, TranslatedKeyLabel name, TranslatedKeyLabel description, string icon)
            : base(uuid, name, description, icon)
        {
        }


    }


}
