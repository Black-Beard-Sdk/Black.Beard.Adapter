

using System.Collections.Specialized;
using System.ComponentModel;

namespace Bb.Commands
{


    /// <summary>
    /// Manages command transactions, allowing for commit and rollback operations.
    /// </summary>
    public class CommandTransactionManager : ICommandTransactionManager
    {


        static CommandTransactionManager()
        {
            _targetFolder = Path.GetTempPath();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandTransactionManager"/> class.
        /// </summary>
        /// <param name="folder"></param>
        public static void SetFolder(string folder)
        {
            _targetFolder = folder;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandTransactionManager"/> class.
        /// </summary>
        /// <param name="targetFolder">The target folder where transaction data will be stored.</param>
        public CommandTransactionManager(ICommandMemorizer target)
        {

            _target = target;
            _forUndo = new Stack<CommandTransaction>();
            _forUndoView = new CommandTransactionViewList();

            _forRedo = new Stack<CommandTransaction>();
            _forRedoView = new CommandTransactionViewList();

            Sessionid = "_cmd_" + Guid.NewGuid().ToString().Replace("-", "");
            var folder = _targetFolder.Combine(Sessionid).AsDirectory();
            TargetFolder = folder.FullName;

            if (target is INotifyCollectionChanged n)
                n.CollectionChanged += N_CollectionChanged;

            if (target is INotifyPropertyChanged p)
                p.PropertyChanged += P_PropertyChanged;
           
        }

        private void P_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (Status == StatusTransaction.Waiting)
                throw new InvalidOperationException("Transaction not initialized");
        }

        private void N_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (Status == StatusTransaction.Waiting)
                throw new InvalidOperationException("Transaction not initialized");
        }

        /// <summary>
        /// Begin a new transaction. If the result is null, the transaction manager is paused.
        /// </summary>
        /// <param name="label">Label to show for describes the updates</param>
        /// <param name="command"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Put the transaction manager on pause
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void Pause()
        {

            using (var l = _lock.LockForWrite())
            {
                Status = StatusTransaction.InPause;
            }

            if (_currentTransaction != null)
                throw new InvalidOperationException("transaction already began. Please wait the transaction is closed.");

            using (var l = _lock.LockForWrite())
            {
                Status = StatusTransaction.InPause;
            }

        }

        /// <summary>
        /// Re active the transaction manager
        /// </summary>
        public void Resume()
        {

            using (var l = _lock.LockForWrite())
            {
                Status = StatusTransaction.Waiting;
            }

        }

        /// <summary>
        /// Begin a new transaction. If the result is null, the transaction manager is paused.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="command"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public CommandTransaction? BeginTransaction(string name = "update model")
        {


            using (var l = _lock.LockForUpgradeableRead())
            {

                if (Status == StatusTransaction.InPause)
                    return null;

                if (_currentTransaction != null)
                    throw new InvalidOperationException("transaction already began");

                var trans = new CommandTransaction(this, name, _forUndo.Count + 1);
                trans.InitializeValue(_target);

                using (_lock.LockForWrite())
                {
                    Status = StatusTransaction.Recoding;
                    _currentTransaction = trans;
                }

            }

            return _currentTransaction;

        }

        /// <summary>
        /// Commit current transaction
        /// </summary>
        public void Commit()
        {

            using (var l = _lock.LockForWrite())
            {
                _currentTransaction?.Commit();
                _forUndo.Push(_currentTransaction);
                _forUndoView.Push(_currentTransaction.GetView());
                _currentTransaction = null;
                _forRedo.Clear();
                Status = StatusTransaction.Waiting;
            }

        }

        /// <summary>
        /// Reset the transaction manager
        /// </summary>
        internal void Reset()
        {

            if (_forUndo.Count > 0 || _forRedo.Count > 0)
                using (var l = _lock.LockForWrite())
                {

                    if (_forUndo.Count > 0)
                    {
                        _forUndo.Clear();
                        _forUndoView.Clear();
                    }

                    if (_forRedo.Count > 0)
                        _forRedo.Clear();

                }

        }

