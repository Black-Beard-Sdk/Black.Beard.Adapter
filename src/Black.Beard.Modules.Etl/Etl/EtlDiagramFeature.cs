using Bb.Addons;
using Bb.ComponentModel.Attributes;
using Bb.Diagrams;
using Bb.Modules.Etl.Models;

namespace Bb.Modules.Etl
{


    [ExposeClass(Bb.ComponentModel.ConstantsCore.Plugin, ExposedType = typeof(Feature), LifeCycle = IocScopeEnum.Transiant)]
    public class EtlDiagramFeature : Feature
    {
        private const string Description = "Design data flow for your solution";

        public EtlDiagramFeature()
            : base(new Guid(Filter),
                "data flow diagram",
                Description,
                new Guid(ModuleDatas.Filter),
                typeof(EtlDiagram)
            )
        {
            this.Page = typeof(EtlDiagramPage);
        }


        public override object Load(Document featureInstance)
        {
            var result =  (Diagram)base.Load(featureInstance);

            result.SetSave(d =>
            {
                Save(featureInstance, d);
                featureInstance.Parent.Save(featureInstance); 
            });

            result.SetSpecifications(GetTools());
            return result;

        }

        public override void Save(Document featureInstance, object model)
        {
            base.Save(featureInstance, model);
        }

        /// <summary>
        /// Return all modules
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DiagramSpecificationBase> GetTools()
        {

            if (_items == null)
                lock (_lock)
                    if (_items == null)
                    {

                        var filter = Filter;
                        var items = new Dictionary<Guid, DiagramSpecificationBase>();

                        var types = ComponentModel.TypeDiscovery.Instance
                            .GetTypesWithAttributes<ExposeClassAttribute>(typeof(object),
                            c => c.ExposedType == typeof(DiagramSpecificationBase) && c.Context == filter).ToList();

                        foreach (var item in types)
                        {
                            var module = (DiagramSpecificationBase)Activator.CreateInstance(item);
                            items.Add(module.Uuid, module);
                        }

                        _items = items;
                    }

            return _items.Values;

        }

        public const string Filter = "C9119B69-5DD9-45D2-A28A-617D6CB9D7F9";

        private volatile object _lock = new object();
        private Dictionary<Guid, DiagramSpecificationBase> _items;

    }


}
