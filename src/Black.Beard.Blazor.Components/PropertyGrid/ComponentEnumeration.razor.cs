using Bb.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;

namespace Bb.PropertyGrid
{

    public partial class ComponentEnumeration
    {

        public IListProvider? ListResolver { get; private set; }

        public List<ListItem> Items { get; set; }

        protected override Task OnInitializedAsync()
        {

            if (this.Property.ListProvider != null && ListResolver == null)
            {
                Items = new List<ListItem>();
                this.ListResolver = (IListProvider)Property.Parent.ServiceProvider.GetService(this.Property.ListProvider);
                this.ListResolver.Property = this.Property.PropertyDescriptor;
                //this.ListResolver.TranslateService = this.Property.Parent.TranslateService;
            }

            if (ListResolver != null)
            {
                Items.Clear();
                Items.AddRange(ListResolver.GetItems());
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


    }

}
