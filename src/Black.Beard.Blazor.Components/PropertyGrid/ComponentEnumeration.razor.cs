using Bb.ComponentModel.DataAnnotations;
using Bb.ComponentModel.Factories;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Collections.Generic;
using static MudBlazor.CategoryTypes;

namespace Bb.PropertyGrid
{

    public partial class ComponentEnumeration
    {

        public ComponentEnumeration()
        {

        }

        public IListProvider? ListResolver { get; private set; }

        public List<Option<string>> Items { get; set; }


        public string SelecteItem
        {
            get
            {
                var value = Property.Value;
                return _items.FirstOrDefault(c => c.Selected)?.Display;
            }
            set
            {
                var p = _items.FirstOrDefault(c => c.Display == value);
                Property.Value = p.GetOriginalValue();
            }
        }


        protected override Task OnInitializedAsync()
        {

            if (this.Property.ListProvider != null && ListResolver == null)
            {

                Items = new List<Option<string>>();
                this.ListResolver = (IListProvider)Property.Parent.ServiceProvider.GetService(this.Property.ListProvider);

                if (this.ListResolver == null)
                {
                    var factory2 = ObjectCreatorByIoc.GetActivator<IListProvider>(this.Property.ListProvider);
                    this.ListResolver = factory2.Call(string.Empty, Property.Parent.ServiceProvider);
                }

                this.ListResolver.Property = this.Property.PropertyDescriptor;
                this.ListResolver.Instance = this.Property.Parent.Instance;

            }

            if (ListResolver != null)
            {
                var v = this.Property.Value.ToString();
                Items.Clear();
                _items = ListResolver.GetItems().ToList();
                foreach (var item in _items)
                    Items.Add(new Option<string>()
                    {
                        Text = item.Display,                        
                        Value = item.Name,
                        Selected = item.Selected,
                    });

            }

            return base.OnInitializedAsync();

        }

        public override object Load()
        {
            var v = Property.Value;
            if (v != null)
            {
                foreach (var item in this.Items)
                {
                    if (item.Value.GetHashCode() == v.GetHashCode())
                        return item;
                }
            }
            return default(ListItem);
        }

        public override object Save(object item)
        {

            if (item is ListItem i)
                return i.Value;

            return null;

        }

        private List<ListItem> _items;

    }

}
