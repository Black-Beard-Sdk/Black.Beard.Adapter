using Bb.Modules.Storage;

namespace Bb.Modules
{


    public static class Mapper
    {




    }


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


    public class ModuleInstance : ModelBase<Guid>
    {

        public ModuleInstance()
        {

        }

        public ModuleInstance(Guid uuidSpecification, Guid key)
        {

            this.Uuid = key;
            this.Specification = uuidSpecification;
        }


        [StoreDescriptor(externalize: true, order: 4)]
        public string Label { get; set; }

        public string Description { get; set; }

        public Guid Specification { get; set; }

    }


    [AttributeUsage(AttributeTargets.Property)]
    public class StoreDescriptorAttribute : Attribute
    {

        public StoreDescriptorAttribute(bool externalize = false, bool isPrimary = false, bool insertHisto = false, bool updateHisto = false, bool checkIntegrity = false, int order = 10)
        {
            this.Order = order;
            this.IsPrimary = isPrimary;
            this.InsertHisto = insertHisto;
            this.UpdateHisto = updateHisto;
            this.CheckIntegrity = checkIntegrity;

            if (isPrimary | externalize | insertHisto | updateHisto | checkIntegrity)
                this.Externalize = true;

        }

        public bool Externalize { get; }
        public int Order { get; }
        public bool IsPrimary { get; }
        public bool InsertHisto { get; }
        public bool UpdateHisto { get; }
        public bool CheckIntegrity { get; }
    }


}
