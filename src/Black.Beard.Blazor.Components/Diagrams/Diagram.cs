using Bb.CustomComponents;
using Blazor.Diagrams.Core.Models;
using System.Text.Json.Serialization;
using static MudBlazor.CategoryTypes;

namespace Bb.Diagrams
{

    public partial class Diagram : IValidationService
    {

        public Diagram()
        {
            Specifications = new List<DiagramSpecificationBase>();
            Models = new List<DiagramNode>();
            Relationships = new List<DiagramRelationship>();
            _dicModels = new Dictionary<Guid, DiagramSpecificationNodeBase>();
            _dicLinks = new Dictionary<Guid, DiagramSpecificationRelationshipBase>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<DiagramNode> Models { get; set; }

        public List<DiagramRelationship> Relationships { get; set; }

        public DiagramNode AddModel(Guid specification, double x, double y, string? name = null, Guid? uuid = null)
        {
            var spec = Specifications.OfType<DiagramSpecificationNodeBase>().Where(c => c.Uuid == specification).FirstOrDefault();
            return AddModel(spec, x, y, name, uuid);
        }

        public DiagramNode AddModel(DiagramSpecificationNodeBase specification, double x, double y, string? name = null, Guid? uuid = null)
        {

            if (string.IsNullOrEmpty(name))
            {
                var currentNames = new HashSet<string>(Models.Select(c => c.Name));
                int count = 1;
                while (currentNames.Contains(name = $"{specification.GetDefaultName()} {count}"))
                    count++;
            }

            var result = AddModel(specification.CreateModel(x, y, name, uuid));

            if (_diagram != null)
                ApplyToUI(specification, result);
        
            return result;
        }

        public DiagramNode AddModel(DiagramNode customizedNodeModel)
        {
            this.Models.Add(customizedNodeModel);
            this.Models.Sort((x, y) => x.Uuid.CompareTo(y.Uuid));
            return customizedNodeModel;
        }

        public DiagramNode GetModel(Guid id)
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

        public DiagramNode? GetModelByPort(Guid id)
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
                if (item is DiagramSpecificationNodeBase model)
                    _dicModels.Add(model.Uuid, model);
                else if (item is DiagramSpecificationRelationshipBase link)
                    _dicLinks.Add(link.Uuid, link);
            }
        }


        public virtual void Validate(Diagnostics Diagnostics)
        {

            // Diagnostics.EvaluateModel(this);

            //foreach (var item in this.Models)
            //    Diagnostics.EvaluateModel(item);

            //foreach (var item in this.Relationships)
            //    Diagnostics.EvaluateModel(item);

        }


        [JsonIgnore]
        public IEnumerable<DiagramSpecificationBase> Specifications { get; private set; }

        [JsonIgnore]
        public Action<Diagram> Save { get; private set; }

        [JsonIgnore]
        public Diagnostics LastDiagnostics { get; internal set; }

        public void SetSave(Action<Diagram> save)
        {

            Action<Diagram> _save = model =>
            {

                save(model);

                if (model.OnModelSaved != null)
                    model.OnModelSaved.Invoke(model, model);

            };

            Save = _save;


        }

        public event EventHandler<Diagram>? OnModelSaved;

        private readonly Dictionary<Guid, DiagramSpecificationNodeBase> _dicModels;
        private readonly Dictionary<Guid, DiagramSpecificationRelationshipBase> _dicLinks;

    }


}
