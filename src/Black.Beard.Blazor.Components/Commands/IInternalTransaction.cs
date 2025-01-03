﻿
namespace Bb.Commands
{
    public interface IInternalTransaction
    {

        /// <summary>
        /// Store the current transaction
        /// </summary>
        void Commit();

        /// <summary>
        /// Aboard the current transaction
        /// </summary>
        void Rollback();

        /// <summary>
        /// Return a stream to store the transaction
        /// </summary>
        /// <param name="commandTransaction"></param>
        /// <returns></returns>
        Stream GetStreamForWriting(Transaction commandTransaction);

        /// <summary>
        /// Return a stream to read the transaction
        /// </summary>
        /// <param name="commandTransaction"></param>
        /// <returns></returns>
        Stream GetStreamForReading(Transaction commandTransaction);

        /// <summary>
        /// Return the index of the last transaction
        /// </summary>
        /// <returns></returns>
        int GetIndex();



    }

}
