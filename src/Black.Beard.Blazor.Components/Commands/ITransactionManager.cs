
namespace Bb.Commands
{


    public interface ITransactionManager : IInternalTransaction
    {

        /// <summary>
        /// Save the initial state of the model
        /// </summary>
        public void Initialize();

        /// <summary>
        /// Create a new transaction
        /// </summary>
        /// <param name="description">label description</param>
        /// <param name="autocommit">dispose with auto commit</param>
        /// <returns></returns>
        Transaction BeginTransaction(Mode mode, string description, bool autocommit = false);

        /// <summary>
        /// Restore in specific transaction state and forget all the next transactions
        /// </summary>
        /// <param name="cmd">index to restore</param>
        void Undo(int last);

        /// <summary>
        /// Restore in specific transaction state and forget all the next transactions
        /// </summary>
        /// <param name="cmd">backup to restore</param>
        void Undo(Transaction last);

        /// <summary>
        /// Restore in specific transaction state and forget all the previous transaction.
        /// </summary>
        /// <param name="cmd">index to restore</param>
        void Redo(int last);

        /// <summary>
        /// Restore in specific transaction state and forget all the previous transaction.
        /// </summary>
        /// <param name="cmd">backup to restore</param>
        void Redo(Transaction last);



        /// <summary>
        /// Remove all changes
        /// </summary>
        void Reset();


        /// <summary>
        /// Return count of stored transaction
        /// </summary>
        int UndoCount { get; }

        /// <summary>
        /// Return the list of stored transaction for undo lasts update
        /// </summary>
        TransactionViewList UndoList { get; }

        /// <summary>
        /// Return the list of stored transaction for redo lasts removed update
        /// </summary>
        TransactionViewList RedoList { get; }

        /// <summary>
        /// Return current status of the manager
        /// </summary>
        StatusTransaction Status { get; }

        /// <summary>
        /// return the count of transaction
        /// </summary>
        public int Count { get; }

    }

    public enum StatusTransaction
    {
        Waiting,
        Recording,
        Restoring,
        InPause,
    }

    public enum Mode
    {
        Recording,
        Restoring,
        Paused,
    }

}
