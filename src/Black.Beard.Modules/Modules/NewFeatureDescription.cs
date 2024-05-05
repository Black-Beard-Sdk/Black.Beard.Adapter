using Bb.ComponentModel.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bb.Modules
{

    public interface IModuleInstanceHost
    {
        ModuleInstance Module { get; set; }
    }


    public class NewFeatureDescription : IModuleInstanceHost
    {



        public ModuleInstance Module { get; set; }


        [Description("Name of the feature")]
        [Required]
        public string Name { get; set; }

        [Description("Description of the finality of the feature")]
        [Required]
        public string? Description { get; set; }

        [Description("Type based Feature")]
        [ListProvider(typeof(ListProviderFeature))]
        [Required]
        public Guid? Type { get; set; }

    }

}
