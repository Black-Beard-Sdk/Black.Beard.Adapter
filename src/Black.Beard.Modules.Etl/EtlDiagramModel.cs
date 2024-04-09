namespace Bb.Modules.Etl
{
    public class EtlDiagramModel
    {

        public EtlDiagramModel()
        {
            Items = new List<EtlEntity>();
            Models = new List<EtlEntityKind>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<EtlEntityKind> Models { get; set; }

        public List<EtlEntity> Items { get; set; }


    }

}
