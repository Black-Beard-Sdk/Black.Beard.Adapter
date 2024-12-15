using Bb.ComponentModel.Translations;
using Bb.TypeDescriptors;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

namespace Bb.ComponentDescriptors
{


    public class ObjectDescriptor : Descriptor
    {

        public ObjectDescriptor
        (
            object instance,
            Type type,
            ITranslateHost translateService,
            IServiceProvider serviceProvider,
            string strategyName,
            Func<PropertyDescriptor, bool> propertyDescriptorFilter = null,
            Func<PropertyObjectDescriptor, bool> propertyFilter = null
        )
            : base(serviceProvider, translateService, strategyName, type, propertyDescriptorFilter, propertyFilter)
        {
            Value = instance;
            _invaLidItems = new List<PropertyObjectDescriptor>();
            Analyze();
        }

        protected override void Analyze()
        {
            if (Type != null)
            {

                base.Analyze();

                if (Type.IsValueType || Type == typeof(string))
                    Trace.WriteLine($"the list of string or value types are not managed. Please use Mapper<{Type.Name}>");

                else if (IsEnumerable && Value is IEnumerable e)
                    AnalyseEnumerable(e);

                else
                    AnalyzeProperties();

            }


        }

        private void AnalyseEnumerable(IEnumerable e)
        {
            foreach (var item in e)
            {

                var descriptor = this.CreateSub(item);

                descriptor.Enabled = true;
                Add(descriptor);

            }
        }

        private void AnalyzeProperties()
        {
            var properties = TypeDescriptor.GetProperties(Value);
            foreach (PropertyDescriptor property in properties)
                if (PropertyDescriptorFilter(property) && property.IsBrowsable)
                {

                    var p = new PropertyObjectDescriptor(property, this, StrategyName, PropertyDescriptorFilter, PropertyFilter);
                    if (p.Browsable)
                    {
                        p.Enabled = PropertyFilter == null || PropertyFilter(p);

                        if (p.IsValid)
                            Add(p);
                        else
                            _invaLidItems.Add(p);
                    }
                }
        }

        protected override void AssignStrategy(StrategyEditor strategy)
        {

            var i = strategy.AttributeInitializers;
            if (i != null)
            {

                var attributes = TypeDescriptor.GetAttributes(Value).OfType<Attribute>().ToList();
                foreach (Attribute attribute in attributes)
                    if (i.TryGetValue(attribute.GetType(), out var a))
                        a(attribute, _strategy, this);
            }

            base.AssignStrategy(strategy);


        }


        public IEnumerable<TranslatedKeyLabel> Categories()
        {

            var result = Items
                            .OfType<PropertyObjectDescriptor>()
                            .Where(c => c.Browsable)
                            .Select(x => x.Category).ToList();

            var h = new HashSet<string>();
            foreach (var item in result)
                if (h.Add(item.ToString()))
                    yield return item;

        }


        public IEnumerable<PropertyObjectDescriptor> ItemsByCategories(TranslatedKeyLabel category)
        {

            var c = category.ToString();

            var result = Items
                            .OfType<PropertyObjectDescriptor>()
                            .Where(c => c.Browsable)
                            .Where(x => x.Category.ToString() == c)
                            // .OrderBy(c => c.Display.ToString())
                            ;

            foreach (var item in result)
                yield return item;

        }


        public IEnumerable<PropertyObjectDescriptor> InvalidItems { get => _invaLidItems; }


        private readonly List<PropertyObjectDescriptor> _invaLidItems;

    }


}