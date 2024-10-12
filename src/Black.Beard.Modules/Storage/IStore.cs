
namespace Bb.Storage
{

    public interface IStore<TKey, TValue>
    {
        
        /// <summary>
        /// Index name
        /// </summary>
        string IndexName { get; }

        /// <summary>
        /// Initialize the store
        /// </summary>
        void Initialize();

        /// <summary>
        /// Return true if the key <see cref="TKey"/> exists in the store.
        /// </summary>
        /// <param name="key">the key <see cref="TKey"/> to evaluate</param>
        /// <returns>return true if the key exists in the store</returns>
        bool Exists(TKey key);

        /// <summary>
        /// Load the model <see cref="TValue"/> from the store by the key <see cref="TKey"/>.
        /// </summary>
        /// <param name="key">the key <see cref="TKey"/> to Load</param>
        TValue Load(TKey key);

        /// <summary>
        /// Save the model
        /// </summary>
        /// <param name="value">The value to store</param>
        void Save(TValue value);

        /// <summary>
        /// Remove the model <see cref="TValue"/> from the store by the key <see cref="TKey"/>.
        /// </summary>
        /// <param name="key">the key <see cref="TKey"/> to remove</param>
        bool RemoveKey(TKey key);

        /// <summary>
        /// Return the list of all existing values <see cref="TValue"/> in the store.
        /// </summary>
        /// <returns></returns>
        IEnumerable<TValue> Index();

        /// <summary>
        /// Return the list of all existing keys <see cref="TKey"/> in the store.
        /// </summary>
        /// <returns></returns>
        IEnumerable<TKey> Keys();

    }

}