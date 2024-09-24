using Bb.ComponentModel.Attributes;
using System.ComponentModel;
namespace Bb.Diagrams
{
    public class DiagramGroupNode : SerializableDiagramNode
    {

        public DiagramGroupNode() : base()
        {

        }

        //[EvaluateValidation(false)]
        //[Browsable(false)]
        //public bool AutoSize { get; set; } = true;

        [EvaluateValidation(false)]
        [Browsable(false)]
        public byte Padding { get; set; } = 30;

    }

    //public class ExternalDiagramReference
    //{
        
    //    public string Document { get; set; }    

    //    public Guid Uuid { get; set; }

    //}

}
