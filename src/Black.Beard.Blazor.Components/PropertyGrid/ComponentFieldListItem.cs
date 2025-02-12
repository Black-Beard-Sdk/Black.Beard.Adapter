using Bb.ComponentDescriptors;
using System.ComponentModel;
using static MudBlazor.CategoryTypes;

namespace Bb.PropertyGrid
{

    public class ComponentFieldListItem
    {

        /// <summary>
        /// Initialize a new instance of ComponentFieldListItem
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="name"></param>
        /// <param name="instance"></param>
        public ComponentFieldListItem(
            SubObjectDescriptor descriptor,
            Func<object, string> name)
        {
            IsCurrent = false;
            this.Descriptor = descriptor;
            this._functionNabel = name;
            this.PropertyGridView = null;
        }

        [Browsable(false)]
        public bool IsStapleType => Descriptor?.IsStapleType ?? false;

        /// <summary>
        /// Return true if the item is the current item selected
        /// </summary>
        [Browsable(false)]
        public bool IsCurrent { get; set; }

        [Browsable(false)]
        public SubObjectDescriptor Descriptor { get; }

        [Browsable(false)]
        public object Key => Descriptor.GetKey();

        [Browsable(false)]
        public string Label => _functionNabel(Value);

        [PropertyDescriptorTypeResolver<ResolveTypeFromValue>]
        [Browsable(true)]
        public object Value
        {
            get
            {
                return this.Descriptor.Value;
            }
            set
            {
                this.Descriptor.Value = value;
            }
        }

        [Browsable(false)]
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
        private readonly Func<object, string> _functionNabel;
        private readonly object _key;
    }


}
