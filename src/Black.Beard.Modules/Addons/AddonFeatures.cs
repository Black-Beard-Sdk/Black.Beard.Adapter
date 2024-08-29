using Bb.ComponentModel.Attributes;

namespace Bb.Addons
{

    /// <summary>
    /// Service for discover all features in modules
    /// </summary>
    [ExposeClass(UIConstants.Service, ExposedType = typeof(AddonFeatures), LifeCycle = IocScopeEnum.Scoped)]
    public class AddonFeatures
    {

        public AddonFeatures()
        {

        }

        /// <summary>
        /// Return all modules
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Feature> GetFeatures()
        {

            if (_items == null)
                lock (_lock)
                    if (_items == null)
                    {

                        var filter = ComponentModel.ConstantsCore.Plugin;
                        var items = new Dictionary<Guid, Feature>();

                        var types = ComponentModel.TypeDiscovery.Instance
                            .GetTypesWithAttributes<ExposeClassAttribute>(typeof(object),
                            c => c.ExposedType == typeof(Feature) && c.Context == filter).ToList();

                        foreach (var item in types)
                        {
                            var feature = (Feature)Activator.CreateInstance(item);
                            feature.Parent = this;
                            items.Add(feature.Uuid, feature);
                        }

                        _items = items;
                    }

            return _items.Values;

        }

        public Feature GetFeature(Guid guid)
        {
            if (_items == null)
                lock (_lock)
                    if (_items == null)
                        GetFeatures();

            if (_items.TryGetValue(guid, out Feature module))
                return module;

            return null;

        }

        private volatile object _lock = new object();
        private Dictionary<Guid, Feature> _items;

    }


}
