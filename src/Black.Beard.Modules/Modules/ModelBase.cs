
using Bb.Storage;

namespace Bb.Modules
{
    public class ModelBase<TKey>
        where TKey : struct
    {


        public ModelBase()
        {

        }

        [StoreDescriptor(isPrimary: true, order: 0)]
        public TKey Uuid { get; set; }

        [StoreDescriptor(checkIntegrity: true, order: 1)]
        public int Version { get; set; }

        [StoreDescriptor(updateHisto: true, order: 2)]
        public DateTimeOffset? LastUpdate { get; set; }

        [StoreDescriptor(insertHisto: true, order: 3)]
        public DateTimeOffset? Inserted { get; set; }


    }



}
