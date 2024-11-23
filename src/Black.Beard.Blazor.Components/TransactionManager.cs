using System;
using System.Collections;

namespace Bb
{

    public class CommandTransactionManager : ICommandTransactionManager
    {


        static CommandTransactionManager()
        {
            _targetFolder = Path.GetTempPath();
        }

        public static void SetFolder(string folder)
        {
            _targetFolder = folder;
        }

        /// <summary>
        /// Initialize a new transaction manager
        /// </summary>
        public CommandTransactionManager(ICommandMemorizer target)
        {
            this._target = target;
            _forUndo = new Stack<CommandTransaction>();
            _forUndoView = new CommandTransationViewList();

            _forRedo = new Stack<CommandTransaction>();
            _forRedoView = new CommandTransationViewList();

            this.Sessionid = "_cmd_" + Guid.NewGuid().ToString().Replace("-", "");
            var folder = _targetFolder.Combine(this.Sessionid).AsDirectory();
            TargetFolder = folder.FullName;

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

        public void Resume()
        {

            using (var l = _lock.LockForWrite())
            {
                Status = StatusTransaction.Waiting;
            }

        }

        public StatusTransaction Status { get; private set; }


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

                if (this.Status == StatusTransaction.InPause)
                    return null;

                if (_currentTransaction != null)
                    throw new InvalidOperationException("transaction already began");

                var trans = new CommandTransaction(this, name, this._forUndo.Count + 1);
                trans.InitializeValue(_target);

                using (_lock.LockForWrite())
                {
                    Status = StatusTransaction.Recoding;
                    _currentTransaction = trans;
                }

            }

            return _currentTransaction;

        }

        public CommandTransationViewList UndoList => _forUndoView;

        public CommandTransationViewList RedoList => _forRedoView;

        /// <summary>
        /// Commit current transaction
        /// </summary>
        public void Commit()
        {

            using (var l = _lock.LockForWrite())
            {
                _currentTransaction?.Commit();
                _forUndo.Push(_currentTransaction);
                _forUndoView.Add(_currentTransaction.GetView());
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

                        while (_forUndo.Peek().Index != index)
                        {
                            var c = _forUndo.Pop();
                            _forUndoView.Pop();
                            if (this._target.Mode == MemorizerEnum.Snapshot || cmd == c.Index)
                                _target.Restore(c);
                            _forRedo.Push(c);
                            _forRedoView.Add(c.GetView());
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
        /// <param name="cmd">index to restore</param>
        public void Redo(int cmd)
        {

            using (var l1 = _lock.LockForUpgradeableRead())
            {
                _currentTransaction = null;

                if (_forRedo.Count > 0)
                {

                    Status = StatusTransaction.Restoring;

                    try
                    {
                        while (_forRedo.Peek().Index != cmd)
                        {
                            var c = _forRedo.Pop();
                            _forRedoView.Pop();
                            if (this._target.Mode == MemorizerEnum.Snapshot || cmd == c.Index)
                                _target.Restore(c);
                            _forUndo.Push(c);
                            _forUndoView.Add(c.GetView());
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

        /// <summary>
        /// Return number of transaction stored and waiting for undo states.
        /// </summary>
        public int UndoCount => _forUndo.Count;
  

        void ICommandTransactionManager.Reset()
        {
            _currentTransaction = null;
            Status = StatusTransaction.Waiting;
            _forUndo.Clear();
            _forUndoView.Clear();
            _forRedo.Clear();
            _forRedoView.Clear();
        }

        private CommandTransaction _currentTransaction;

        public string Sessionid { get; }

        internal string TargetFolder { get; }

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly ICommandMemorizer _target;
        private Stack<CommandTransaction> _forUndo;
        private readonly CommandTransationViewList _forUndoView;
        private Stack<CommandTransaction> _forRedo;
        private readonly CommandTransationViewList _forRedoView;
        private int _currentIndex = 0;
        private static string _targetFolder;

    }

}
