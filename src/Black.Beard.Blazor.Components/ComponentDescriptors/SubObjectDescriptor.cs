using Bb.PropertyGrid;
using System.ComponentModel;

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
            Value = instance;
            Analyze();
            ComponentView = typeof(ComponentGrid);
            KindView = PropertyKindView.Object;
        }

    }


}