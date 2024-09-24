using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Bb.Addons;
using Bb.Storage;

namespace Bb.Modules
{


    public class Document : ModelBase<Guid>
    {

        public Document()
        {

        }




        [StoreDescriptor(externalize: true, order: 5)]
        public Guid ModuleUuid { get; set; }

        public Guid Specification { get; set; }



        [JsonIgnore]
        public Feature Feature { get; set; }

        [JsonIgnore]
        public Documents Parent { get; internal set; }


        public JsonNode Model { get; set; }

        public string GetRoute()
        {
            return Feature.GetRoute(Uuid);
        }

        public object Load()
        {
        
            if (Feature != null)
                if (Feature.LoadDocument(this, out var result))
                    return result;
            
            return null;

        }

        public void Save(object model)
        {
            Feature.Save(this, model);
        }


    }


}
