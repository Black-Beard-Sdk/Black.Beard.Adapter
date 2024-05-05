using Bb.ComponentModel.Translations;

namespace Bb.Diagrams
{
    public class DiagramModelSpecificationBase : DiagramItemSpecificationBase
    {

        public DiagramModelSpecificationBase(Guid uuid, TranslatedKeyLabel name, TranslatedKeyLabel description, string icon) 
            : base(uuid, name, description, icon)
        {
        }


    }


}
