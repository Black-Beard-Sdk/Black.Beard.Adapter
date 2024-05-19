using Bb.ComponentModel.Attributes;
using Bb.Diagrams;
using Bb.TypeDescriptors;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Bb.Modules.Sgbd.Models
{
    public class SgbdDiagram : Diagram, IDynamicDescriptorInstance
    {

        public SgbdDiagram()
        {
            this._container = new DynamicDescriptorInstanceContainer(this);
        }

        [Description("Target technology")]
        [ListProvider(typeof(ListProviderTechnologies))]
        public string Technology { get; set; }

        [JsonIgnore]
        public SgbdTechnologies SgbdTechnologies { get; internal set; }
    
        public SgbdTechnology GetTechnology()
        {
            return SgbdTechnologies.GetTechnology(Technology);
        }

        public object GetProperty(string name)
        {
            return this._container.GetProperty(name);
        }


        public void SetProperty(string name, object value)
        {
            this._container.SetProperty(name, value);
        }

        private readonly DynamicDescriptorInstanceContainer _container;


    }

}
