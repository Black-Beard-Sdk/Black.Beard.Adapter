namespace Bb.Modules.Etl
{
    public class EtlRelationship
    {

        public Guid Uuid { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Guid Type { get; set; }

        public Guid Source { get; set; }

        public Guid Target { get; set; }

    }

}
