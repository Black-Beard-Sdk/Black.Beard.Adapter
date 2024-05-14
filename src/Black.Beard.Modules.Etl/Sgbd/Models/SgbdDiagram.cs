﻿using Bb.ComponentModel.Attributes;
using Bb.Diagrams;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Bb.Modules.Sgbd.Models
{
    public class SgbdDiagram : Diagram
    {

        [Description("Target technology")]
        [ListProvider(typeof(ListProviderTechnologies))]
        public string Technology { get; set; }

        [JsonIgnore]
        public SgbdTechnologies SgbdTechnologies { get; internal set; }
    
        public SgbdTechnology GetTechnology()
        {
            return SgbdTechnologies.GetTechnology(Technology);
        }

    }

}
