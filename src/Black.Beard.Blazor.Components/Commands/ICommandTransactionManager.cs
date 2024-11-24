namespace Bb.Commands
{
    public interface ICommandTransactionManager
    {

        /// <summary>
        /// Create a new transaction
        /// </summary>
        /// <param name="name"></param>
        /// <param name="command"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        CommandTransaction BeginTransaction(string name);

        /// <summary>
        /// Current status of the manager
        /// </summary>
        StatusTransaction Status { get; }

        /// <summary>
        /// Store the current transaction
        /// </summary>
        void Commit();

        /// <summary>
        /// Aboard the current transaction
        /// </summary>
        void Rollback();

        /// <summary>
        /// Restore in specific transaction state and forget all the next transactions
        /// </summary>
        /// <param name="cmd">index to restore</param>
        void Undo(int last);

        /// <summary>
        /// Restore in specific transaction state and forget all the next transactions
        /// </summary>
        /// <param name="cmd">backup to restore</param>
        void Undo(CommandTransaction last);

        /// <summary>
        /// Restore in specific transaction state and forget all the previous transaction.
        /// </summary>
        /// <param name="cmd">index to restore</param>
        void Redo(int last);

        /// <summary>
        /// Restore in specific transaction state and forget all the previous transaction.
        /// </summary>
        /// <param name="cmd">backup to restore</param>
        void Redo(CommandTransaction last);

        /// <summary>
        /// Remove all changes
        /// </summary>
        void Reset();

        /// <summary>
        /// Return count of stored transaction
        /// </summary>
        int UndoCount { get; }

        CommandTransactionViewList UndoList { get; }

        CommandTransactionViewList RedoList { get; }

        /// <summary>
        /// Return a transaction.
        /// if the result is true, the transaction , is just created.
        /// </summary>
        /// <param name="label">labal of the creation</param>
        /// <param name="command">the transaction</param>
        /// <returns></returns>
        bool Scope(string label, out CommandTransaction command);

        /// <summary>
        /// If the manager is paused, the transaction is not created.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resume the manager close the pause.
        /// </summary>
        public void Resume();




    }

    public enum StatusTransaction
    {
        Waiting,
        Recoding,
        Restoring,
        InPause,
    }


}
