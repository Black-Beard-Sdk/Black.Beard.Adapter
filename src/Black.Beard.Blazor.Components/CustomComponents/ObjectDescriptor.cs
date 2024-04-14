using Bb.ComponentModel.Translations;
using Bb.PropertyGrid;
using System.ComponentModel;
using System.Diagnostics;

namespace Bb.CustomComponents
{

    public class ObjectDescriptor
    {



        public ObjectDescriptor(object? instance,
            Type? type,
            ITranslateService translateService,
            IServiceProvider serviceProvider,
            string StrategyName,
            Func<PropertyDescriptor, bool> propertyDescriptorFilter = null,
            Func<PropertyObjectDescriptor, bool> propertyFilter = null)
        {
            this.StrategyName = StrategyName;
            this.Instance = instance;
            this._type = type;
            this.TranslateService = translateService;
            this.ServiceProvider = serviceProvider;
            this._items = new List<PropertyObjectDescriptor>();
            this._invaLidItems = new List<PropertyObjectDescriptor>();


            if (propertyDescriptorFilter != null)
                this.PropertyDescriptorFilter = propertyDescriptorFilter;
            else
                this.PropertyDescriptorFilter = (p) => true;

            if (propertyFilter != null)
                this.PropertyFilter = propertyFilter;
            else
                this.PropertyFilter = (p) => true;

            if (this._type != null)
                this.Analyze();

        }


        public Func<PropertyDescriptor, bool> PropertyDescriptorFilter { get; }

        public Func<PropertyObjectDescriptor, bool> PropertyFilter { get; }

        private void Analyze()
        {
            if (this._type != null)
            {
                if (this._type.IsValueType || this._type == typeof(string))
                    Trace.WriteLine($"the list of string or value types are not managed. Please use Mapper<{this._type.Name}>");

                else
                {

                    var properties = TypeDescriptor.GetProperties(this._type);
                    foreach (PropertyDescriptor property in properties)
                        if (PropertyDescriptorFilter(property))
                        {
                            var p = new PropertyObjectDescriptor(property, this, this.StrategyName)
                                .Build();
                            p.Enabled = (PropertyFilter == null || PropertyFilter(p));
                            if (p.IsValid)
                                _items.Add(p);
                            else
                                _invaLidItems.Add(p);
                        }

                }
            }
        }

        public ITranslateService TranslateService { get; }

        public IServiceProvider ServiceProvider { get; }
        public string StrategyName { get; }
        public object? Instance { get; set; }

        public IEnumerable<TranslatedKeyLabel> Categories()
        {

            var result = _items
                .Where(c => c.Browsable)
                .Select(x => x.Category).ToList();

            var h = new HashSet<string>();
            foreach (var item in result)
                if (h.Add(item.ToString()))
                    yield return item;

        }

        internal void ValidationHasChanged<T>(ComponentFieldBase<T> componentFieldBase)
        {

            if (UiPropertyValidationHasChanged != null)
                UiPropertyValidationHasChanged(componentFieldBase);

            if (PropertyValidationHasChanged != null)
                PropertyValidationHasChanged(componentFieldBase.Property);

        }

        public Action<ComponentFieldBase> UiPropertyValidationHasChanged { get; set; }

        public Action<PropertyObjectDescriptor> PropertyValidationHasChanged { get; set; }

        public IEnumerable<PropertyObjectDescriptor> ItemsByCategories(TranslatedKeyLabel category)
        {

            var c = category.ToString();

            var result = _items
                .Where(c => c.Browsable)
                .Where(x => x.Category.ToString() == c)
                // .OrderBy(c => c.Display.ToString())
                ;

            foreach (var item in result)
                yield return item;

        }


        public IEnumerable<PropertyObjectDescriptor> Items { get => _items; }

        public IEnumerable<PropertyObjectDescriptor> InvalidItems { get => _invaLidItems; }

        private readonly Type? _type;
        private readonly List<PropertyObjectDescriptor> _items;
        private readonly List<PropertyObjectDescriptor> _invaLidItems;


        internal void HasChanged(PropertyObjectDescriptor propertyObjectDescriptor)
        {
            if (this.PropertyHasChanged != null)
                PropertyHasChanged(propertyObjectDescriptor);
        }

        public DiagnosticValidator Validate()
        {

            DiagnosticValidator validator = new DiagnosticValidator();

            foreach (var item in _items)
                if (item.Enabled && !item.Validate(out var result))
                    validator.Add(result);

            return validator;

        }

        public Action<PropertyObjectDescriptor> PropertyHasChanged { get; set; }

    }



}