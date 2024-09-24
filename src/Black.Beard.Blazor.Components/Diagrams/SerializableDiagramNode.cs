using Bb.ComponentModel.Attributes;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using System.ComponentModel.DataAnnotations;
namespace Bb.Diagrams
{


    public class SerializableDiagramNode : IDiagramNode
    {

        public SerializableDiagramNode()
        {
            Position = new Position();
            Name = string.Empty;
            Ports = new List<Port>();
            Properties = new Properties();
        }

        protected internal virtual void InitializeFirst(DiagramToolNode source)
        {

            this.Locked = source.Locked;
            this.ControlledSize = source.ControlledSize;

            foreach (var port in source.Ports)
                AddPort(port, Guid.NewGuid());

        }

        [Required]
        public Guid Uuid { get; set; }

        public Guid? UuidParent { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public Guid Type { get; set; }

        [EvaluateValidation(false)]
        public Position Position { get; set; }

        public List<Port> Ports { get; set; }

        [EvaluateValidation(false)]
        public Properties Properties { get; set; }

        public bool Locked { get; internal set; }
        
        public bool ControlledSize { get; internal set; }

        public Size? Size { get ; set ; }

        public void SetProperty(string name, string value) => Properties.SetProperty(name, value);

        public string? GetProperty(string name) => Properties.GetProperty(name);

        public Port AddPort(PortAlignment alignment, Guid id)
        {

            var p = Ports.FirstOrDefault(c => c.Uuid == id);
            if (p == null)
            {
                p = new Port() { Uuid = id, Alignment = alignment };
                Ports.Add(p);
            }
            else
            {
                if (p.Alignment != alignment)
                {
                    p.Alignment = alignment;
                }

            }
            return p;
        }

        public Port GetPort(PortAlignment alignment)
        {
            return Ports.FirstOrDefault(c => c.Alignment == alignment);
        }

        public Port GetPort(Guid id)
        {
            return Ports.FirstOrDefault(c => c.Uuid == id);
        }

        #region diagram

        public T? GetDiagram<T>()
            where T : Diagram
        {
            return (T)_diagram;
        }

        public void SetDiagram<T>(T diagram)
            where T : Diagram
        {
            _diagram = diagram;
        }

        public bool DiagramIs<T>()
        {
            if (_diagram is T)
                return true;
            return false;
        }

        private Diagram? _diagram;

        #endregion diagram

        // public ExternalDiagramReference ExternalReference { get; set; }

    }

    //public class ExternalDiagramReference
    //{

    //    public string Document { get; set; }    

    //    public Guid Uuid { get; set; }

    //}

}
