using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Bb.Addons;
using Bb.Commands;
using Bb.Storage;

namespace Bb.Modules
{

    public class Document : ModelBase<Guid>
    {

        /// <summary>
        /// Generic document
        /// </summary>
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

        public CommandTransactionManager Command { get; private set; }

        public bool CanBeCommand => Command != null;

        public string GetRoute()
        {
            return Feature.GetRoute(Uuid);
        }

        public object Load()
        {

            if (Feature != null)
                if (Feature.LoadDocument(this, out var result))
                {

                    if (result is IMemorizer m)
                        m.CommandManager?.Reset();

                    return result;
                }

            return null;

        }

        public void Save(object model)
        {
            Feature.Save(this, model);
        }


    }


}
