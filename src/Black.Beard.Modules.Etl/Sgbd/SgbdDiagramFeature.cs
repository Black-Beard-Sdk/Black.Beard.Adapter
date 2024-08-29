using Bb.Addons;
using Bb.ComponentModel.Attributes;
using Bb.Diagrams;
using Bb.Modules.Sgbd.Models;

namespace Bb.Modules.Sgbd
{

    [ExposeClass(ComponentModel.ConstantsCore.Plugin, ExposedType = typeof(Feature), LifeCycle = IocScopeEnum.Transiant)]
    public class SgbdDiagramFeature : Feature
    {

        public SgbdDiagramFeature()
            : base(new Guid(Filter),
                "Sgbd diagram",
                "Design database structure",
                new Guid(ModuleDatas.Filter),
                typeof(SgbdDiagram)
            )
        {
            Page = typeof(SgbdDiagramPage);
        }


        public override object Load(Document featureInstance)
        {

            var result = (SgbdDiagram)base.Load(featureInstance);

            result.SetSave( d =>
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

        public const string Filter = "487E5B0F-C9E8-47A2-B72C-11DABCAE9C00";

        private volatile object _lock = new object();
        private Dictionary<Guid, DiagramSpecificationBase> _items;

    }


}
