namespace Bb.Modules.Storage
{

    public class StoreItem<T>
    {

        public StoreItem(T item, Guid version)
        {
            this.Item = item;
        }

        public T Item { get; }

    }


}
