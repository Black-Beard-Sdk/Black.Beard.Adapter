using Bb.Addons;
using Bb.ComponentModel.Attributes;
using Bb.Diagrams;
using Bb.Modules.Bpms.Models;
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

            if (base.Load<BpmsDiagram>(featureInstance, out var r))
            {
                r.SetSpecifications(GetTools());
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

        /// <summary>
        /// Return all modules
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DiagramToolBase> GetTools()
        {

            if (_items == null)
                lock (_lock)
                    if (_items == null)
                    {

                        var filter = _filter;
                        var items = new Dictionary<Guid, DiagramToolBase>();

                        var types = ComponentModel.TypeDiscovery.Instance
                            .GetTypesWithAttributes<ExposeClassAttribute>(typeof(object),
                            c => c.ExposedType == typeof(DiagramToolBase) && c.Context == filter)
                            .ToList();

                        foreach (var item in types)
                        {
                            var module = (DiagramToolBase)Activator.CreateInstance(item);
                            items.Add(module.Uuid, module);
                        }

                        _items = items;
                    }

            return _items.Values;

        }

        private volatile object _lock = new object();
        private Dictionary<Guid, DiagramToolBase> _items;
        private readonly string _filter;

    }


}
