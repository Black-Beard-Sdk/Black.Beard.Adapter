
namespace Bb.Modules.Storage
{
    public interface IStore<TKey, TValue>
    {

        void Initialize();
        /// <summary>
        /// Load the model
        /// </summary>
        /// <param name="value"></param>
        TValue Load(TKey key);

        /// <summary>
        /// Save the model
        /// </summary>
        /// <param name="value"></param>
        void Save(TValue key);

        ///// <summary>
        ///// Remove the model
        ///// </summary>
        ///// <param name="value"></param>
        //void Remove(TKey key);


        /// <summary>
        /// Return the list of values
        /// </summary>
        /// <returns></returns>
        List<TValue> Values();

        ///// <summary>
        ///// Return true if the key exist
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //bool Exist(TKey key);


    }

}