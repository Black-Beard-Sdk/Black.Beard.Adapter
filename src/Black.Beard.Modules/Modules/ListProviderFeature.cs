using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.DataAnnotations;

namespace Bb.Modules
{

    [ExposeClass(UIConstants.Service, ExposedType = typeof(ListProviderFeature), LifeCycle = IocScopeEnum.Transiant)]
    public class ListProviderFeature : ProviderListBase<FeatureSpecification>
    {

        public ListProviderFeature(FeatureSpecifications FeatureSpecifications)
        {
            _featureSpecifications = FeatureSpecifications;
        }

        public override IEnumerable<ListItem<FeatureSpecification>> GetItems()
        {


            List<ListItem<FeatureSpecification>> list = new List<ListItem<FeatureSpecification>>();

            if (Instance != null && Property != null)
            {

                IEnumerable<FeatureSpecification> items;

                var o = this.Instance;
                if (o is IModuleInstanceHost h)
                    items= h.Module.ModuleSpecification.GetFeatures();
                else
                    items = _featureSpecifications.GetFeatures();

                foreach (var item in items)
                {
                    var value = item.Uuid;
                    var text = item.Name;
                    list.Add(CreateItem(item, item.Name, item.Uuid, c =>
                    {

                    }));
                }
            }

            return list;

        }

        protected override object ResolveOriginalValue(ListItem<FeatureSpecification> item)
        {
            return item.Value;
        }


        private readonly FeatureSpecifications _featureSpecifications;

    }


}
