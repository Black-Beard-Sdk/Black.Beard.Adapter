using Bb.ComponentModel.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bb.Modules
{

    public class NewModuleDescription
    {

        [Description("Name of the module")]
        [Required]
        public string Name { get; set; }

        [Description("Description of the finality of the new module")]
        [Required]
        public string? Description { get; set; }

        [Description("Type based module")]
        [ListProvider(typeof(ListProviderModule))]
        [Required]
        public Guid? Type { get; set; }

    }

    public enum Enum1
    {
        Value1,
        Value2,
        Value3,
    }

}
