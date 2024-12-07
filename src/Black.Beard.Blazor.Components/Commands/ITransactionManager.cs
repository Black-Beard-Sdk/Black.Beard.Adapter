
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
        /// <param name="description">label description. Can only be null if the behavior ask to remove last action. the description will take last transaction description.</param>
        /// <param name="behavior">ask a specific behavior</param>
        /// <returns></returns>
        Transaction? BeginTransaction(Mode mode, string description, Behavior behavior = Behavior.None);

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

    [Flags]
    public enum Behavior
    {
        None = 0,
        AutoCommit = 1,
        RemoveLastTransaction = 2,
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
