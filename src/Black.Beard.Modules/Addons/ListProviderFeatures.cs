using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.DataAnnotations;
using Bb.Modules;

namespace Bb.Addons
{

    [ExposeClass(UIConstants.Service, ExposedType = typeof(ListProviderFeatures), LifeCycle = IocScopeEnum.Transiant)]
    public class ListProviderFeatures : ProviderListBase<Feature>
    {

        public ListProviderFeatures(AddonFeatures FeatureSpecifications)
        {
            _featureSpecifications = FeatureSpecifications;
        }

        public override IEnumerable<ListItem<Feature>> GetItems()
        {


            List<ListItem<Feature>> list = new List<ListItem<Feature>>();

            if (Instance != null && Property != null)
            {

                IEnumerable<Feature> items = _featureSpecifications.GetFeatures();
                foreach (var item in items)
                    list.Add(CreateItem(item, item.Name, item.Uuid, c =>
                    {

                    }));
            }

            return list;

        }

        protected override object ResolveOriginalValue(ListItem<Feature> item)
        {
            return item.Value;
        }


        private readonly AddonFeatures _featureSpecifications;

    }


}
