
namespace Bb.Modules
{

    public class ModuleSpecification
    {

        protected ModuleSpecification(Guid uuid, string name, string description)
        {

            if (uuid == Guid.Empty)
                throw new ArgumentException("uuid is empty");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name is empty");

            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("description is empty");

            this.Uuid = uuid;
            this.Name = name;
            this.Description = description;

            GetFeatures();

        }

        /// <summary>
        /// Return all features
        /// </summary>
        /// <returns></returns>
        private IEnumerable<FeatureSpecification> GetFeatures()
        {

            if (_items == null)
                lock (_lock)
                    if (_items == null)
                    {
                        var items = new Dictionary<Guid, FeatureSpecification>();

                        var assembly = GetType().Assembly;
                        var types = assembly.GetTypes()
                            .Where(c => typeof(FeatureSpecification).IsAssignableFrom(c))
                            .ToList();

                        foreach (var item in types)
                        {
                            var module = (FeatureSpecification)Activator.CreateInstance(item);
                            items.Add(module.Uuid, module);
                        }

                        _items = items;

                    }

            return _items.Values;

        }

        /// <summary>
        /// Uuid of the module
        /// </summary>
        public Guid Uuid { get; }

        /// <summary>
        /// Name of the module
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Describe the module
        /// </summary>
        public string Description { get; }

        private volatile object _lock = new object();
        private Dictionary<Guid, FeatureSpecification> _items;

    }

}
