using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.DataAnnotations;
using System.Collections;
using System.ComponentModel;

namespace Bb.Modules
{

    [ExposeClass(UIConstants.Service, ExposedType = typeof(ListProviderModule), LifeCycle = IocScopeEnum.Transiant)]
    public class ListProviderModule : ProviderListBase<ModuleSpecification>
    {

        public ListProviderModule(ModuleSpecifications moduleSpecifications)
        {
            _moduleSpecifications = moduleSpecifications;
        }

        public override IEnumerable<ListItem<ModuleSpecification>> GetItems()
        {

            List<ListItem<ModuleSpecification>> list = new List<ListItem<ModuleSpecification>>();

            if (Instance != null && Property != null)
            {
                var items = _moduleSpecifications.GetModules();
                foreach (var item in items)
                    list.Add(CreateItem( item, item.Name, item.Uuid, c =>
                    {

                    }));
            }

            return list;
                    
        }

        protected override object ResolveOriginalValue(ListItem<ModuleSpecification> item)
        {
            return item.Value;
        }
         

        private readonly ModuleSpecifications _moduleSpecifications;

    }


}
