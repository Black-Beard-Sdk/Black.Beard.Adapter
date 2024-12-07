

namespace Bb.Commands
{

    /// <summary>
    /// Represents a stack of transactions that waiting until restore.
    /// </summary>
    public class TransactionStack
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionStack"/> class.
        /// </summary>
        public TransactionStack(CommandTransactionManager manager)
        {
            this._transactionManager = manager;
            this._stack = new Stack<Transaction>();
            View = new TransactionViewList();
        }

        /// <summary>
        /// return the last transaction without removing it
        /// </summary>
        /// <returns></returns>
        public Transaction? Peek => _stack.Count > 0 ? _stack.Peek() : default;


        /// <summary>
        /// Add a transaction to the stack.
        /// </summary>
        /// <param name="transaction"></param>
        public void Push(Transaction transaction)
        {
            _stack.Push(transaction);
            View.Push(transaction.GetView());
        }

        /// <summary>
        /// Remove and return the last transaction from the stack.
        /// </summary>
        /// <returns></returns>
        public Transaction Pop()
        {
            View.Pop();
            return _stack.Pop();
        }


        public void DeleteCurrent()
        {
            _transactionManager.Delete(Pop());
        }

        public void DeleteAll()
        {
            while (_stack.Count > 0)
                _transactionManager.Delete(Pop());
        }


        /// <summary>
        /// Returns a value indicating whether the stack is empty.
        /// </summary>
        public bool IsNotEmpty => _stack.Count > 0;

        /// <summary>
        /// Returns the number of transactions in the stack.
        /// </summary>
        public int Count => _stack.Count;

        private readonly CommandTransactionManager _transactionManager;
        private readonly Stack<Transaction> _stack;

        public TransactionViewList View { get; }

    }

}


