using Bb.Addons;
using Bb.ComponentModel.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bb.Modules
{


    public class NewDocumentDescription : ISolutionInstanceHost
    {



        public Solution Solution { get; set; }


        [Description("Name of the feature")]
        [Required]
        public string Name { get; set; }

        [Description("Description of the finality of the feature")]
        [Required]
        public string? Description { get; set; }

        [Description("Type based Feature")]
        [ListProvider(typeof(ListProviderFeatures))]
        [Required]
        public Guid? Type { get; set; }

    }

}
