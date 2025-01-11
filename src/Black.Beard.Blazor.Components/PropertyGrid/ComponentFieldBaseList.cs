using Bb.ComponentDescriptors;
using Bb.ComponentModel.Factories;
using Bb.ComponentModel.Translations;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using System.Collections;

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
                var items = Descriptor?.Value as IEnumerable;
                if (items != null && this.Descriptor != null)
                {
                    int cnt = 0;
                    if (_keyDefaultValue == null)
                        _keyDefaultValue = new TranslatedKeyLabel($"No name")
                            .Translate(this.TranslateService);

                    int isSelected = 0;
                    foreach (object item in items)
                    {
                        cnt++;
                        var key = this.Descriptor.GetValueKey(item);
                        if (!_dic.TryGetValue(key, out ComponentFieldListItem? value))
                            _dic.Add(key, value = GetViewModel(cnt, item));
                        if (value.IsCurrent)
                            isSelected++;
                    }

                    if (isSelected != 1)
                        ChangeCurrent(_dic.FirstOrDefault().Value);

                }

                return _dic.Values;

            }
        }

        public async void Add()
        {

            object newItem;

            if (PropertyObjectDescriptor.Create(this.StrategyName, Property.SubType, this.Descriptor?.ServiceProvider, out newItem))
            {

            }
            else if (Property.SubType.IsClass && Property.SubType.GetConstructor([]) != null)
            {
                newItem = Activator.CreateInstance(Property.SubType);
                if (newItem != null && newItem is IInitialize i)
                    i.Initialize(this.Descriptor?.ServiceProvider);

            }

            if (newItem != null)
            {

                var value1 = GetViewModel(_dic.Count, newItem);

                using (var transaction = GetTransaction($"Add {Property.SubType.Name} {value1.Label}"))
                {
                    var value = Descriptor.Value;
                    var method = this.Descriptor.Type.GetMethod("Add");
                    method.Invoke(value, new object[] { newItem });
                    ChangeCurrent(value1);
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
            using (var transaction = GetTransaction($"Remove {Property.SubType.Name} {_currentItem.Label}"))
            {
                var value = Descriptor.Value;
                var method = this.Descriptor.Type.GetMethod("Remove");
                method.Invoke(value, new object[] { _currentItem.Instance });
                Property?.PropertyChange();
                PropertyChange();
            }

            StateHasChanged();
        }


        private bool ChangeCurrent(ComponentFieldListItem current)
        {
            bool isChanged = false;
            foreach (var item in _dic)
            {
                var i = item.Value;
                var p = i.IsCurrent;
                i.IsCurrent = i.Instance == current.Instance;
                if (p != i.IsCurrent)
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

        private ComponentFieldListItem GetViewModel(int cnt, object item)
        {
            ComponentFieldListItem value;
            Descriptor subDescriptor = this.Descriptor.CreateSub(item);
            value = new ComponentFieldListItem(subDescriptor
                , c => subDescriptor.GetValueLabel(c, $"{_keyDefaultValue} {cnt}")
                , item);
            return value;
        }

        private string _keyDefaultValue;
        private Dictionary<object, ComponentFieldListItem> _dic = new Dictionary<object, ComponentFieldListItem>();
        private ComponentFieldListItem _currentItem;

    }


}
