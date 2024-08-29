using Bb.ComponentModel.Attributes;

namespace Bb.Addons
{

    /// <summary>
    /// Service for discover all modules
    /// </summary>

    [ExposeClass(UIConstants.Service, ExposedType = typeof(AddOnLibraries), LifeCycle = IocScopeEnum.Scoped)]
    public class AddOnLibraries
    {

        public AddOnLibraries(AddonFeatures featureSpecifications)
        {

            FeatureSpecifications = featureSpecifications;
        }


        /// <summary>
        /// Return all modules
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AddOnLibrary> GetModules()
        {

            if (_items == null)
                lock (_lock)
                    if (_items == null)
                    {

                        var filter = ComponentModel.ConstantsCore.Plugin;
                        var items = new Dictionary<Guid, AddOnLibrary>();

                        var types = ComponentModel.TypeDiscovery.Instance
                            .GetTypesWithAttributes<ExposeClassAttribute>(typeof(object),
                            c => c.ExposedType == typeof(AddOnLibrary) && c.Context == filter).ToList();

                        foreach (var item in types)
                        {
                            var module = (AddOnLibrary)Activator.CreateInstance(item);
                            module.Parent = this;
                            items.Add(module.Uuid, module);
                        }

                        _items = items;
                    }

            return _items.Values;

        }

        public AddOnLibrary GetModule(Guid guid)
        {
            if (_items == null)
                lock (_lock)
                    if (_items == null)
                        GetModules();

            if (_items.TryGetValue(guid, out AddOnLibrary module))
                return module;

            return null;

        }

        private volatile object _lock = new object();

        private Dictionary<Guid, AddOnLibrary> _items;

        public AddonFeatures FeatureSpecifications { get; }
    }


}
