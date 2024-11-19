using Bb.ComponentModel.Attributes;
using Bb.Diagrams;
using Bb.Modules.Sgbd.DiagramTools;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Bb.Modules.Sgbd.Models
{

    public class SgbdDiagram : FeatureDiagram
    {

        public static Guid Key = new Guid("7BDE57DD-14BE-4E19-9896-54E4B2F35050");

        public SgbdDiagram()
            : base(Key, false)
        {

        }

        [Description("Target technology")]
        [ListProvider(typeof(ListProviderTechnologies))]
        [Required]
        public string Technology { get; set; }

        [JsonIgnore]
        [EvaluateValidation(false)]
        public SgbdTechnologies SgbdTechnologies { get; internal set; }
    
        public SgbdTechnology GetTechnology()
        {
            return SgbdTechnologies.GetTechnology(Technology);
        }

        public override void InitializeToolbox(DiagramToolbox toolbox)
        {
            toolbox
                .Add(new TableTool())
                .Add(new ConstraintRelationship())
                ;
        }

    }

}
