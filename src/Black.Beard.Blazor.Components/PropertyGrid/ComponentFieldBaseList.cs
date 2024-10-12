using Bb.ComponentDescriptors;
using MudBlazor;
using System.Collections;

namespace Bb.PropertyGrid
{

    public partial class ComponentFieldBaseList : ComponentFieldBase
    {

        protected override void PropertyChange()
        {

            if (Property != null)
            {

                try
                {

                    var _value = Property.Value;

                    if (_value == null)
                        Property.Value = _value = Activator.CreateInstance(Property.Type);

                    this.Descriptor = new ObjectDescriptor(null, this.Property.SubType, this.Property.Parent.TranslateService, this.Property.Parent.ServiceProvider, this.StrategyName, null, null);

                }
                catch (Exception)
                {

                }

            }

        }

        protected void PropertyHasChanged(PropertyObjectDescriptor obj)
        {
            StateHasChanged();
        }

        public IEnumerable<PropertyObjectDescriptor> Headings()
        {

            if (this.Descriptor != null)
                foreach (PropertyObjectDescriptor item in this.Descriptor.Items)
                    yield return item;

        }

        public IEnumerable<object> Rows
        {
            get
            {

                var items = Property.Value as IEnumerable;

                if (items != null && this.Descriptor != null)
                    foreach (object item in items)
                    {
                        this.Descriptor.Instance = item;
                        yield return item;
                    }
            }
        }


        public async void Add()
        {

            object newItem;

            if (PropertyObjectDescriptor.Create(this.StrategyName, Property.SubType, out newItem))
            {

            }
            else if (Property.SubType.IsClass)
                newItem = Activator.CreateInstance(Property.SubType);

            if (newItem != null)
            {
                var value = Property.Value;
                var method = this.Property.Type.GetMethod("Add");
                method.Invoke(value, new object[] { newItem });

            }

            CurrentItem = newItem;
            PropertyChange();
            StateHasChanged();

        }

        public async void Del(object item)
        {
            CurrentItem = item;
            StateHasChanged();
            bool? result = await mbox.Show();
        }

        protected async void Remove()
        {
            var value = Property.Value;
            var method = this.Property.Type.GetMethod("Remove");
            method.Invoke(value, new object[] { CurrentItem });
            CurrentItem = new object();
            Property.PropertyChange();
            PropertyChange();
            StateHasChanged();
        }

        public async void Edit(object item)
        {
            CurrentItem = item;
            StateHasChanged();
        }

        public object CurrentItem { get; set; }

        public ObjectDescriptor Descriptor { get; set; }

        protected MudMessageBox mbox { get; set; }

    }


}
