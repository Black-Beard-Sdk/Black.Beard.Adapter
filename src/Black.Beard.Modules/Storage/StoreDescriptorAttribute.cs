
namespace Bb.Storage
{


    [AttributeUsage(AttributeTargets.Property)]
    public class StoreDescriptorAttribute : Attribute
    {

        public StoreDescriptorAttribute(bool externalize = false, bool isPrimary = false, bool insertHisto = false, bool updateHisto = false, bool checkIntegrity = false, int order = 10)
        {
            Order = order;
            IsPrimary = isPrimary;
            InsertHisto = insertHisto;
            UpdateHisto = updateHisto;
            CheckIntegrity = checkIntegrity;

            if (isPrimary | externalize | insertHisto | updateHisto | checkIntegrity)
                Externalize = true;

        }

        public bool Externalize { get; }

        public int Order { get; }

        public bool IsPrimary { get; }

        public bool InsertHisto { get; }

        public bool UpdateHisto { get; }

        public bool CheckIntegrity { get; }

        public Type TypeInitializeColumn { get; set; }
    }


    public interface IInitializeColumn<TKey, TValue>
    {
        bool InitializeColumns(TValue instance);
    }

}
