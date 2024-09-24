using Bb.Diagrams;
using Blazor.Diagrams.Core;
using System.Text.Json.Serialization;

namespace Bb.Modules.Bpms.Models
{
    public class BpmsDiagram : Diagrams.Diagram
    {

        public BpmsDiagram()
        {
            this.TypeModelId = new Guid("0E61164D-92C8-4A3E-8BD8-68EF1EAAB2BA");

        }

        public override DiagramToolbox CreateTool()
        {
            var toolbox = base.CreateTool();

            toolbox
                  .Add(new SwimLaneTool())

                  ;

            return toolbox;


        }


    }



}
