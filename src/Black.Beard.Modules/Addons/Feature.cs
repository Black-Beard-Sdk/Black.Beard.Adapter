using Bb.Diagrams;
using Bb.Modules;
using Microsoft.AspNetCore.Components;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Bb.Addons
{

    public class Feature<T> : Feature
            where T : ISave
    {

        protected Feature(Guid uuid, string name, string description, Guid owner)
            : base(uuid, name, description, owner, typeof(T))
        {

        }

        public override bool LoadDocument(Document featureInstance, out object result)
        {

            result = default(T);

            if (Load<T>(featureInstance, out var r))
                result = r;

            return result != null;

        }


    }

    /// <summary>
    /// Base model for a new feature in a module
    /// </summary>
    public abstract class Feature
    {

        /// <summary>
        /// Initialize a new feature
        /// </summary>
        /// <param name="uuid">Unique key Feature</param>
        /// <param name="name">Name of the feature</param>
        /// <param name="description">Description of the feature</param>
        /// <param name="owner">Guid module owner</param>
        /// <param name="model">Type of the model</param>
        /// <exception cref="ArgumentException"></exception>
        protected Feature(Guid uuid, string name, string description, Guid owner, Type model)
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
            Owner = owner;
            Model = model;

        }


        public JsonSerializerOptions GetJsonSerializerOptions() => _jsonSerializerOptions ?? (_jsonSerializerOptions = BuildJsonSerializerOptions());

        protected virtual JsonSerializerOptions BuildJsonSerializerOptions()
        {

            var options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true,
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
                IncludeFields = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };

            options.Converters.Add(new JsonStringEnumConverter());
            //options.Converters.Add(new );

            return options;
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
                    Route = attribute.Template;
                }
                else
                    throw new ArgumentException("value is not a page");

            }
        }

        /// <summary>
        /// Describe the feature
        /// </summary>
        public string Description { get; }

        public AddonFeatures Parent { get; internal set; }

        public string Route { get; private set; }



        public string GetRoute(Guid uuid)
        {
            return Replace(Route, uuid.ToString());
        }

        private string Replace(string input, string substitution)
        {
            Regex regex = new Regex(_pattern, options);
            string result = regex.Replace(input, substitution);
            return result;
        }


        #region In/Out

        /// <summary>
        /// Load the document
        /// </summary>
        /// <param name="featureInstance"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public abstract bool LoadDocument(Document featureInstance, out object result);

        /// <summary>
        /// Load the document
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="featureInstance"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public virtual bool Load<T>(Document featureInstance, out T result)
            where T : ISave
        {

            result = default(T);

            if (featureInstance.Model == null)
                result = (T)Activator.CreateInstance(Model);

            else
            {

                var payload = featureInstance
                    .Model
                    .ToJsonString();

                var options = GetJsonSerializerOptions() ?? new JsonSerializerOptions();

                try
                {
                    result = (T)payload.Deserialize(Model, options);
                }
                catch (NotSupportedException ex)
                {
                    System.Diagnostics.Trace.Fail(ex.Message, ex.ToString());
                }

            }

            if (result != null)
            {

                result.SetSave<T>(d =>
                {
                    Save(featureInstance, d);
                    featureInstance.Parent.Save(featureInstance);
                });

                return true;

            }

            return false;

        }

        public virtual void Save(Document featureInstance, object model)
        {
            if (model == null)
                featureInstance.Model = null;

            else
            {

                var options = GetJsonSerializerOptions() ?? new JsonSerializerOptions();

                try
                {
                    var modelText = model.Serialize(options);
                    featureInstance.Model = JsonNode.Parse(modelText);
                }
                catch (Exception ex)
                {

                }
            }

        }

        #endregion In/Out


        private Type _page;
        private string _pattern = @"{\w+(:\w+)?}";
        private const RegexOptions options = RegexOptions.Multiline;
        private JsonSerializerOptions _jsonSerializerOptions;
    }

}
