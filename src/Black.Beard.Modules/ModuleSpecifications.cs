using Bb.ComponentModel.Attributes;

namespace Bb.Modules
{


    [ExposeClass("Service", ExposedType = typeof(ModuleSpecifications), LifeCycle = IocScopeEnum.Singleton)]
    public class ModuleSpecifications
    {

        public ModuleSpecifications()
        {
            
        }
        /// <summary>
        /// Return all modules
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ModuleSpecification> GetModules()
        {

            if (_items == null)
                lock (_lock)
                    if (_items == null)
                    {

                        var items = new Dictionary<Guid, ModuleSpecification>();

                        var types = ComponentModel.TypeDiscovery.Instance
                            .GetTypesWithAttributes<ExposeClassAttribute>(typeof(object),
                            c => c.ExposedType == typeof(ModuleSpecification) && c.Context == "Plugin");
                        Add(types);

                        _items = items;

                    }

            return _items.Values;

        }

        private void Add(IEnumerable<Type> types)
        {
            foreach (var item in types)
                Add(item);
        }

        private void Add(Type item)
        {
            var module = (ModuleSpecification)Activator.CreateInstance(item);
            _items.Add(module.Uuid, module);

        }

        private volatile object _lock = new object();
        private Dictionary<Guid, ModuleSpecification> _items;

    }


}
