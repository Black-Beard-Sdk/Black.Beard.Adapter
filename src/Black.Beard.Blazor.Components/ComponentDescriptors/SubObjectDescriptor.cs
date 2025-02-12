using Bb.PropertyGrid;
using System.Collections;
using System.ComponentModel;
using static MudBlazor.CategoryTypes;
using static MudBlazor.Colors;

namespace Bb.ComponentDescriptors
{
    public class SubObjectDescriptor : Descriptor
    {

        public SubObjectDescriptor
        (
            object instance,
            Type type,
            Descriptor parent
        )
            : base(parent.ServiceProvider, parent, parent.StrategyName, type, parent.PropertyDescriptorFilter, parent.PropertyFilter)
        {
            this.Parent = parent;
            _value = instance;
            _key = GetKey();
            Analyze();
            if (!this.IsStapleType)
            {
                ComponentView = typeof(ComponentGrid);
                KindView = PropertyKindView.Object;
            }
        }


        public object GetKey()
        {
            var items = Parent.Value as IEnumerable;
            var key = Parent.ListAccessor.GetKey(items, _value);
            return key;
        }


        public override object Value 
        { 
            get 
            {
                return Parent.ListAccessor.Get(Parent.Value, _key);
            }
            set
            {
                if (_value != value)
                {
                    Parent.ListAccessor.Set(Parent.Value, _key, value);
                    _value = value;
                    _key = GetKey();

                }
            }
        }

        private object _value;
        private object _key;

    }


}