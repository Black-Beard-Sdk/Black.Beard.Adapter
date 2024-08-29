
using Bb.Storage;

namespace Bb.Modules
{
    public class ModelBase<TKey>
        where TKey : struct
    {


        public ModelBase()
        {

        }

        /// <summary>
        /// Unique key of the document
        /// </summary>
        [StoreDescriptor(isPrimary: true, order: 0)]
        public TKey Uuid { get; set; }


        /// <summary>
        /// Name of the module
        /// </summary>
        [StoreDescriptor(externalize: true, order: 4)]
        public string Label { get; set; }

        /// <summary>
        /// Description of the module
        /// </summary>
        public string Description { get; set; }


        [StoreDescriptor(checkIntegrity: true, order: 1)]
        public int Version { get; set; }

        [StoreDescriptor(updateHisto: true, order: 2)]
        public DateTimeOffset? LastUpdate { get; set; }

        [StoreDescriptor(insertHisto: true, order: 3)]
        public DateTimeOffset? Inserted { get; set; }


    }



}
