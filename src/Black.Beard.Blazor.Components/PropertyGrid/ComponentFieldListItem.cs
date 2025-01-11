using Bb.ComponentDescriptors;

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
        public ComponentFieldListItem(Descriptor descriptor, Func<object, string> name, object instance)
        {
            IsCurrent = false;
            this.Descriptor = descriptor;
            this._label = name;
            this._key = descriptor.GetValueKey(instance);
            this.Instance = instance;
            this.PropertyGridView = null;
        }

        /// <summary>
        /// Return true if the item is the current item selected
        /// </summary>
        public bool IsCurrent { get; set; }


        public Descriptor Descriptor { get; }


        public object Key => _key;

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
        private readonly string _key;
    }


}
