using Bb.ComponentDescriptors;
using Bb.ComponentModel.Factories;
using Bb.ComponentModel.Translations;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using System.Collections;
using System.Diagnostics.Metrics;
using static MudBlazor.CategoryTypes;
using static MudBlazor.Colors;

namespace Bb.PropertyGrid
{

    public partial class ComponentFieldBaseList : ComponentFieldBase
    {

        protected void PropertyHasChanged(PropertyObjectDescriptor obj)
        {
            StateHasChanged();
        }

        public IEnumerable<ComponentFieldListItem> Rows
        {
            get
            {

                this._currentItem = null;

                List<ComponentFieldListItem> _items = new List<ComponentFieldListItem>();
                var items = Descriptor?.Value as IEnumerable;
                if (items != null && this.Descriptor != null)
                {

                    int cnt = 0;
                    if (_keyDefaultValue == null)
                        _keyDefaultValue = new TranslatedKeyLabel($"No name")
                            .Translate(this.TranslateService);

                    foreach (object item in items)
                    {
                        var value = GetViewModel(cnt++, item);
                        if (value != null)
                            _items.Add(value);
                        else
                        {

                        }
                    }

                }


                if (this._currentItem == null)
                    this._currentItemKey = null;


                if (_items.Count > 0 && this._currentItemKey == null)
                {
                    this._currentItem = _items[0];
                    this._currentItemKey = this._currentItem;
                    this._currentItem.IsCurrent = true;
                }

                return _items;

            }
        }

        private object GetKey(object value)
        {
            var items = Descriptor?.Value as IEnumerable;
            var key = this.Descriptor.ListAccessor.GetKey(items, value);
            return key;
        }

        private ComponentFieldListItem GetViewModel(int cnt, object item)
        {

            ComponentFieldListItem value;
            var subDescriptor = this.Descriptor.CreateSub(item);

            Func<object, string> name;
            name = c => subDescriptor.GetValueLabel(c, $"{_keyDefaultValue} {cnt}");
            value = new ComponentFieldListItem((SubObjectDescriptor)subDescriptor, name)
            {
                PropertyGridView = this.ParentGrid
            };

            if (this._currentItemKey != null && value.Key.Equals(_currentItemKey))
            {
                this._currentItem = value;
                value.IsCurrent = true;
            }

            return value;

        }

        public bool CanAdd => Descriptor?.ListAccessor?.CanAdd ?? false && CanCreate();

        public bool CanDel => Descriptor?.ListAccessor?.CanDel ?? false; 

        public async void Add()
        {

            object newItem = CreateNewItem();
            if (newItem != null)
            {

                using (var transaction = GetTransaction($"Add {Descriptor.SubType.Name} "))
                {

                    var key = Descriptor.ListAccessor.GetKey(Descriptor.Value, newItem);
                    Descriptor.ListAccessor.Add(Descriptor.Value, key, newItem);
                    PropertyChange();
                }

                StateHasChanged();

            }
            else
            {

            }

        }

        public async void Del(ComponentFieldListItem item)
        {
            _currentItem = item;
            StateHasChanged();
            bool? result = await mbox.ShowAsync();
        }

        internal void OnClick(MouseEventArgs args, ComponentFieldListItem current)
        {
            if (ChangeCurrent(current))
            {
                PropertyChange();
                StateHasChanged();
            }
        }

        protected async void Remove()
        {

            using (var transaction = GetTransaction($"Remove {Descriptor.SubType.Name} {_currentItem.Label}"))
            {
                var key = Descriptor.ListAccessor.GetKey(Descriptor.Value, _currentItem.Value);
                Descriptor.ListAccessor.Del(Descriptor.Value, key);
                PropertyChange();
            }

            StateHasChanged();
        }


        private bool CanCreate()
        {

            if (_canCreate.HasValue)
                return _canCreate.Value;

            try
            {
                _canCreate = CreateNewItem() != null;
            }
            catch (Exception)
            {
                _canCreate = false;
            }

            return _canCreate.Value;

        }

        private bool? _canCreate;

        private object? CreateNewItem()
        {

            object newItem;
            var type = Descriptor.SubType;
            if (PropertyObjectDescriptor.Create(this.StrategyName, type, this.Descriptor?.ServiceProvider, out newItem))
            {
                return newItem;

            }

            return null;

        }

        private bool ChangeCurrent(ComponentFieldListItem current)
        {

            bool isChanged = false;
            if (current != null && current.Key != _currentItemKey)
            {
                _currentItem = current;
                _currentItemKey = current.Key;
                isChanged = true;
            }
            return isChanged;

        }

        protected MudMessageBox mbox { get; set; }

        private bool One(IEnumerable objects)
        {
            int count = 0;
            foreach (var item in objects)
            {
                count++;
                if (count > 1)
                    return false;
            }
            return false;
        }

        private string _keyDefaultValue;
        private Dictionary<object, ComponentFieldListItem> _dic = new Dictionary<object, ComponentFieldListItem>();
        private object _currentItemKey;
        private ComponentFieldListItem _currentItem;

    }


}