        /// <summary>
        /// Aboard the current transaction
        /// </summary>
        public void Rollback()
        {

            using (var l1 = _lock.LockForUpgradeableRead())
            {

                if (_currentTransaction != null)
                {

                    var act = () =>
                    {
                        Status = StatusTransaction.Waiting;
                        _currentTransaction = null;
                    };

                    using (var l2 = _lock.LockForWrite(act))
                    {
                        _target.Restore(_currentTransaction);
                    }

                }

            }

        }

        /// <summary>
        /// Restore in specific transaction state and forget all the next transactions
        /// </summary>
        /// <param name="cmd">index to restore</param>
        public void Undo(int index)
        {

            using (var l1 = _lock.LockForUpgradeableRead())
            {

                _currentTransaction = null;

                if (_forUndo.Count > 0)
                {

                    var act = () =>
                    {
                        Status = StatusTransaction.Waiting;
                    };

                    using (var l2 = _lock.LockForWrite(act))
                    {

                        Status = StatusTransaction.Restoring;

                        while (_forUndo.Count > 0 && _forUndo.Peek().Index >= index)
                        {
                            var c = _forUndo.Pop();
                            _forUndoView.Pop();
                            if (_target.Mode == MemorizerEnum.Snapshot || index == c.Index)
                                _target.Restore(c);
                            _forRedo.Push(c);
                            _forRedoView.Push(c.GetView());
                        }

                    }

                }

            }

        }

        /// <summary>
        /// Restore in specific transaction state and forget all the next transactions
        /// </summary>
        /// <param name="cmd">backup to restore</param>
        public void Undo(CommandTransaction cmd)
        {
            Undo(cmd.Index);
        }

        /// <summary>
        /// Restore in specific transaction state and forget all the previous transaction.
        /// </summary>
        /// <param name="index">index to restore</param>
        public void Redo(int index)
        {

            using (var l1 = _lock.LockForUpgradeableRead())
            {
                _currentTransaction = null;

                if (_forRedo.Count > 0)
                {

                    Status = StatusTransaction.Restoring;

                    try
                    {
                        while (_forRedo.Count > 0 && _forRedo.Peek().Index >= index)
                        {
                            var c = _forRedo.Pop();
                            _forRedoView.Pop();
                            if (_target.Mode == MemorizerEnum.Snapshot || index == c.Index)
                                _target.Restore(c);
                            _forUndo.Push(c);
                            _forUndoView.Push(c.GetView());
                        }
                    }
                    finally
                    {
                        Status = StatusTransaction.Waiting;
                    }
                }
            }

        }

        /// <summary>
        /// Restore in specific transaction state and forget all the previous transaction.
        /// </summary>
        /// <param name="cmd">backup to restore</param>
        public void Redo(CommandTransaction cmd)
        {
            Redo(cmd.Index);
        }


        void ICommandTransactionManager.Reset()
        {
            _currentTransaction = null;
            Status = StatusTransaction.Waiting;
            _forUndo.Clear();
            _forUndoView.Clear();
            _forRedo.Clear();
            _forRedoView.Clear();
        }


        public StatusTransaction Status { get; private set; }

        public CommandTransactionViewList UndoList => _forUndoView;

        public CommandTransactionViewList RedoList => _forRedoView;

        /// <summary>
        /// Return number of transaction stored and waiting for undo states.
        /// </summary>
        public int UndoCount => _forUndo.Count;

        /// <summary>
        /// Unique session id
        /// </summary>
        public string Sessionid { get; }

        /// <summary>
        /// Gets the target folder where transaction data is stored.
        /// </summary>
        public string TargetFolder { get; }

        private CommandTransaction _currentTransaction;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly ICommandMemorizer _target;
        private Stack<CommandTransaction> _forUndo;
        private readonly CommandTransactionViewList _forUndoView;
        private Stack<CommandTransaction> _forRedo;
        private readonly CommandTransactionViewList _forRedoView;
        private int _currentIndex = 0;
        private static string _targetFolder;

    }

}
