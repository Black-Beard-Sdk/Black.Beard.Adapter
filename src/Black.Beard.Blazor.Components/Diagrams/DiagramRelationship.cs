using Bb.ComponentModel.Translations;

namespace Bb.Diagrams
{

    public class DiagramRelationship
    {


        public Guid Uuid { get; set; }

        public TranslatedKeyLabel Name { get; set; }

        // public TranslatedKeyLabel Description { get; set; }

        public Guid Type { get; set; }

        public Guid Source { get;  set; }

        public Guid Target { get;  set; }

    }

}
