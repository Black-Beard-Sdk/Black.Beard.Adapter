using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Bb.Storage;

namespace Bb.Modules
{
    public class FeatureInstance : ModelBase<Guid>
    {

        public FeatureInstance()
        {

        }

        [StoreDescriptor(externalize: true, order: 4)]
        public string Label { get; set; }

        [StoreDescriptor(externalize: true, order: 5)]
        public Guid ModuleUuid { get; set; }

        public string Description { get; set; }

        public Guid Specification { get; set; }

        public JsonNode Model { get; set; }


        [JsonIgnore]
        public FeatureSpecification FeatureSpecification { get; set; }

        [JsonIgnore]
        public FeatureInstances Parent { get; internal set; }

        public string GetRoute()
        {
            return FeatureSpecification.GetRoute(Uuid);
        }

        public object GetModel()
        {
            return FeatureSpecification.GetModel(this);
        }

        public void SetModel(object model)
        {
            FeatureSpecification.SetModel(this, model);
        }


    }


}
