using Bb.Addons;
using Bb.Configuration.Git;
using Bb.Storage;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Bb.Modules
{


    /// <summary>
    /// ModuleInstance for manage module
    /// </summary>
    public class Solution : ModelBase<Guid>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Solution"/> class.
        /// </summary>
        public Solution()
        {
            Sources = new GitConnection();
        }

        

        /// <summary>
        /// Location of the sources
        /// </summary>
        public GitConnection Sources { get; set; }

        /// <summary>
        /// List of features of the module
        /// </summary>
        [JsonIgnore]
        public Documents Documents { get; internal set; }


        internal ObservableCollection<Document> GetFeatures()
        {
            var result = new ObservableCollection<Document>(Documents.GetDocuments().Where(c => c.ModuleUuid == this.Uuid));
            return result;
        }
    }


}
