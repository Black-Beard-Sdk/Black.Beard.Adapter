using Bb.Configuration.Git;
using Bb.Storage;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Bb.Modules
{


    /// <summary>
    /// ModuleInstance for manage module
    /// </summary>
    public class ModuleInstance : ModelBase<Guid>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleInstance"/> class.
        /// </summary>
        public ModuleInstance()
        {
            Sources = new GitConnection();
        }

        /// <summary>
        /// Name of the module
        /// </summary>
        [StoreDescriptor(externalize: true, order: 4)]
        public string Label { get; set; }

        /// <summary>
        /// Description of the module
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Type of the module
        /// </summary>
        public Guid Specification { get; set; }

        /// <summary>
        /// Instance of the module
        /// </summary>
        [JsonIgnore]
        public ModuleSpecification ModuleSpecification { get; set; }
        

        /// <summary>
        /// Location of the sources
        /// </summary>
        public GitConnection Sources { get; set; }

        /// <summary>
        /// List of features of the module
        /// </summary>
        [JsonIgnore]
        public FeatureInstances FeatureInstances { get; internal set; }

        internal ObservableCollection<FeatureInstance> GetFeatures()
        {
            var result = new ObservableCollection<FeatureInstance>(FeatureInstances.GetFeatures().Where(c => c.ModuleUuid == this.Uuid));
            return result;
        }
    }


}
