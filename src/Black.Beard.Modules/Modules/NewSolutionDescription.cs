﻿using Bb.Addons;
using Bb.ComponentModel.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bb.Modules
{

    public class NewSolutionDescription
    {

        [Description("Name of the module")]
        [Required]
        public string Name { get; set; }

        [Description("Description of the finality of the new module")]
        [Required]
        public string? Description { get; set; }

        [Description("Type based module")]
        [ListProvider(typeof(ListProviderAddons))]
        [Required]
        public Guid? Type { get; set; }

    }

}
