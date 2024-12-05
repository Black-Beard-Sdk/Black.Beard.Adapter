

using System.Collections.Specialized;
using System.ComponentModel;
using System.Transactions;

namespace Bb.Commands
{


    /// <summary>
    /// Manages command transactions, allowing for commit and rollback operations.
    /// </summary>
    public class CommandTransactionManager : IInternalTransaction, ITransactionManager
    {

        static CommandTransactionManager()
        {
            _targetGlobalFolder = Path.GetTempPath();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandTransactionManager"/> class.
        /// </summary>
        /// <param name="targetFolder">The target folder where transaction data will be stored.</param>
        public CommandTransactionManager(IMemorizer target)
        {

            _target = target;
            _transactions = new Stack<Transaction>();
            _forUndo = new Stack<Transaction>();
            _forUndoView = new TransactionViewList();
            _forRedo = new Stack<Transaction>();
            _forRedoView = new TransactionViewList();


            Sessionid = Guid.NewGuid().ToString();
            var folder = "_cmd_".Combine(_targetGlobalFolder, Sessionid.Replace("-", "")).AsDirectory();
            if (!folder.Exists)
                folder.Create();
            _targetFolder = folder.FullName;


            if (target is INotifyCollectionChanged n)
                n.CollectionChanged += CollectionChanged;

            if (target is INotifyPropertyChanged p)
                p.PropertyChanged += PropertyChanged;

        }



        /// <summary>
        /// Create a new transaction
        /// </summary>
        /// <param name="mode">transaction mode</param>
        /// <param name="name">label transaction</param>
        /// <param name="autocommit">the transaction need a commit for validate</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public Transaction? BeginTransaction(Mode mode, string name, bool autocommit = false)
        {

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            switch (mode)
            {
                case Mode.Recording:
                    return BeginTransaction_Iml(name, autocommit);

                case Mode.Restoring:
                    return BeginRestoreTransaction_Iml();

                case Mode.Paused:
                    return BeginHiddenTransaction_Iml();

                default:
                    throw new NotSupportedException();

            }
        }

        /// <summary>
        /// Begin a new transaction. If the result is null, the transaction manager is paused.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="command"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private Transaction? BeginTransaction_Iml(string name, bool autocommit)
        {

            using (var l = _lock.LockForUpgradeableRead())
            {

                if (Status == StatusTransaction.InPause)
                    return null;

                Transaction trans = new Transaction(this, name, _forUndo.Count + 1, autocommit);
                using (_lock.LockForWrite())
                {
                    if (Status == StatusTransaction.Waiting)
                        Status = StatusTransaction.Recording;
                    _transactions.Push(trans);

                }

                return trans;

            }

        }

        private Transaction? BeginRestoreTransaction_Iml()
        {

            using (var l = _lock.LockForUpgradeableRead())
            {

                if (Status == StatusTransaction.Restoring)
                    throw new InvalidOperationException("restore already began");

                if (Status == StatusTransaction.InPause)
                    return null;

                var trans = new Transaction(this, "Restoring", _forUndo.Count + 1, true)
                {
                    ResumeToEnd = true,
                };

                using (_lock.LockForWrite())
                {
                    Status = StatusTransaction.Restoring;
                    _transactions.Push(trans);
                }

                return trans;

            }

        }

        private Transaction BeginHiddenTransaction_Iml()
        {

            using (var l = _lock.LockForUpgradeableRead())
            {

                if (Status == StatusTransaction.InPause)
                    return null;

                if (_transactions.Count > 0)
                    throw new InvalidOperationException("transaction already began");

                bool p = Status != StatusTransaction.InPause;

                var trans = new Transaction(this, string.Empty, _forUndo.Count + 1, true)
                {
                    ResumeToEnd = p
                };

                using (_lock.LockForWrite())
                {
                    _transactions.Push(trans);
                    Status = StatusTransaction.InPause;

                }

                return trans;

            }

        }




        /// <summary>
        /// Restore in specific transaction state and forget all the next transactions
        /// </summary>
        /// <param name="cmd">backup to restore</param>
        public void Undo(Transaction cmd)
        {
            Undo(cmd.Index);
        }

        /// <summary>
        /// Restore in specific transaction state and forget all the next transactions
        /// </summary>
        /// <param name="cmd">index to restore</param>
        public void Undo(int ind)
        {

            RefreshContext context = null;
            var index = ind - 1;
            Transaction transaction = null;

            var act = () =>
            {
                Status = StatusTransaction.Waiting;
                if (transaction != null)
                {
                    context = new RefreshContext(this.Sessionid, transaction);
                    _target.Restore(context);
                }
            };

            using (var l1 = _lock.LockForUpgradeableRead(act))
            {

                if (_transactions.Count > 0)
                    throw new InvalidOperationException("transaction already began");

                using (var l2 = _lock.LockForWrite())
                    if (index > 0 && _forUndo.Count > 0)
                        while (_forUndo.Count > 0 && _forUndo.Peek().Index >= index)
                        {

                            transaction = _forUndo.Pop();
                            _forUndoView.Pop();
                            _forRedo.Push(transaction);
                            _forRedoView.Push(transaction.GetView());

                            if (index == transaction.Index)
                                break;
                            else
                                transaction = null;

                        }

                    else
                    {

                        while (_forUndo.Count > 0)
                        {
                            transaction = _forUndo.Pop();
                            _forUndoView.Pop();
                            _forRedo.Push(transaction);
                            _forRedoView.Push(transaction.GetView());
                        }

                        transaction = _initialState;

                    }

            }

        }

        /// <summary>
        /// Restore in specific transaction state and forget all the previous transaction.
        /// </summary>
        /// <param name="index">index to restore</param>
        public void Redo(int index)
        {

            RefreshContext context = null;

            Transaction transaction = null;
            var act = () =>
            {
                Status = StatusTransaction.Waiting;
                if (transaction != null)
                {
                    context = new RefreshContext(this.Sessionid, transaction);
                    _target.Restore(context);
                }
            };

            using (var l1 = _lock.LockForUpgradeableRead(act))
                if (_forRedo.Count > 0)
                {

                    if (_transactions.Count > 0)
                        throw new InvalidOperationException("transaction already began");

                    using (var l2 = _lock.LockForWrite())
                        while (_forRedo.Count > 0 && _forRedo.Peek().Index <= index)
                        {

                            transaction = _forRedo.Pop();
                            _forRedoView.Pop();
                            _forUndo.Push(transaction);
                            _forUndoView.Push(transaction.GetView());

                            if (index == transaction.Index)
                                break;
                            else
                                transaction = null;


                        }

                }

        }

        /// <summary>
        /// Restore in specific transaction state and forget all the previous transaction.
        /// </summary>
        /// <param name="cmd">backup to restore</param>
        public void Redo(Transaction cmd)
        {
            Redo(cmd.Index);
        }



        /// <summary>
        /// Reset the transaction manager
        /// </summary>
        public void Reset()
        {

            using (_lock.LockForUpgradeableRead())
                if (_forUndo.Count > 0 || _forRedo.Count > 0 || _transactions.Count > 0)
                    using (_lock.LockForWrite())
                        ResetImpl();

        }

        public void Initialize()
        {

            using (_lock.LockForUpgradeableRead())
            using (_lock.LockForWrite())
            {

                ResetImpl();

                this._initialState = new Transaction(this, "Initialize", 0, true);
                this._initialState.Save(_target);

            }

        }



        /// <summary>
        /// Return the status of the transaction manager
        /// </summary>
        public StatusTransaction Status { get; private set; }

        /// <summary>
        /// Return the list of transaction stored and waiting for undo states.
        /// </summary>
        public TransactionViewList UndoList => _forUndoView;

        /// <summary>
        /// Return the list of transaction stored and waiting for redo states.
        /// </summary>
        public TransactionViewList RedoList => _forRedoView;

        /// <summary>
        /// Return number of transaction stored and waiting for undo states.
        /// </summary>
        public int UndoCount => _forUndo.Count;




        /// <summary>
        /// Unique session id
        /// </summary>
        public string Sessionid { get; }

        /// <summary>
        /// Return the count of current transaction
        /// </summary>
        public int Count => _transactions.Count;


        #region ITransactionManagerBase

        /// <summary>
        /// Commit current transaction
        /// </summary>
        void IInternalTransaction.Commit()
        {

            Transaction currentTransaction = null;

            var act = () =>
            {
                if (currentTransaction != null)
                {
                    Status = currentTransaction.LastStatus;
                }
            };

            using (_lock.LockForUpgradeableRead(act))
            {

                if (_transactions.Count == 0 || Status == StatusTransaction.Waiting)
                    throw new InvalidOperationException("transaction not began");

                using (_lock.LockForWrite())
                {
                    currentTransaction = _transactions.Pop();
                    if (Status == StatusTransaction.Recording)
                    {
                        uint? currentCrc = currentTransaction.GetCrc();
                        if (haschangedAfterLastChange(currentCrc))
                        {
                            currentTransaction.Save(_target);
                            _forUndo.Push(currentTransaction);
                            _forUndoView.Push(currentTransaction.GetView());
                        }
                        _forRedo.Clear();
                    }

                }

            }

        }

        private bool haschangedAfterLastChange(uint? currentCrc)
        {
            bool toStore = true;
            if (GetLastTransactionCrc() == currentCrc)
                toStore = false;
            return toStore;
        }

        private uint GetLastTransactionCrc()
        {
            if (_forUndo.Count > 0)
                return _forUndo.Peek().GetCrc().Value;
            return _initialState.GetCrc().Value;
        }

        /// <summary>
        /// Aboard the current transaction
        /// </summary>
        void IInternalTransaction.Rollback()
        {

            RefreshContext context = null;
            Transaction transaction = null;
            bool delete = _transactions.Count > 0;
            var act = () =>
            {
                if (transaction != null)
                {
                    context = new RefreshContext(this.Sessionid, transaction);
                    _target.Restore(context);
                    Status = transaction.LastStatus;
                    if (delete)
                        this.Delete(transaction);
                }
            };

            using (var l2 = _lock.LockForWrite())
                transaction = delete ? _transactions.Pop() : _initialState;

        }

        Stream IInternalTransaction.GetStreamForWriting(Transaction transaction)
        {
            return GetStreamForWriting_Iml(transaction);
        }

        /// <summary>
        /// Get the stream associated with the transaction.
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public virtual Stream GetStreamForWriting_Iml(Transaction transaction)
        {

            FileInfo f = GetFile(transaction);
            if (f.Exists)
                f.Delete();

            f.Refresh();

            return f.OpenWrite();

        }

        Stream IInternalTransaction.GetStreamForReading(Transaction transaction)
        {
            return GetStreamForReading_Impl(transaction);
        }

        /// <summary>
        /// Get the stream associated with the transaction.
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public virtual Stream GetStreamForReading_Impl(Transaction transaction)
        {

            FileInfo f = GetFile(transaction);
            if (f.Exists)
                return f.OpenRead();

            throw new FileNotFoundException(f.FullName);

        }

        protected virtual void Delete(Transaction transaction)
        {

            FileInfo f = GetFile(transaction);
            if (f.Exists)
                f.Delete();

            f.Refresh();
        }

        protected virtual FileInfo GetFile(Transaction transaction)
        {
            var _path = _targetFolder.Combine(transaction.Index.ToString() + ".json");
            var f = _path.AsFile();
            f.Refresh();
            return f;
        }


        #endregion ITransactionManagerBase






        /// <summary>
        /// Initializes a new instance of the <see cref="CommandTransactionManager"/> class.
        /// </summary>
        /// <param name="folder"></param>
        public static void SetFolder(string folder)
        {
            _targetGlobalFolder = folder;
        }

        private void ResetImpl()
        {

            if (_transactions.Count > 0)
                _transactions.Clear();

            if (_forUndo.Count > 0)
            {

                foreach (var item in _forUndo)
                {
                    Delete(item);
                }

                _forUndo.Clear();
                _forUndoView.Clear();
            }

            if (_forRedo.Count > 0)
            {

                foreach (var item in _forUndo)
                {
                    Delete(item);
                }

                _forRedo.Clear();
                _forRedoView.Clear();

            }

            Status = StatusTransaction.Waiting;

        }

        protected virtual void PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (Status == StatusTransaction.Waiting)
                throw new InvalidOperationException("Transaction not initialized");
        }

        protected virtual void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (Status == StatusTransaction.Waiting)
                throw new InvalidOperationException("Transaction not initialized");
        }

        private Transaction _initialState;
        private Stack<Transaction> _transactions;
        private string _targetFolder;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly IMemorizer _target;
        private Stack<Transaction> _forUndo;
        private readonly TransactionViewList _forUndoView;
        private Stack<Transaction> _forRedo;
        private readonly TransactionViewList _forRedoView;
        //private int _currentIndex = 0;
        private static string _targetGlobalFolder;

    }

}


