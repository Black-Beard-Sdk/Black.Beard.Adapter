using Bb.ComponentModel.Attributes;
using System.ComponentModel;

namespace Bb.Diagrams
{

    public class SerializableDiagramGroupNode : SerializableDiagramNode
    {

        public SerializableDiagramGroupNode(Guid Type) 
            : base(Type)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDiagramGroupNode"/> class.
        /// For de-serialization
        /// </summary>
        public SerializableDiagramGroupNode() 
            : base()
        {

        }

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
