using Bb.ComponentModel.Translations;
using Bb.UIComponents;
using Blazor.Diagrams.Core.Models;

namespace Bb.Diagrams
{
    public class DiagramSpecificationModelBase : DiagramSpecificationBase
    {

        public DiagramSpecificationModelBase(Guid uuid, TranslatedKeyLabel name, TranslatedKeyLabel description, string icon)
            : base(uuid, name, description, icon)
        {
            this.Ports = new HashSet<PortAlignment>();
            Kind = ToolKind.Node;
        }

        public override string GetDefaultName()
        {
            return $"Node";
        }

        public DiagramSpecificationBase AddPort(params PortAlignment[] ports)
        {
            foreach (var port in ports)
                Ports.Add(port);
            return this;
        }

        public virtual CustomizedNodeModel CreateUI(DiagramItemBase model)
        {
            var node = new CustomizedNodeModel(model);
            return node;
        }

        public virtual CustomizedNodeModel CreateUI(double x, double y, string name, string? description = null)
        {
            var node = new CustomizedNodeModel(CreateModel(x, y, name, description ?? name));
            return node;
        }

        public virtual DiagramItemBase CreateModel(double x, double y, string name, string? description = null, Guid? uuid = null)
        {

            var model = Create();
            model.Position = new Position(x, y);
            model.Name = name;
            model.Description = description ?? name;
            model.Uuid = uuid.HasValue ? uuid.Value : Guid.NewGuid();
            model.Type = Uuid;

            foreach (var port in Ports)
                model.AddPort(port, Guid.NewGuid());

            return model;

        }

        protected virtual DiagramItemBase Create()
        {

            return (DiagramItemBase)Activator.CreateInstance(typeof(DiagramItemBase), new object[] { });

        }

        public HashSet<PortAlignment> Ports { get; }

    }

}
