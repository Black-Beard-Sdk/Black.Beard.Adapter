using Bb.Addons;
using Bb.ComponentModel.Loaders;
using Bb.Diagrams;
using Bb.TypeDescriptors;
using System.Text.Json;

namespace Bb.Modules
{

    public class BaseDiagramFeature<T1>
        : Feature<T1>
            where T1 : ISave
    {

        public BaseDiagramFeature(Guid uuid, string name, string description, Guid owner)
            : base(uuid, name, description, owner)
        {
            _filter = uuid.ToString().ToUpper();
        }

        public override bool Load<T>(Document featureInstance, out T result)
        {

            result = default;

            if (base.Load<T>(featureInstance, out var r))
            {

                if (r is Diagram d)
                {

                    var toolbox = d.Toolbox;

                    foreach (var item in d.Models)
                        if (toolbox.TryGetNodeTool(item.Type, out var tool))
                            item.Initialize(tool, false);

                    //foreach (var item in d.Relationships)
                    //    if (toolbox.TryGetLinkTool(item.Type, out var tool))
                    //        item.Initialize(tool);

                }

                result = (T)(object)r;

            }

            return result != null;

        }

        public override void Save(Document featureInstance, object model)
        {
            base.Save(featureInstance, model);
        }

        protected override JsonSerializerOptions BuildJsonSerializerOptions()
        {
            var options = base.BuildJsonSerializerOptions();
            options.AppendConverterFor(Model, initializer);
            options.Converters.Add(new SerializableDiagramNodeJsonConverter());
            return options;
        }

        private volatile object _lock = new object();
        private Dictionary<Guid, DiagramToolBase> _items;
        private readonly string _filter;

    }


}
