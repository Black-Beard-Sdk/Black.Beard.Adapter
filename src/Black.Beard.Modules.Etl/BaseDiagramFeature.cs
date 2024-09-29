using Bb.Addons;
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

            options.Converters.Add(new InstanceJsonConverter());

            return options;
        }

        private volatile object _lock = new object();
        private Dictionary<Guid, DiagramToolBase> _items;
        private readonly string _filter;

    }


}
