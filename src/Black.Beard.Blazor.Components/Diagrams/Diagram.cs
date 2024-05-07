using System.Text.Json.Serialization;

namespace Bb.Diagrams
{

    public partial class Diagram
    {

        public Diagram()
        {
            Specifications = new List<DiagramSpecificationBase>();
            Models = new List<DiagramItemBase>();
            Relationships = new List<DiagramRelationship>();
            _dicModels = new Dictionary<Guid, DiagramSpecificationModelBase>();
            _dicLinks = new Dictionary<Guid, DiagramSpecificationRelationshipBase>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        [JsonIgnore]
        public IEnumerable<DiagramSpecificationBase> Specifications { get; private set; }

        public List<DiagramItemBase> Models { get; set; }

        public List<DiagramRelationship> Relationships { get; set; }

        public DiagramItemBase AddModel(Guid specification, double x, double y, string? name = null, string? description = null, Guid? uuid = null)
        {
            var spec = Specifications.OfType<DiagramSpecificationModelBase>().Where(c => c.Uuid == specification).FirstOrDefault();
            return AddModel(spec, x, y, name, description ?? name, uuid);
        }

        public DiagramItemBase AddModel(DiagramSpecificationModelBase specification, double x, double y, string? name = null, string? description = null, Guid? uuid = null)
        {

            if (string.IsNullOrEmpty(name))
            {
                var currentNames = new HashSet<string>(Models.Select(c => c.Name));
                int count = 1;
                while (currentNames.Contains(name = $"{specification.GetDefaultName()} {count}"))
                    count++;
            }

            var result = AddModel(specification.CreateModel(x, y, name, description ?? name, uuid));

            if (_diagram != null)
                ApplyToUI(specification, result);
        
            return result;
        }

        public DiagramItemBase AddModel(DiagramItemBase customizedNodeModel)
        {
            this.Models.Add(customizedNodeModel);
            this.Models.Sort((x, y) => x.Uuid.CompareTo(y.Uuid));
            return customizedNodeModel;
        }

        public DiagramItemBase GetModel(Guid id)
        {
            return this.Models.FirstOrDefault(c => c.Uuid == id);
        }


        public DiagramRelationship AddLink(Guid specification, Port left, Port right, string name, Guid? uuid = null)
        {
            var spec = Specifications.OfType<DiagramSpecificationRelationshipBase>().Where(c => c.Uuid == specification).FirstOrDefault();
            return AddLink(spec, left, right, name, uuid);
        }

        public DiagramRelationship AddLink(DiagramSpecificationRelationshipBase specification, Port left, Port right, string name, Guid? uuid = null)
        {
            var link = new DiagramRelationship() 
            {
                Uuid = uuid.HasValue ? uuid.Value : Guid.NewGuid(),
                Name = name,
                Type = specification.Uuid, 
                Source = left.Uuid, 
                Target = right.Uuid 
            };
            this.Relationships.Add(link);
            this.Models.Sort((x, y) => x.Uuid.CompareTo(y.Uuid));
            return link;
        }


        public DiagramItemBase? GetModelByPort(Guid id)
        {

            foreach (var item in this.Models)
                foreach (var port in item.Ports)
                    if (port.Uuid == id)
                        return item;

            return null;

        }

        public void SetSpecifications(IEnumerable<DiagramSpecificationBase> specifications)
        {
            Specifications = specifications.ToList();
            foreach (var item in specifications)
            {
                if (item is DiagramSpecificationModelBase model)
                    _dicModels.Add(model.Uuid, model);
                else if (item is DiagramSpecificationRelationshipBase link)
                    _dicLinks.Add(link.Uuid, link);
            }
        }

        private readonly Dictionary<Guid, DiagramSpecificationModelBase> _dicModels;
        private readonly Dictionary<Guid, DiagramSpecificationRelationshipBase> _dicLinks;

    }


}
