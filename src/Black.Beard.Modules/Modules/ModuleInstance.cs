using Bb.Modules.Storage;
using Bb.Storage;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Bb.Modules
{


    public class ModuleInstance : ModelBase<Guid>
    {

        public ModuleInstance()
        {

        }


        [StoreDescriptor(externalize: true, order: 4)]
        public string Label { get; set; }

        public string Description { get; set; }

        public Guid Specification { get; set; }

        [JsonIgnore]
        public ModuleSpecification ModuleSpecification { get; set; }
        
        [JsonIgnore]
        public FeatureInstances FeatureInstances { get; internal set; }

        internal ObservableCollection<FeatureInstance> GetFeatures()
        {
            var result = new ObservableCollection<FeatureInstance>(FeatureInstances.GetFeatures().Where(c => c.ModuleUuid == this.Uuid));
            return result;
        }
    }


}
