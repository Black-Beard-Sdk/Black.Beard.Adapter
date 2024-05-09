using Bb.ComponentModel.Translations;
using Bb.UIComponents;
using Blazor.Diagrams.Core.Models;

namespace Bb.Diagrams
{

    public class DiagramSpecificationNodeBase : DiagramSpecificationBase
    {

        public DiagramSpecificationNodeBase(Guid uuid, TranslatedKeyLabel name, TranslatedKeyLabel description, string icon)
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

        public virtual CustomizedNodeModel CreateUI(double x, double y, string name)
        {
            var model = CreateModel(x, y, name);
            var node = CreateUI(model);
            return node;
        }

        public virtual CustomizedNodeModel CreateUI(DiagramNode model)
        {
            return (CustomizedNodeModel)Activator.CreateInstance(TypeModel, new object[] { model });
        }

        public virtual DiagramNode CreateModel(double x, double y, string name, Guid? uuid = null)
        {

            var model = Create();
            model.Position = new Position(x, y);
            model.Name = name;
            model.Uuid = uuid.HasValue ? uuid.Value : Guid.NewGuid();
            model.Type = Uuid;

            foreach (var port in Ports)
                model.AddPort(port, Guid.NewGuid());

            return model;

        }

        protected virtual DiagramNode Create()
        {
            return (DiagramNode)Activator.CreateInstance(typeof(DiagramNode), new object[] { });
        }

        public HashSet<PortAlignment> Ports { get; }

    }

}
