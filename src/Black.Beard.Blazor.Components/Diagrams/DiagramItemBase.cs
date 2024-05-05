using Bb.ComponentModel.Translations;
using System.Text.Json.Serialization;

namespace Bb.Diagrams
{

    public class DiagramItemBase
    {

        public Guid Uuid { get; set; }

        public TranslatedKeyLabel Name { get; set; }

        public TranslatedKeyLabel Description { get; set; }

        public Guid Type { get; set; }

        public DiagramItemSpecificationBase Specification { get; set; }

    }

}
