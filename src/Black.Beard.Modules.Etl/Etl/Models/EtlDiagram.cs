using Bb.Diagrams;
using System.Text.Json.Serialization;

namespace Bb.Modules.Etl.Models
{

    public class EtlDiagram : Diagram
    {

        public static Guid Key = new Guid("3E4BF95A-58CE-4D05-959B-E3CEBFB82A5E");

        public EtlDiagram()
            : base(Key, false)
        {


        }

        public override void InitializeToolbox(DiagramToolbox toolbox)
        {

            toolbox
                .Add(new WebServiceTool())
                .Add(new WebServiceMethodTool())
                .Add(new TemplateJsltTool())
                .Add(new FolderTool())
                .Add(new ConstraintRelationship())

      ;

        }

    }

}
