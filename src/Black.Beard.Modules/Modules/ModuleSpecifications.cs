using Bb.ComponentModel.Attributes;

namespace Bb.Modules
{


    [ExposeClass(UIConstants.Service, ExposedType = typeof(ModuleSpecifications), LifeCycle = IocScopeEnum.Singleton)]
    public class ModuleSpecifications
    {

        public ModuleSpecifications(FeatureSpecifications featureSpecifications)
        {

            this.FeatureSpecifications = featureSpecifications;
        }


        /// <summary>
        /// Return all modules
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ModuleSpecification> GetModules()
        {

            if (_items == null)
                lock (_lock)
                    if (_items == null)
                    {

                        var filter = Bb.ComponentModel.ConstantsCore.Plugin;
                        var items = new Dictionary<Guid, ModuleSpecification>();

                        var types = ComponentModel.TypeDiscovery.Instance
                            .GetTypesWithAttributes<ExposeClassAttribute>(typeof(object),
                            c => c.ExposedType == typeof(ModuleSpecification) && c.Context == filter).ToList();

                        foreach (var item in types)
                        {
                            var module = (ModuleSpecification)Activator.CreateInstance(item);
                            module.Parent = this;
                            items.Add(module.Uuid, module);
                        }

                        _items = items;
                    }

            return _items.Values;

        }

        public ModuleSpecification GetModule(Guid guid)
        {
            if (_items == null)
                lock (_lock)
                    if (_items == null)
                        GetModules();

            if (_items.TryGetValue(guid, out ModuleSpecification module))
                return module;

            return null;

        }

        private volatile object _lock = new object();

        private Dictionary<Guid, ModuleSpecification> _items;

        public FeatureSpecifications FeatureSpecifications { get; }
    }


}
