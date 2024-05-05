
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

        }

        /// <summary>
        /// Return all features
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FeatureSpecification> GetFeatures()
        {
            return Parent.FeatureSpecifications.GetFeatures().Where(c => c.Owner == this.Uuid);
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

        public ModuleSpecifications Parent { get; internal set; }


    }

}
