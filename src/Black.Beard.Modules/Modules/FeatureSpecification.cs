
namespace Bb.Modules
{

    public class FeatureSpecification
    {


        protected FeatureSpecification(Guid uuid, string name, string description, Type owner, Type model = null)
        {

            if (uuid == Guid.Empty)
                throw new ArgumentException("uuid is empty");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name is empty");

            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("description is empty");

            if (owner == null)
                throw new ArgumentException("owner is null");

            this.Uuid = uuid;
            this.Name = name;
            this.Description = description;
            this.Owner = owner;
            this.Model = model;

        }


        /// <summary>
        /// Uuid of the feature
        /// </summary>
        public Guid Uuid { get; }

        /// <summary>
        /// Name of the feature
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Module owner
        /// </summary>
        public Type Owner { get; }

        /// <summary>
        /// Describe the feature module
        /// </summary>
        public Type Model { get; }

        /// <summary>
        /// Describe the feature
        /// </summary>
        public string Description { get; }

    }

}
