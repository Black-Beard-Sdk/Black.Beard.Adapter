using System.Collections;
using System.Collections.Specialized;

namespace Bb.Commands
{

    
    /// <summary>
    /// Represents a collection of <see cref="TransactionView"/> objects that supports push, pop, and clear operations.
    /// </summary>
    public class TransactionViewList : INotifyCollectionChanged, IEnumerable<TransactionView>
    {
        private Stack<TransactionView> _list = new Stack<TransactionView>();

        /// <summary>
        /// Adds an item to the top of the collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Push(TransactionView item)
        {
            _list.Push(item);
            var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item);
            CollectionChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Removes and returns the item at the top of the collection.
        /// </summary>
        public void Pop()
        {
            var item = _list.Pop();
            var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item);
            CollectionChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            _list.Clear();
            var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            CollectionChanged?.Invoke(this, e);
        }

        /// <inheritdoc/>
        public IEnumerator<TransactionView> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }

}
