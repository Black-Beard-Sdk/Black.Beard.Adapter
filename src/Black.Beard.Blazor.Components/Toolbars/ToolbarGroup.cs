using Bb.ComponentModel.Translations;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Linq;
using static MudBlazor.CategoryTypes;

namespace Bb.Toolbars
{

    public class ToolbarGroup : ICollection<Tool>, INotifyCollectionChanged
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolbarGroup"/> class.
        /// </summary>
        public ToolbarGroup(Guid id, TranslatedKeyLabel name) 
            : this(id, name, new List<Tool>() { })
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolbarGroup"/> class.
        /// </summary>
        /// <param name="items"></param>
        public ToolbarGroup(Guid id, TranslatedKeyLabel name, params Tool[] items) 
            : this(id, name, items.ToList())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolbarGroup"/> class.
        /// </summary>
        /// <param name="items"></param>
        public ToolbarGroup(Guid id, TranslatedKeyLabel name, IEnumerable<Tool> items)
        {
            this.Id = id;
            this.Name = name;
            _items = new List<Tool>(items);
        }


        public void Select()
        {

            if (Parent != null)
                Parent.Select(this);

        }

        public bool IsSelected => Parent?.IsSelected(this) ?? false; 

        public ToolbarList? Parent { get; private set; }

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


        public int Count => _items.Count;

        public bool IsReadOnly => false;

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

        }

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
            }
        }

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
                    }
                    return true;
                }

            return false;

        }

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

            if (_removed.Count > 0 && CollectionChanged != null)
            {
                var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, _removed);
                CollectionChanged.Invoke(this, e);
            }

        }

        public void Clear()
        {
            _items.Clear();
            if (CollectionChanged != null)
            {
                var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, new List<ToolbarGroup>() { });
                CollectionChanged.Invoke(this, e);
            }
        }

        public bool Contains(Tool item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(Tool[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Tool> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public Guid Id { get; }
        public TranslatedKeyLabel Name { get; }

        private readonly List<Tool> _items;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

    }

    //    public class ToolbarList
    //    {

    //        public ToolbarList(IEnumerable<ToolItemBase> items)
    //        {

    //            if (items != null)
    //            {

    //                var i = items.ToLookup(i => i.Category).ToList();
    //                Items = new List<ToolboxCategory>(i.Count());
    //                foreach (var item in i)
    //                {
    //                    var display = item.Key.DefaultDisplay;
    //                    var category = new ToolbarCategory(this, display, item);
    //                    Items.Add(category);
    //                }

    //                var links = items.OfType<DiagramToolRelationshipBase>().ToList();
    //                if (links.Count == 1)
    //                    CurrentLink = links[0];
    //                else
    //                    CurrentLink = links.FirstOrDefault(c => c.IsDefaultLink);

    //            }

    //        }

    //        public List<ToolboxCategory> Items { get; }

    //        public string Current { get; internal set; }

    //        public DiagramToolRelationshipBase CurrentLink { get; set; }


    //        public void EnsureCategoryIsShown(DiagramToolBase item)
    //        {

    //            foreach (var category in Items)
    //                if (category.Items.Contains(item))
    //                {
    //                    category.Toggle();
    //                    return;
    //                }

    //        }


    //    }

    //    public class ToolbarCategory
    //    {

    //        public ToolbarCategory(ToolbarList parent, string name, IEnumerable<ToolItemBase> items)
    //        {
    //            _parent = parent;
    //            Name = name;
    //            Items = new List<ToolItemBase>(items);
    //        }


    //        public bool IsExpanded
    //        {
    //            get => _parent.Current == this.Name;
    //        }


    //        public string Name { get; set; }

    //        public List<ToolItemBase> Items { get; }

    //        //public void Toggle()
    //        //{
    //        //    _parent.Current = this.Name;
    //        //}

    //        private readonly ToolboxList _parent;

    //    }


}
