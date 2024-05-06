
using Microsoft.AspNetCore.Components;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace Bb.Modules
{

    public class FeatureSpecification
    {


        protected FeatureSpecification(Guid uuid, string name, string description, Guid owner, Type model)
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
        public Guid Owner { get; }

        /// <summary>
        /// Describe the feature module
        /// </summary>
        public Type Model { get; }

        public Type Page
        {
            get => _page;
            protected set
            {

                if (value != null && typeof(ComponentBase).IsAssignableFrom(value))
                {
                    _page = value;
                    var attribute = _page.GetCustomAttribute<RouteAttribute>(true);
                    this.Route = attribute.Template;
                }
                else
                    throw new ArgumentException("value is not a page");

            }
        }

        /// <summary>
        /// Describe the feature
        /// </summary>
        public string Description { get; }

        public FeatureSpecifications Parent { get; internal set; }

        public string GetRoute(Guid uuid)
        {
            return Replace(Route, uuid.ToString());
        }

        public string Route { get; private set; }

        private string Replace(string input, string substitution)
        {
            Regex regex = new Regex(_pattern, options);
            string result = regex.Replace(input, substitution);
            return result;
        }

        public virtual object GetModel(FeatureInstance featureInstance)
        {            
            if (featureInstance.Model == null)
                return Activator.CreateInstance(Model);
            var payload = featureInstance.Model.ToJsonString();
            var result = payload.Deserialize(Model);
            return result;
        }

        public virtual void SetModel(FeatureInstance featureInstance, object model)
        {
            if (model == null)
                featureInstance.Model = null;
            else
            {
                var modelText = model.Serialize(true);
                featureInstance.Model = JsonObject.Parse(modelText);
            }

        }

        private Type _page;
        private string _pattern = @"{\w+(:\w+)?}";
        private const RegexOptions options = RegexOptions.Multiline;


    }

}
