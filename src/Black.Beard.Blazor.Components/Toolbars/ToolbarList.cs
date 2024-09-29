using Microsoft.Extensions.Logging.Abstractions;
using System.Collections;
using System.Collections.Specialized;
using static MudBlazor.CategoryTypes;

namespace Bb.Toolbars
{

    public class ToolbarList : ICollection<ToolbarGroup>, INotifyCollectionChanged
    {

        public ToolbarList(Guid id, string name, IEnumerable<ToolbarGroup>? items = null)
        {
            this.Id = id;
            this.Name = name;
            if (items != null)
                _items = new List<ToolbarGroup>(items);
            else
                _items = new List<ToolbarGroup>();
        }

        public bool IsEmpty { get; set; }

        public int Count => _items.Count;

        public bool IsReadOnly => false;

        public void Add(params ToolbarGroup[] items)
        {

            List<ToolbarGroup> _added = new List<ToolbarGroup>(items.Length);
            if (items != null)
                foreach (var item in items)
                    if (item != null && !_items.Any(c => c.Id == item.Id))
                    {
                        _items.Add(item);
                        _added.Add(item);
                        item.SetParent(this);
                    }

            if (_added.Count > 0 && CollectionChanged != null)
            {
                var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, _added);
                CollectionChanged.Invoke(this, e);
            }

        }

        internal void Item_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
                CollectionChanged.Invoke(this, e);
        }

        public void Add(ToolbarGroup item)
        {
            if (item != null && !_items.Contains(item) && !_items.Any(c => c.Name == item.Name))
            {
                _items.Add(item);
                item.SetParent(this);

                if (CollectionChanged != null)
                {
                    var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<ToolbarGroup>() { item });
                    CollectionChanged.Invoke(this, e);
                }
            }
        }

        public bool Remove(ToolbarGroup item)
        {

            if (item != null)
                if (_items.Remove(item))
                {
                    item.SetParent(null);

                    if (CollectionChanged != null)
                    {
                        var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<ToolbarGroup>() { item });
                        CollectionChanged.Invoke(this, e);
                    }
                    return true;
                }

            return false;

        }

        public void Remove(params ToolbarGroup[] items)
        {

            List<ToolbarGroup> _removed = new List<ToolbarGroup>(items.Length);
            if (items != null)
                foreach (var item in items)
                    if (item != null && !_items.Contains(item))
                    {
                        _items.Remove(item);
                        _removed.Add(item);
                        item.SetParent(null);
                    }

            if (_removed.Count > 0 && CollectionChanged != null)
            {
                var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, _removed);
                CollectionChanged.Invoke(this, e);
            }

        }

        public void Clear()
        {

            if (_items.Count > 0)
            {

                if (CollectionChanged != null)
                {
                    var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                    CollectionChanged.Invoke(this, e);
                }

                foreach (var item in _items)
                    item.SetParent(null);

                _items.Clear();

            }
        }

        public bool Contains(ToolbarGroup item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(ToolbarGroup[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public IEnumerator<ToolbarGroup> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// Return the list tools
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tool> GetItems()
        {
            return _items.SelectMany(c => c);
        }

        public void ApplyChange(ToolbarList d)
        {
            this.Name = d.Name;
            this.Clear();
            Add(d.ToArray());
        }

        internal void Select(ToolbarGroup toolbarGroup)
        {
            this._selected = toolbarGroup;
        }

        internal bool IsSelected(ToolbarGroup toolbarGroup)
        {
            return this._selected == toolbarGroup;
        }

        public Guid Id { get; }

        public string Name { get; private set; }

        public Microsoft.AspNetCore.Components.ComponentBase Component { get; internal set; }

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        private readonly List<ToolbarGroup> _items;
        private ToolbarGroup _selected;
    }

}
