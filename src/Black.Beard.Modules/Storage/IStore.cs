
namespace Bb.Modules.Storage
{
    public interface IStore<TKey, TValue>
    {

        /// <summary>
        /// Return the list of keys
        /// </summary>
        /// <returns></returns>
        List<(TKey, string, ModuleSpecification)> Keys();

        /// <summary>
        /// Return the list of values
        /// </summary>
        /// <returns></returns>
        List<StoreItem<TValue>> Values();

        /// <summary>
        /// Return true if the key exist
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Exist(TKey key);

        /// <summary>
        /// Load the model
        /// </summary>
        /// <param name="value"></param>
        StoreItem<TValue> Load(TKey key);

        /// <summary>
        /// Save the model
        /// </summary>
        /// <param name="value"></param>
        void Save(StoreItem<TValue> key);

        /// <summary>
        /// Save the model
        /// </summary>
        /// <param name="value"></param>
        void Save(TValue key);

        /// <summary>
        /// Remove the model
        /// </summary>
        /// <param name="value"></param>
        void Remove(TKey key);

    }

}