using System.Collections;

namespace Bb
{

    public interface ICommandTransactionManager : IEnumerable<CommandTransaction>
    {

        /// <summary>
        /// Create a new transaction
        /// </summary>
        /// <param name="name"></param>
        /// <param name="command"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        CommandTransaction BeginTransaction(string name);

        CommandTransaction Snapshot(string name);


        /// <summary>
        /// Access to command store by specified index
        /// </summary>
        /// <param name="index">index of the transaction</param>
        /// <returns></returns>
        public CommandTransaction this[int index] { get; }

        /// <summary>
        /// Store the current transaction
        /// </summary>
        void Commit();

        /// <summary>
        /// Aboard the current transaction
        /// </summary>
        void Rollback();

        /// <summary>
        /// Restore the last transaction state and forget the transactions
        /// </summary>
        public void RestoreLast();

        /// <summary>
        /// Restore in specific transaction state and forget all the next transactions
        /// </summary>
        /// <param name="last"></param>
        public void Restore(CommandTransaction last);
        
        /// <summary>
        /// Remove all changes
        /// </summary>
        void Reset();

        /// <summary>
        /// Return count of stored transaction
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// return true if transaction is initialized
        /// </summary>
        public bool TransactionInitialized { get; }

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

    public class CommandTransactionManager : ICommandTransactionManager
    {

        /// <summary>
        /// Initialize a new transaction manager
        /// </summary>
        public CommandTransactionManager(ICommandMemorizer instance)
        {
            this._instance = instance;
            commands = new List<CommandTransaction>();
        }


        public bool Scope(string label, out CommandTransaction command)
        {
            bool resultBool = false;
            command = null;

            if (_currentTransaction != null)
                command = _currentTransaction;
            
            else
            {
                command = BeginTransaction(label);
                resultBool = true;
            }

            return resultBool;

        }


        public void Pause()
        {

            if (_currentTransaction != null)
                throw new InvalidOperationException("transaction already began. Please wait the transaction is closed.");

            this._InPaused = true;

        }

        public void Resume()
        {
            this._InPaused = true;
        }

        /// <summary>
        /// return true if transaction is initialized
        /// </summary>
        public bool TransactionInitialized => _currentTransaction != null;

        /// <summary>
        /// Begin a new transaction. If the result is null, the transaction manager is paused.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="command"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public CommandTransaction? BeginTransaction(string name = "update model")
        {

            if (this._InPaused)
                return null;

            if (_currentTransaction != null)
                throw new InvalidOperationException("transaction already began");

            return _currentTransaction = new CommandTransaction(this, _instance, name);

        }

        public CommandTransaction Snapshot(string name)
        {
            return _currentTransaction = new CommandTransaction(this, _instance, name);
        }

        /// <summary>
        /// Access to command store by specified index
        /// </summary>
        /// <param name="index">index of the transaction</param>
        /// <returns></returns>
        public CommandTransaction this[int index] => commands[index];

        /// <summary>
        /// Commit current transaction
        /// </summary>
        public void Commit()
        {
            _currentTransaction?.Commit();
        }

        /// <summary>
        /// Add a new transaction
        /// </summary>
        /// <param name="command"></param>
        /// <param name="model"></param>
        internal void Add(ICommandMemorizer command)
        {
            commands.Add(_currentTransaction);
            _currentTransaction = null;
        }

        /// <summary>
        /// Reset the transaction manager
        /// </summary>
        internal void Reset()
        {
            if (commands.Count > 0)
                commands.Clear();
        }

        /// <summary>
        /// Aboard the current transaction
        /// </summary>
        public void Rollback()
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Restore();
                _currentTransaction = null;
            }
        }

        /// <summary>
        /// Restore the last transaction state and forget the transactions
        /// </summary>
        public void RestoreLast()
        {
            if (commands.Count > 1)
                Restore(commands[commands.Count - 2]);
        }

        /// <summary>
        /// Restore in specific transaction state and forget all the next transactions
        /// </summary>
        /// <param name="last"></param>
        public void Restore(CommandTransaction last)
        {

            var index = commands.IndexOf(last);
            last.Restore();

            while (index > commands.Count - 1)
                commands.RemoveAt(index);

        }

        /// <summary>
        /// Return number of transaction stored in the manager.
        /// </summary>
        public int Count => commands.Count;

        public IEnumerator<CommandTransaction> GetEnumerator()
        {
            return commands.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return commands.GetEnumerator();
        }

        void ICommandTransactionManager.Reset()
        {
            commands.Clear();
        }

        private CommandTransaction _currentTransaction;
        private readonly ICommandMemorizer _instance;
        private List<CommandTransaction> commands;
        private bool _InPaused;

    }

    public class CommandTransaction : IDisposable
    {

        /// <summary>
        /// initialize a new transaction
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="command"></param>
        /// <param name="model"></param>
        internal CommandTransaction(CommandTransactionManager manager, ICommandMemorizer command, string name)
        {
            _manager = manager;
            _command = command;
            Label = name;
            Value = null;
            var _modelBefore = new MemoryStream();
            command.Memorize(_modelBefore);
            _crc = _modelBefore.GetHashCode();
        }

        public string Label { get; set; }

        public MemoryStream Value { get; set; }

        public void Commit()
        {
            _modelBefore = null;
            Value = _modelBefore;
            _manager.Add(_command);
        }

        /// <summary>
        /// Dispose the transaction
        /// </summary>
        public void Dispose()
        {
            _manager.Rollback();
        }

        internal void Restore()
        {
            _command.Restore(this);
            _modelBefore = null;
        }

        private readonly CommandTransactionManager _manager;
        private readonly ICommandMemorizer _command;
        private MemoryStream _modelBefore;
        private readonly int _crc;
    }

}
