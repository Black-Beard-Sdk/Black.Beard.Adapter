using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.DataAnnotations;
using System.Collections;
using System.ComponentModel;

namespace Bb.Addons
{

    [ExposeClass(UIConstants.Service, ExposedType = typeof(ListProviderAddons), LifeCycle = IocScopeEnum.Transiant)]
    public class ListProviderAddons : ProviderListBase<AddOnLibrary>
    {

        public ListProviderAddons(AddOnLibraries moduleSpecifications)
        {
            _moduleSpecifications = moduleSpecifications;
        }

        public override IEnumerable<ListItem<AddOnLibrary>> GetItems()
        {

            List<ListItem<AddOnLibrary>> list = new List<ListItem<AddOnLibrary>>();

            if (Instance != null && Property != null)
            {
                var items = _moduleSpecifications.GetModules();
                foreach (var item in items)
                    list.Add(CreateItem(item, item.Name, item.Uuid, c =>
                    {

                    }));
            }

            return list;

        }

        protected override object ResolveOriginalValue(ListItem<AddOnLibrary> item)
        {
            return item.Value;
        }


        private readonly AddOnLibraries _moduleSpecifications;

    }


}
