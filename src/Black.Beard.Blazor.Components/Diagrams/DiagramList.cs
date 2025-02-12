using Bb.Commands;
using Bb.ComponentModel.Accessors;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bb.Diagrams
{

    public static class DiagramListExtension
    {

        public static IEnumerable<KeyValuePair<TKey, TValue>> As<TKey, TValue>(this DiagramList<TKey, TValue> self)
            where TKey : IComparable
            where TValue : class
        {

            return self;

        }

    }

    public class DiagramListJsonConverter : JsonConverter<DiagramList>
    {

        public DiagramListJsonConverter()
        {

        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(DiagramList).IsAssignableFrom(typeToConvert);
        }

        public override DiagramList? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {

            var types = ResolveTypes(typeToConvert);

            if (types != null)
            {
                var converter = (JsonConverter<object>)Activator.CreateInstance( typeof(DiagramListJsonConverter<,>).MakeGenericType(types));
                return (DiagramList)converter.Read(ref reader, typeToConvert, options);
            }

            return null;

        }

        private static Type[] ResolveTypes(Type typeToConvert)
        {

            if (typeToConvert.IsConstructedGenericType)
            {

                var type = typeToConvert.GetGenericTypeDefinition();
                if (type == typeof(DiagramList<,>))
                {

                    var a = typeToConvert.GetGenericArguments();
                    if (a.Length == 2)

                        return a;
                }

            }

            return ResolveTypes(typeToConvert.BaseType);

        }

        public override void Write(Utf8JsonWriter writer, DiagramList value, JsonSerializerOptions options)
        {

            var types = ResolveTypes(value.GetType());
            if (types != null)
            {
                var converter = (JsonConverter<object>)Activator.CreateInstance(typeof(DiagramListJsonConverter<,>).MakeGenericType(types));
                converter.Write(writer, value, options);
            }

        }

    }

    public class DiagramListJsonConverter<TKey, TValue> : JsonConverter<object>
        where TKey : IComparable
        where TValue : class
    {

        public DiagramListJsonConverter()
        {

        }

        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {

            var list = (DiagramList<TKey, TValue>)Activator.CreateInstance(typeToConvert);

            while (reader.Read())
            {

                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                var value = (TValue)JsonSerializer.Deserialize(ref reader, typeof(TValue), options);
                list.Add(value);

            }

            return list;

        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            var list = (DiagramList<TKey, TValue>)value;
            JsonSerializer.Serialize(writer, list.Values, options);
        }


    }

    public class DiagramList
    {



    }

    /// <summary>
    /// Represents a thread-safe collection of diagram nodes with unique keys.
    /// </summary>
    /// <typeparam name="TValue">The type of elements in the collection, which must implement IKey.</typeparam>
    public class DiagramList<TKey, TValue>
        : DiagramList
        , ICollection<TValue>
        , IDictionary<TKey, TValue>
        , ICollection<KeyValuePair<TKey, TValue>>
        , IEnumerable<KeyValuePair<TKey, TValue>>
        , INotifyCollectionChanged
        , INotifyPropertyChanging
        , INotifyPropertyChanged
        , IRestorable
        where TKey : IComparable
        where TValue : class
    {




        #region ctors

        /// <summary>
        /// Initializes the static fields of the DiagramList class.
        /// </summary>
        /// <exception cref="Exception"></exception>
        static DiagramList()
        {

            var a = typeof(TValue).GetAccessors();
            foreach (var item in a)
                if (item.ContainsAttribute<KeyAttribute>())
                {
                    _key = (c) => (TKey)item.GetValue(c);
                    break;
                }

            if (_key == default)
                throw new Exception($"The type {nameof(TValue)} have no key. Please add an attribute Key on the property that be functional key.");

        }

        /// <summary>
        /// Constructs a List. The list is initially empty and has a capacity
        /// of zero. Upon adding the first element to the list the capacity is
        /// increased to DefaultCapacity, and then increased in multiples of two
        /// as required.
        /// </summary>
        public DiagramList()
        {
            _dic = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// Constructs a List with a given initial capacity. The list is
        /// initially empty, but will have room for the given number of elements
        /// before any reallocations are required.
        /// </summary>
        /// <param name="capacity"></param>
        public DiagramList(int capacity)
        {
            _dic = new Dictionary<TKey, TValue>(capacity);
        }

        /// <summary>
        /// Constructs a List, copying the contents of the given collection. The
        /// size and capacity of the new list will both be equal to the size of the
        /// given collection.
        /// </summary>
        /// <param name="collection"></param>
        public DiagramList(IEnumerable<TValue> collection)
        {
            _dic = collection.ToDictionary(c => _key(c));
        }

        #endregion ctors


        /// <summary>
        /// Try to add a range of elements to the collection or replace them if the keys already exists.
        /// </summary>
        /// <param name="newItems">The elements to add to the collection.</param>
        public void AddRange(params TValue[] newItems)
        {
            AddRange((IEnumerable<TValue>)newItems);
        }

        /// <summary>
        /// Try to add a range of elements to the collection or replace them if the keys already exists.
        /// </summary>
        /// <param name="newItems">The elements to add to the collection.</param>
        public void AddRange(IEnumerable<TValue> newItems)
        {

            if (newItems.Any())
            {

                var length = newItems.Count();
                List<TValue> listAdded = new List<TValue>(length);
                List<(TValue, TValue)> listUpdated = new List<(TValue, TValue)>(length);


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

        }

        /// <summary>
        /// Try to add element to the collection or replace it if the keys already exists.
        /// </summary>
        /// <param name="newItem">The element to add to the collection.</param>
        public void Add(TValue newItem)
        {
            var key = _key(newItem);
            Add(key, newItem);
        }


        /// <summary>
        /// Adds a new item to the collection.
        /// </summary>
        /// <param name="newItem">The item to add to the collection.</param>
        /// <remarks>
        /// This method adds a new item to the collection. If the item's key already exists in the collection, it will be replaced with the new item.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when the newItem parameter is null.</exception>
        public void Add(KeyValuePair<TKey, TValue> value)
        {
            Add(value.Key, value.Value);
        }

        /// <summary>
        /// Try to add element to the collection or replace it if the keys already exists.
        /// </summary>
        /// <param name="key">The key of the element to add to the collection.</param>
        /// <param name="value">The element to add to the collection.</param>
        public void Add(TKey key, TValue value)
        {

            bool t = false;
            TValue oldValue = default;

            var dispose = () =>
            {

                if (t)
                {
                    if (oldValue != null)
                    {
                        Unsuscribes(oldValue);
                        OnReplacedInCollection(oldValue, value);
                    }
                    else
                        OnChangedInCollection(NotifyCollectionChangedAction.Add, new[] { value });

                    Suscribes(value);

                }

            };

            using (_lock.LockForUpgradeableRead(dispose))
            {
                if (!_dic.ContainsKey(key))
                    using (_lock.LockForWrite())
                    {
                        if (!_dic.ContainsKey(key))
                        {
                            _dic.Add(key, value);
                            t = true;
                        }
                    }

                else
                    using (_lock.LockForWrite())
                    {
                        if (_dic.TryGetValue(key, out oldValue))
                        {
                            t = true;
                            _dic[key] = value;
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
        /// Removes the first occurrence of a specific element from the collection.
        /// </summary>
        /// <param name="value">The element to remove from the collection.</param>
        /// <returns>true if the element was successfully removed; otherwise, false.</returns>
        public bool Remove(KeyValuePair<TKey, TValue> value)
        {
            return Remove(value.Key);
        }

        /// <summary>
        /// Removes the first occurrence of a specific element from the collection.
        /// </summary>
        /// <param name="key">The key of the element to remove from the collection.</param>
        /// <returns>true if the element was successfully removed; otherwise, false.</returns>
        public bool Remove(TKey key)
        {

            TValue? item = default;
            bool t = false;

            var dispose = () =>
            {
                Unsuscribes(item);
                OnChangedInCollection(NotifyCollectionChangedAction.Remove, [item]);
            };

            using (_lock.LockForUpgradeableRead(dispose))
                if (_dic.TryGetValue(key, out item))
                {

                    if (_dic.ContainsKey(key))
                        using (_lock.LockForWrite())
                            if (_dic.ContainsKey(key))
                            {
                                _dic.Remove(key);
                                t = true;
                                return true;
                            }

                }

            return t;

        }

        /// <summary>
        /// Removes the first occurrence of a specific element from the collection.
        /// </summary>
        /// <param name="items"></param>
        public void RemoveRange(params TValue[] items)
        {
            RemoveRange((IEnumerable<TValue>)items);
        }

        /// <summary>
        /// Try to remove a range of elements to the collection.
        /// </summary>
        /// <param name="items">items to add</param>
        public void RemoveRange(IEnumerable<TValue> items)
        {
            if (items.Any())
            {

                List<TValue> listRemoved = new List<TValue>(items.Count());

                var dispose = () =>
                {
                    var a = listRemoved.ToArray();
                    Unsuscribes(a);
                    OnChangedInCollection(NotifyCollectionChangedAction.Remove, a);
                };

                using (_lock.LockForUpgradeableRead(dispose))
                    foreach (var item in items)
                    {
                        var key = _key(item);
                        if (_dic.ContainsKey(key))
                            using (_lock.LockForWrite())
                                if (_dic.ContainsKey(key))
                                {
                                    _dic.Remove(key);
                                    listRemoved.Add(item);
                                }
                    }


            }
        }

        /// <summary>
        /// Try to removes the element with the specified key from the collection.
        /// </summary>
        /// <param name="key">key of the item to remove</param>
        /// <returns></returns>
        public bool TryRemove(TKey key)
        {

            TValue item = default;
            using (_lock.LockForRead())
                if (!_dic.TryGetValue(key, out item))
                    return false;

            return Remove(item);

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
        /// Return true if the collection contains the specified key.
        /// </summary>
        /// <param name="value">item to evaluate</param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<TKey, TValue> value)
        {
            using (_lock.LockForRead())
                return _dic.ContainsKey(value.Key);
        }

        /// <summary>
        /// Return true if the collection contains the specified key.
        /// </summary>
        /// <param name="key">key to evaluate</param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            using (_lock.LockForRead())
                return _dic.ContainsKey(key);
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
        /// Copies the elements of the collection to an array, starting at a particular array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the collection.</param>
        /// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {

            var d = _dic as IDictionary<TKey, TValue>;
            using (_lock.LockForRead())
            {
                d.CopyTo(array, arrayIndex);
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

        private void Unsuscribes(params TValue[] items)
        {

            foreach (var item in items)
            {

                if (item is INotifyPropertyChanged n1)
                    n1.PropertyChanged -= N_PropertyChanged;

                if (item is INotifyPropertyChanging n2)
                    n2.PropertyChanging -= N_PropertyChanging;

            }
        }

        private void Suscribes(params TValue[] items)
        {

            foreach (var item in items)
            {

                if (item is INotifyPropertyChanged n1)
                    n1.PropertyChanged += N_PropertyChanged;

                if (item is INotifyPropertyChanging n2)
                    n2.PropertyChanging += N_PropertyChanging;

            }

        }

        private void N_PropertyChanging(object? sender, PropertyChangingEventArgs e)
        {
            PropertyChanging?.Invoke(sender, e);
        }

        private void N_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        internal void Unsubscribes()
        {
            foreach (var item in _dic.Values)
                Unsuscribes(item);
        }





        /// <summary>
        /// Return the elements that are in the model but not in the current collection.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IEnumerable<TValue> FindMissingFrom(DiagramList<TKey, TValue> model)
        {

            foreach (var item in model)
                if (!ContainsKey(item))
                    yield return item;
        }

        public IEnumerable<TValue> FindMissingFrom(IEnumerable<TKey> model)
        {

            foreach (var item in model)
                if (!ContainsKey(item))
                    if (TryGetValue(item, out var value))
                        yield return value;
        }

        public IEnumerable<TValue> Find(IEnumerable<TKey> model)
        {

            foreach (var item in model)
                if (ContainsKey(item))
                    if (TryGetValue(item, out var value))
                        yield return value;
        }

        public IEnumerable<TValue> Find(DiagramList<TKey, TValue> model)
        {
            foreach (var item in model)
                if (TryGetValue(_key(item), out var value))
                    yield return value;
        }

        public bool RestoreRemove(object model, RefreshContext context)
        {

            bool changed = false;

            TValue[] items = null;

            if (model is DiagramList<TKey, TValue> list)
            {

                // remove items that are not in the model
                items = list.FindMissingFrom(this).ToArray();
                if (items.Length > 0)
                {
                    changed = true;
                    RemoveRange(items);
                    foreach (var item in items)
                        context.Apply<TValue>(RefreshStrategy.Removed, item, _key(item).ToString());
                }

            }

            return changed;

        }

        public bool Restore(object model, RefreshContext context)
        {

            bool changed = false;

            TValue[] items = null;

            if (model is DiagramList<TKey, TValue> list)
            {

                // remove items that are not in the model
                items = list.FindMissingFrom(this).ToArray();
                if (items.Length > 0)
                {
                    changed = true;
                    RemoveRange(items);
                    foreach (var item in items)
                        context.Apply<TValue>(RefreshStrategy.Removed, item, _key(item).ToString());
                }

                List<TValue> addedItems = new List<TValue>();
                List<(TValue, TValue)> updatedItems = new List<(TValue, TValue)>();
                RestoreUpdate(list, context, addedItems, updatedItems);

                if (addedItems.Count > 0)
                {
                    changed = true;
                    context.Register(RefreshStrategy.Added, () =>
                    {
                        AddRange(addedItems);
                        context.Apply(RefreshStrategy.Added, addedItems, $"update list of {typeof(TValue)}");
                    });

                }

                if (updatedItems.Count > 0)
                    context.Register(RefreshStrategy.Updated, () =>
                    {
                        foreach (var item in updatedItems)
                            if (context.ApplyUpdate(item.Item1, item.Item2))
                            {
                                changed = true;
                                var k = _key(item.Item1);
                                context.Apply(RefreshStrategy.Updated, item.Item1, $"update {typeof(TValue)} {k}");
                            }
                    });

            }

            return changed;

        }

        public bool RestoreUpdate(object model, RefreshContext context)
        {

            bool changed = false;

            TValue[] items = null;

            if (model is DiagramList<TKey, TValue> list)
            {

                List<TValue> addedItems = new List<TValue>();
                List<(TValue, TValue)> updatedItems = new List<(TValue, TValue)>();
                RestoreUpdate(list, context, addedItems, updatedItems);

                if (addedItems.Count > 0)
                {
                    changed = true;
                    AddRange(addedItems);
                    context.Apply(RefreshStrategy.Added, addedItems, $"update list of {typeof(TValue)}");
                }

                if (updatedItems.Count > 0)
                    foreach (var item in updatedItems)
                    {

                        var s = item.Item2;
                        var t = item.Item1;

                        if (context.ApplyUpdate(s, t))
                        {
                            changed = true;
                            var k = _key(s);
                            context.Apply(RefreshStrategy.Updated, s, $"update {typeof(TValue)} {k}");
                        }
                    }


            }

            return changed;

        }

        public void RestoreUpdate(DiagramList<TKey, TValue> listSource, RefreshContext context, List<TValue> addedItems, List<(TValue, TValue)> updatedItems)
        {

            foreach (var item in listSource)
            {
                var k = _key(item);
                if (this.TryGetValue(k, out TValue value))
                    updatedItems.Add((value, item));

                else
                {
                    addedItems.Add(item);
                }
            }

        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
            return obj.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {

            int i = 0;

            unchecked
            {
                foreach (var item in _dic.Keys.OrderBy(c => c))
                    i ^= _dic[item].GetHashCode();
            }

            return i;

        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return _dic.GetEnumerator();
        }


        //
        // Résumé :
        //     Gets an System.Collections.Generic.ICollection`1 containing the keys of the System.Collections.Generic.IDictionary`2.
        //
        //
        // Retourne :
        //     An System.Collections.Generic.ICollection`1 containing the keys of the object
        //     that implements System.Collections.Generic.IDictionary`2.
        public ICollection<TKey> Keys
        {
            get
            {
                return _dic.Keys;
            }
        }

        //
        // Résumé :
        //     Gets an System.Collections.Generic.ICollection`1 containing the values in the
        //     System.Collections.Generic.IDictionary`2.
        //
        // Retourne :
        //     An System.Collections.Generic.ICollection`1 containing the values in the object
        //     that implements System.Collections.Generic.IDictionary`2.
        public ICollection<TValue> Values
        {
            get
            {
                return _dic.Values;
            }
        }

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event PropertyChangingEventHandler? PropertyChanging;
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly Dictionary<TKey, TValue> _dic;
        private static readonly Func<TValue, TKey> _key;

    }




}
