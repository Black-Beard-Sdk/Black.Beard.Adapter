

namespace Bb.Modules.Storage
{
    public interface IStore<TKey, TValue>
    {
        
        /// <summary>
        /// Initialize the store
        /// </summary>
        void Initialize();

        /// <summary>
        /// Return true if the key exist in the referential
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        bool Exists(TKey uuid);


        /// <summary>
        /// Load the model
        /// </summary>
        /// <param name="value"></param>
        TValue Load(TKey key);

        /// <summary>
        /// Save the model
        /// </summary>
        /// <param name="value"></param>
        void Save(TValue value);

        /// <summary>
        /// Remove the model
        /// </summary>
        /// <param name="value"></param>
        bool Remove(TKey key);


        int Remove((string, object) parameter);


        /// <summary>
        /// Return the list of values
        /// </summary>
        /// <returns></returns>
        IEnumerable<TValue> Values();

    }

}