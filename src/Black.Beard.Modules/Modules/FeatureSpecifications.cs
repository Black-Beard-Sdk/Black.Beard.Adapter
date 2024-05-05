using Bb.ComponentModel.Attributes;

namespace Bb.Modules
{
    [ExposeClass(UIConstants.Service, ExposedType = typeof(FeatureSpecifications), LifeCycle = IocScopeEnum.Singleton)]
    public class FeatureSpecifications
    {

        public FeatureSpecifications()
        {

        }

        /// <summary>
        /// Return all modules
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FeatureSpecification> GetFeatures()
        {

            if (_items == null)
                lock (_lock)
                    if (_items == null)
                    {

                        var filter = Bb.ComponentModel.ConstantsCore.Plugin;
                        var items = new Dictionary<Guid, FeatureSpecification>();

                        var types = ComponentModel.TypeDiscovery.Instance
                            .GetTypesWithAttributes<ExposeClassAttribute>(typeof(object),
                            c => c.ExposedType == typeof(FeatureSpecification) && c.Context == filter).ToList();

                        foreach (var item in types)
                        {
                            var feature = (FeatureSpecification)Activator.CreateInstance(item);
                            feature.Parent = this;
                            items.Add(feature.Uuid, feature);
                        }

                        _items = items;
                    }

            return _items.Values;

        }

        public FeatureSpecification GetFeature(Guid guid)
        {
            if (_items == null)
                lock (_lock)
                    if (_items == null)
                        GetFeatures();

            if (_items.TryGetValue(guid, out FeatureSpecification module))
                return module;

            return null;

        }

        private volatile object _lock = new object();
        private Dictionary<Guid, FeatureSpecification> _items;

    }


}
