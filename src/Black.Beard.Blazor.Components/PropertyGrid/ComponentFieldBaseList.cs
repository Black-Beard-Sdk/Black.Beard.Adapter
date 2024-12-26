using Bb.ComponentDescriptors;
using Bb.ComponentModel.Translations;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using System.Collections;
using static MudBlazor.CategoryTypes;
using static MudBlazor.Components.Chart.Models.TimeSeriesChartSeries;

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
                        _keyDefaultValue = new TranslatedKeyLabel($"No name").Translate(this.TranslateService);

                    int isSelected = 0;
                    foreach (object item in items)
                    {
                        cnt++;
                        if (!_dic.TryGetValue(item, out ComponentFieldListItem value))
                            _dic.Add(item, value = GetViewModel(cnt, item));
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

            if (PropertyObjectDescriptor.Create(this.StrategyName, Property.SubType, out newItem))
            {

            }
            else if (Property.SubType.IsClass && Property.SubType.GetConstructor(new Type[0]) != null)
                newItem = Activator.CreateInstance(Property.SubType);

            if (newItem != null)
            {
                var value = Descriptor.Value;
                var method = this.Descriptor.Type.GetMethod("Add");
                method.Invoke(value, new object[] { newItem });
                var value1 = GetViewModel(0, newItem);

                using (var transaction = GetTransaction($"Add {Property.SubType.Name} {value1.Label}"))
                {
                    _dic.Add(newItem, value1);
                    ChangeCurrent(value1);
                    PropertyChange();
                }
                
                StateHasChanged();

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
            var value = Descriptor.Value;
            var method = this.Descriptor.Type.GetMethod("Remove");
            method.Invoke(value, new object[] { _currentItem.Instance });

            using (var transaction = GetTransaction($"Add {Property.SubType.Name} {_currentItem.Label}"))
            {
                _dic.Remove(_currentItem.Instance);
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
            value = new ComponentFieldListItem(subDescriptor, c => subDescriptor.GetValueLabel(c, $"{_keyDefaultValue} {cnt}"), item);
            return value;
        }

        private string _keyDefaultValue;
        private Dictionary<object, ComponentFieldListItem> _dic = new Dictionary<object, ComponentFieldListItem>();
        private ComponentFieldListItem _currentItem;

    }

    public class ComponentFieldListItem
    {

        public ComponentFieldListItem(Descriptor descriptor, Func<object, string> name, object instance)
        {
            IsCurrent = false;
            this.Descriptor = descriptor;
            this._label = name;
            this.Instance = instance;
            this.PropertyGridView = null;
        }

        public bool IsCurrent { get; set; }

        public Descriptor Descriptor { get; }


        public string Label => _label(Instance);

        public object Instance { get; }

        public PropertyGridView PropertyGridView
        {
            get => _PropertyGridView;
            set
            {
                _PropertyGridView = value;
                this.Descriptor.SetUI(_PropertyGridView);
                if (_PropertyGridView != null && this.Descriptor.Parent.Ui is PropertyGridView v)
                    _PropertyGridView.BuildDynamicParameter(v);
            }
        }

        private PropertyGridView _PropertyGridView;
        private readonly Func<object, string> _label;

    }


}
