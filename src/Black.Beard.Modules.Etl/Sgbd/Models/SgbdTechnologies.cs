using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Loaders;

namespace Bb.Modules.Sgbd.Models
{

    [ExposeClass(ComponentModel.ConstantsCore.Service, ExposedType = typeof(SgbdTechnologies), LifeCycle = IocScopeEnum.Singleton)]
    public class SgbdTechnologies
    {

        public SgbdTechnologies()
        {
            
            var filter = Bb.ComponentModel.ConstantsCore.Plugin;
            _items = new List<SgbdTechnology>();

            var types = ComponentModel.TypeDiscovery.Instance
                .GetTypesWithAttributes<ExposeClassAttribute>(typeof(SgbdTechnology),
                c => c.ExposedType == typeof(SgbdTechnology) && c.Context == filter).ToList();

            foreach (var item in types)
            {
                var module = (SgbdTechnology)Activator.CreateInstance(item);
                module.Parent = this;
                _items.Add(module);
            }

        }

        public IEnumerable<SgbdTechnology> Items => _items.ToArray();

        private SgbdTechnologies Add(SgbdTechnology item)
        {
            _items.Add(item);
            return this;
        }

        internal SgbdTechnology GetTechnology(string technology)
        {
            return _items.FirstOrDefault(c => c.Name == technology);
        }

        private List<SgbdTechnology> _items;

    }

}
