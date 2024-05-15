using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.DataAnnotations;
using Bb.Modules.Sgbd.Models;
using System.Collections;
using System.ComponentModel;

namespace Bb.Modules
{

    [ExposeClass(UIConstants.Service, ExposedType = typeof(ListProviderTechnologies), LifeCycle = IocScopeEnum.Transiant)]
    public class ListProviderTechnologies : ProviderListBase<SgbdTechnology>
    {

        public ListProviderTechnologies(SgbdTechnologies provider)
        {
            _provider = provider;
        }

        public override IEnumerable<ListItem<SgbdTechnology>> GetItems()
        {

            List<ListItem<SgbdTechnology>> list = new List<ListItem<SgbdTechnology>>();

            if (Instance != null && Property != null)
            {

                var items = _provider.Items;
                foreach (var item in items)
                    list.Add(CreateItem(item, item.Description, item.Name, c =>
                    {

                    }));
            }

            return list;

        }

        protected override object ResolveOriginalValue(ListItem<SgbdTechnology> item)
        {
            return item.Value;
        }

        private readonly SgbdTechnologies _provider;

    }



}
