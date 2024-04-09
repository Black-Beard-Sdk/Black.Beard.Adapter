using MudBlazor;

namespace Bb.PropertyGrid
{
    public partial class ComponentFieldTree<T> //: ComponentFieldBase    //, INotifyCollectionChanged
    {


        public HashSet<T> Items
        {
            get
            {

                return null;          ;

            }
        }


        //public void Apply()
        //{
        //    if (Property != null)
        //    {
        //        Property.Value = _value;
        //    }
        //}


        //public void Reset()
        //{

        //    if (Property != null)
        //    {

        //        try
        //        {

        //            _value = Property.Value;

        //            if (_value == null)
        //            {
        //                _value = Activator.CreateInstance(Property.Type);
        //                Property.Value = _value;
        //            }

        //            this.Descriptor = new ObjectDescriptor(null, this.Property.SubType, this.Property.Parent.TranslateService);
        //            this.Descriptor.Analyze();
        //        }
        //        catch (Exception ex)
        //        {

        //        }

        //    }

        //}

        //public IEnumerable<PropertyObjectDescriptor> Headings()
        //{

        //    if (this.Descriptor == null)
        //        Reset();

        //    foreach (PropertyObjectDescriptor item in this.Descriptor.Items)
        //        yield return item;

        //}

        //public IEnumerable<object> Rows
        //{
        //    get
        //    {
        //        if (this.Descriptor == null)
        //            Reset();

        //        var items = Property.Value as IEnumerable;

        //        if (items != null)
        //            foreach (object item in items)
        //            {
        //                this.Descriptor.Instance = item;
        //                yield return item;
        //            }
        //    }
        //}


        //public void Add()
        //{
        //    var newItem = Activator.CreateInstance(Property.SubType);
        //    var method = this.Property.Type.GetMethod("Add");
        //    method.Invoke(_value, new object[] { newItem });

        //    if (this.CollectionChanged != null)
        //        this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem));

        //    PropertyChange();

        //}

        //public void Del(object item)
        //{
        //    CurrentItem = item;
        //}

        //public void Edit(object item)
        //{
        //    CurrentItem = item;
        //    StateHasChanged();
        //}

        //public object CurrentItem { get; set; }

        //public ObjectDescriptor Descriptor { get; set; }

        //private object _value;

        //public event NotifyCollectionChangedEventHandler? CollectionChanged;
    }


}
