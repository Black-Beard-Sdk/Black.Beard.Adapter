namespace Bb.Addons
{


    public class AddOnLibrary
    {

        protected AddOnLibrary(Guid uuid, string name, string description)
        {

            if (uuid == Guid.Empty)
                throw new ArgumentException("uuid is empty");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name is empty");

            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("description is empty");

            Uuid = uuid;
            Name = name;
            Description = description;

        }

        /// <summary>
        /// Return all features
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Feature> GetFeatures()
        {
            return Parent.FeatureSpecifications.GetFeatures().Where(c => c.Owner == Uuid);
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

        public AddOnLibraries Parent { get; internal set; }


    }

}
