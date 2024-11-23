using System.Collections;
using System.Collections.Specialized;

namespace Bb
{
    public class CommandTransaction : IDisposable
    {

        /// <summary>
        /// initialize a new transaction
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="target"></param>
        /// <param name="model"></param>
        internal CommandTransaction(CommandTransactionManager manager, string name, int index)
        {
            _manager = manager;
            Index = index;
            Label = name;
        }

        internal void InitializeValue(ICommandMemorizer target)
        {
            var path = _manager.TargetFolder.AsDirectory();
            if (!path.Exists)
                path.Create();
            _path = path.Combine(Index.ToString());

            var f = _path.AsFile();
            if (f.Exists)
                f.Delete();
            using (var stream = f.OpenWrite())
            {
                target.Memorize(stream);
            }
        
        }

        public string Label { get; set; }

        private string _path;

        public int Index { get; private set; }

        public void Commit()
        {
            _modelBefore = null;
        }

        /// <summary>
        /// Dispose the transaction
        /// </summary>
        public void Dispose()
        {
            _manager.Rollback();
        }

        public CommandTransactionView GetView()
        {
            return new CommandTransactionView(this);
        }

        private readonly CommandTransactionManager _manager;
        private MemoryStream _modelBefore;


        //private readonly int _crc;
    }

    public class CommandTransactionView
    {

        public CommandTransactionView(CommandTransaction transaction)
        {
            Index = transaction.Index;
            Label = transaction.Label;
        }

        public int Index { get; set; }
        public string Label { get; set; }

    }

    public class CommandTransationViewList : INotifyCollectionChanged, IEnumerable<CommandTransactionView>
    {

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Add(CommandTransactionView item)
        {
            _list.Push(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void Pop()
        {
            var item = _list.Pop();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }

        public void Clear()
        {
            _list.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public IEnumerator<CommandTransactionView> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        private Stack<CommandTransactionView> _list = new Stack<CommandTransactionView>();

    }

}
