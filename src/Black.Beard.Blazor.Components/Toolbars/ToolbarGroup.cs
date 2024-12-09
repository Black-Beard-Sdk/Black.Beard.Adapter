using Bb.ComponentModel.Translations;
using Bb.UIComponents;
using System.Collections;
using System.Collections.Specialized;

namespace Bb.Toolbars
{


    public class ToolbarGroup : ICollection<Tool>, INotifyCollectionChanged
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolbarGroup"/> class.
        /// </summary>
        public ToolbarGroup(Guid? id, TranslatedKeyLabel name)
            : this(id, name, new List<Tool>() { })
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolbarGroup"/> class.
        /// </summary>
        /// <param name="items"></param>
        public ToolbarGroup(Guid? id, TranslatedKeyLabel name, params Tool[] items)
            : this(id, name, items.ToList())
        {

        }

        public ToolbarGroup(Guid? id, TranslatedKeyLabel name, params ExtendedTool[] items)
            : this(id, name, items.ToList())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolbarGroup"/> class.
        /// </summary>
        /// <param name="items"></param>
        public ToolbarGroup(Guid? id, TranslatedKeyLabel name, IEnumerable<Tool> items)
        {
            this.Id = id.HasValue ? id.Value : Guid.NewGuid();
            this.Name = name ?? throw new NullReferenceException(nameof(name));
            _items = new List<Tool>(items ?? new Tool[] { });
            this.Show = _items.Any(c => c.Show);
        }

        public ToolbarGroup(Guid? id, TranslatedKeyLabel name, IEnumerable<ExtendedTool> items)
        {
            this.Id = id.HasValue ? id.Value : Guid.NewGuid();
            this.Name = name ?? throw new NullReferenceException(nameof(name));
            _items = new List<Tool>(items ?? new ExtendedTool[] { });
            this.Show = _items.Any(c => c.Show);
        }


        /// <summary>
        /// Select the group
        /// </summary>
        public void Select()
        {

            if (Parent != null)
                Parent.Select(this);

        }

        /// <summary>
        /// Return true if the group is selected
        /// </summary>
        public bool IsSelected => Parent?.IsSelected(this) ?? false;

        /// <summary>
        /// Return the <see cref="ToolbarList"/> parent
        /// </summary>
        public ToolbarList? Parent { get; private set; }

        /// <summary>
        /// Set the parent
        /// </summary>
        /// <param name="parent"></param>
        public void SetParent(ToolbarList? parent)
        {

            if (Parent != null)
            {
                CollectionChanged -= Parent.Item_CollectionChanged;
            }

            Parent = parent;

            if (Parent != null)
            {
                CollectionChanged += Parent.Item_CollectionChanged;
            }

        }

        /// <summary>
        /// return the count of items
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// Return true if the collection is readonly
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Add new tools list
        /// </summary>
        /// <param name="items"></param>
        public void Add(params Tool[] items)
        {

            List<Tool> _added = new List<Tool>(items.Length);
            if (items != null)
                foreach (var item in items)
                    if (!_items.Contains(item) && !_items.Any(c => c.Name == item.Name))
                    {
                        item.Group = this;
                        _items.Add(item);
                        _added.Add(item);
                    }

            if (_added.Count > 0 && CollectionChanged != null)
            {
                var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, _added);
                CollectionChanged.Invoke(this, e);
            }

            this.Show = _items.Any(c => c.Show);

        }


        public ToolbarGroup Add(TranslatedKeyLabel name, TranslatedKeyLabel description, Glyph icon, object tag, bool withToggle, bool draggable, bool show)
        {
            var item = new Tool(name, description, icon, tag, withToggle, draggable, show);
            Add(item);
            return this;
        }

        public ToolbarGroup Add(TranslatedKeyLabel name, TranslatedKeyLabel description, Glyph icon, bool withToggle, bool draggable, bool show)
        {
            var item = new Tool(name, description, icon, null, withToggle, draggable, show);
            Add(item);
            return this;
        }

        public ToolbarGroup Add(TranslatedKeyLabel name, TranslatedKeyLabel description, Glyph icon)
        {
            var item = new Tool(name, description, icon, null, false, false, true);
            Add(item);
            return this;
        }

        public ToolbarGroup Add(TranslatedKeyLabel name, TranslatedKeyLabel description, Glyph icon, Action<ExtendedTool, object> command)
        {
            var item = new ExtendedTool(name, description, icon, null, true, command);
            Add(item);
            return this;
        }

        public ToolbarGroup Add<T>(TranslatedKeyLabel name, TranslatedKeyLabel description, Glyph icon, Action<ExtendedTool, T> command)
        {
            var item = new ExtendedTool(name, description, icon, null, true, (c, d) => command(c, (T)d));
            Add(item);
            return this;
        }

        /// <summary>
        /// Add new tool
        /// </summary>
        /// <param name="item"></param>
        public void Add(Tool item)
        {
            if (item != null && !_items.Contains(item) && !_items.Any(c => c.Name == item.Name))
            {
                item.Group = this;
                _items.Add(item);
                if (CollectionChanged != null)
                {
                    var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<Tool>() { item });
                    CollectionChanged.Invoke(this, e);
                }

                this.Show = _items.Any(c => c.Show);
            }

        }

        /// <summary>
        /// Remove the tool
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(Tool item)
        {

            if (item != null)
                if (_items.Remove(item))
                {
                    item.Group = null;
                    if (CollectionChanged != null)
                    {
                        var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<Tool>() { item });
                        CollectionChanged.Invoke(this, e);

                        this.Show = _items.Any(c => c.Show);

                    }
                    return true;
                }

            return false;

        }

        /// <summary>
        /// Remove the tool list
        /// </summary>
        /// <param name="items"></param>
        public void Remove(params Tool[] items)
        {

            List<Tool> _removed = new List<Tool>(items.Length);
            if (items != null)
                foreach (var item in items)
                    if (!_items.Contains(item))
                    {
                        item.Group = null;
                        _items.Remove(item);
                        _removed.Add(item);
                    }

            this.Show = _items.Any(c => c.Show);

            if (_removed.Count > 0 && CollectionChanged != null)
            {
                var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, _removed);
                CollectionChanged.Invoke(this, e);
            }

        }

        /// <summary>
        /// Remove all tools
        /// </summary>
        public void Clear()
        {
            _items.Clear();
            if (CollectionChanged != null)
            {
                var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, new List<ToolbarGroup>() { });
                CollectionChanged.Invoke(this, e);
            }

            this.Show = _items.Any(c => c.Show);

        }

        /// <summary>
        /// Return true if the tool is in the collection
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(Tool item)
        {
            return _items.Contains(item);
        }

        /// <summary>
        /// Copy all tool in the array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(Tool[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Tool> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public void Add(string v1, string v2, Glyph save, object manageParcels)
        {
            throw new NotImplementedException();
        }

        public Guid Id { get; }
        public TranslatedKeyLabel Name { get; }

        private readonly List<Tool> _items;

        public bool Show { get; private set; }

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

    }

}
