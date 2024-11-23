using Bb.ComponentModel.Accessors;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bb.Diagrams
{

    /// <summary>
    /// Represents a thread-safe collection of diagram nodes with unique keys.
    /// </summary>
    /// <typeparam name="TValue">The type of elements in the collection, which must implement IKey.</typeparam>
    public class DiagramList<TKey, TValue>
        : ICollection<TValue>
        , INotifyCollectionChanged
        , INotifyPropertyChanging
        , INotifyPropertyChanged
        where TKey : IComparable
    {


        static DiagramList()
        {

            string name = string.Empty;
            var a = typeof(TValue).GetAccessors();

            foreach (var item in a)
                if (item.ContainsAttribute<KeyAttribute>())
                {
                    name = item.Name;
                    break;
                }

            if (string.IsNullOrEmpty(name))
                throw new Exception($"The type {nameof(TValue)} must implement IKey of add an attribute Key on the property that be functional key.");

            var key = a.Where(c => c.Name == name).FirstOrDefault();
            _key = (c) => (TKey)key.GetValue(c);

        }

        public DiagramList()
        {

        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="index">The key of the element to get or set.</param>
        /// <returns>The element with the specified key.</returns>
        public TValue this[TKey index]
        {
            get
            {
                using (_lock.LockForRead())
                    return _dic[index];
            }
            set
            {
                Unsuscribes(_dic[index]);
                using (_lock.LockForWrite(() => Suscribes(value)))
                {
                    _dic[index] = value;
                }
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                using (_lock.LockForRead())
                    return _dic.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Adds a range of elements to the collection.
        /// </summary>
        /// <param name="newItems">The elements to add to the collection.</param>
        public void AddRange(params TValue[] newItems)
        {
            List<TValue> listAdded = new List<TValue>(newItems.Length);
            List<(TValue, TValue)> listUpdated = new List<(TValue, TValue)>(newItems.Length);


            using (_lock.LockForUpgradeableRead())
            {
                foreach (var newItem in newItems)
                {
                    var key = _key(newItem);
                    if (!_dic.TryGetValue(key, out TValue oldValue))
                    {
                        using (_lock.LockForWrite())
                        {
                            if (!_dic.TryGetValue(key, out oldValue))
                            {
                                _dic.Add(key, newItem);
                                listAdded.Add(newItem);
                            }
                        }
                    }
                    else
                    {
                        using (_lock.LockForWrite())
                        {
                            if (_dic.TryGetValue(key, out oldValue))
                            {
                                _dic[key] = newItem;
                                listUpdated.Add((oldValue, newItem));
                            }
                        }
                    }
                }
            }

            if (listAdded.Count > 0)
            {

                foreach (var item in listAdded)
                    Suscribes(item);

                OnChangedInCollection(NotifyCollectionChangedAction.Add, listAdded.ToArray());

            }

            if (listUpdated.Count > 0)
                foreach (var (oldValue, newItem) in listUpdated)
                {
                    Unsuscribes(oldValue);
                    Suscribes(newItem);
                    OnReplacedInCollection(oldValue, newItem);
                }
        }

        /// <summary>
        /// Adds an element to the collection.
        /// </summary>
        /// <param name="newItem">The element to add to the collection.</param>
        public void Add(TValue newItem)
        {

            bool t = false;
            TValue oldValue = default;
            var key = _key(newItem);

            var dispose = () =>
            {

                if (t)
                {
                    if (oldValue != null)
                    {
                        Unsuscribes(oldValue);
                        OnReplacedInCollection(oldValue, newItem);
                    }
                }
                else
                    OnChangedInCollection(NotifyCollectionChangedAction.Add, new[] { newItem });

                Suscribes(newItem);

            };

            using (_lock.LockForUpgradeableRead(dispose))
            {
                if (!_dic.ContainsKey(key))
                    using (_lock.LockForWrite())
                    {
                        if (!_dic.ContainsKey(key))
                        {
                            _dic.Add(key, newItem);
                            t = true;
                        }
                    }
                
                else
                    using (_lock.LockForWrite())
                    {
                        if (_dic.TryGetValue(key, out oldValue))
                        {
                            t = true;
                            _dic[key] = newItem;
                        }
                    }

            }

        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            TValue[] items = null;

            foreach (var item in _dic.Values)
                Unsuscribes(item);

            using (_lock.LockForWrite())
            {
                items = _dic.Values.ToArray();
                _dic.Clear();
            }

            if (items != null)
                OnChangedInCollection(NotifyCollectionChangedAction.Remove, items);

        }

        /// <summary>
        /// Removes the first occurrence of a specific element from the collection.
        /// </summary>
        /// <param name="item">The element to remove from the collection.</param>
        /// <returns>true if the element was successfully removed; otherwise, false.</returns>
        public bool Remove(TValue item)
        {
            bool t = false;

            var key = _key(item);

            var dispose = () =>
            {
                Unsuscribes(item);
                OnChangedInCollection(NotifyCollectionChangedAction.Remove, [item]);
            };

            using (_lock.LockForUpgradeableRead(dispose))
                if (_dic.ContainsKey(key))
                    using (_lock.LockForWrite())
                        if (_dic.ContainsKey(key))
                        {
                            _dic.Remove(key);
                            t = true;
                            return true;
                        }

            return false;

        }

        /// <summary>
        /// Determines whether the collection contains an element with the specified key.
        /// </summary>
        /// <param name="item">The element to locate in the collection.</param>
        /// <returns>true if the element is found in the collection; otherwise, false.</returns>
        public bool ContainsKey(TValue item)
        {
            using (_lock.LockForRead())
                return _dic.ContainsKey(_key(item));
        }

        /// <summary>
        /// Determines whether the collection contains a specific element.
        /// </summary>
        /// <param name="item">The element to locate in the collection.</param>
        /// <returns>true if the element is found in the collection; otherwise, false.</returns>
        public bool Contains(TValue item)
        {

            var key = _key(item);
            using (_lock.LockForRead())
            {
                if (_dic.TryGetValue(key, out var r))
                    return r.Equals(item);
                return false;
            }

        }


        /// <summary>
        /// Copies the elements of the collection to an array, starting at a particular array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the collection.</param>
        /// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
        public void CopyTo(TValue[] array, int arrayIndex)
        {

            using (_lock.LockForRead())
            {
                var o = new List<TValue>(_dic.Values);
                o.Sort((x, y) => _key(x).CompareTo(_key(y)));
                o.CopyTo(array, arrayIndex);
            }
        }

        /// <summary>
        /// Tries to get the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter.</param>
        /// <returns>true if the collection contains an element with the specified key; otherwise, false.</returns>
        /// <example>
        /// <code>
        /// DiagramList<MyType> list = new DiagramList<MyType>();
        /// MyType value;
        /// if (list.TryGetValue(someTKey, out value))
        /// {
        ///     // Use the value
        /// }
        /// else
        /// {
        ///     // Handle the case where the key is not found
        /// }
        /// </code>
        /// </example>
        public bool TryGetValue(TKey key, out TValue value)
        {
            using (_lock.LockForRead())
                return _dic.TryGetValue(key, out value);
        }


        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator for the collection.</returns>
        public IEnumerator<TValue> GetEnumerator()
        {
            using (_lock.LockForRead())
            {
                var o = new List<TValue>(_dic.Values);
                o.Sort((x, y) => _key(x).CompareTo(_key(y)));
                return o.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            using (_lock.LockForRead())
            {
                var o = new List<TValue>(_dic.Values);
                o.Sort((x, y) => _key(x).CompareTo(_key(y)));
                return o.GetEnumerator();
            }
        }

        /// <summary>
        /// Raises the CollectionChanged event with the specified action and items.
        /// </summary>
        /// <param name="impact">The action that caused the event.</param>
        /// <param name="newitems">The items affected by the change.</param>
        protected void OnReplacedInCollection(object oldItem, object newItem)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldItem, newItem));
        }

        /// <summary>
        /// Raises the CollectionChanged event with the specified action and items.
        /// </summary>
        /// <param name="oldItem">The old item replaced in the collection.</param>
        /// <param name="newItem">The new item replacing the old item in the collection.</param>

        protected void OnChangedInCollection(NotifyCollectionChangedAction impact, TValue[] newitems)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(impact, newitems));
        }

        private void Unsuscribes(TValue item)
        {
            if (item is INotifyPropertyChanged n1)
                n1.PropertyChanged -= N_PropertyChanged;

            if (item is INotifyPropertyChanging n2)
                n2.PropertyChanging -= N_PropertyChanging;
        }

        private void Suscribes(TValue item)
        {
            if (item is INotifyPropertyChanged n1)
                n1.PropertyChanged += N_PropertyChanged;

            if (item is INotifyPropertyChanging n2)
                n2.PropertyChanging += N_PropertyChanging;
        }


        private void N_PropertyChanging(object? sender, PropertyChangingEventArgs e)
        {
            PropertyChanging?.Invoke(sender, e);
        }

        private void N_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }     

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event PropertyChangingEventHandler? PropertyChanging;
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly Dictionary<TKey, TValue> _dic = new Dictionary<TKey, TValue>();
        private static readonly Func<TValue, TKey> _key;

    }


}
