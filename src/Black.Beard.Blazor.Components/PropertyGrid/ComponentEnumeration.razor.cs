using Bb.ComponentModel.DataAnnotations;
using Bb.ComponentModel.Factories;

namespace Bb.PropertyGrid
{

    public partial class ComponentEnumeration
    {

        public ComponentEnumeration()
        {

        }

        public IListProvider? ListResolver { get; private set; }

        public IEnumerable<ListItem> Items { get; set; }

        protected override Task OnInitializedAsync()
        {

            var property = this.Property;
            var listProvider = property?.ListProvider;
            if (listProvider != null && ListResolver == null)
            {

                this.ListResolver = (IListProvider)Descriptor.ServiceProvider.GetService(listProvider);

                if (this.ListResolver == null)
                {
                    var factory2 = ObjectCreatorByIoc.GetActivator<IListProvider>(listProvider);
                    this.ListResolver = factory2.Call(string.Empty, Descriptor.ServiceProvider);
                }

                this.ListResolver.Property = property.PropertyDescriptor;
                this.ListResolver.Instance = property.Parent.Value;

            }

            if (ListResolver != null)
                Items = ListResolver.GetItems().ToList();

            return base.OnInitializedAsync();

        }

        public override object Load()
        {
            var v = Descriptor.Value;
            if (v != null)
                foreach (ListItem item in this.Items)
                    if (item.Compare(v))
                        return item;
            return default(ListItem);
        }

        public override object Save(object item)
        {
            if (item is ListItem i)
                return i.GetOriginalValue();
            return null;
        }

        private List<ListItem> _items;

    }

}
