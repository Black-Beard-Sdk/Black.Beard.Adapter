using Bb.ComponentModel.Attributes;
using Bb.Diagrams;
using Bb.Modules.Etl.Pages;
using static MudBlazor.CategoryTypes;

namespace Bb.Modules.Etl
{


    [ExposeClass(Bb.ComponentModel.ConstantsCore.Plugin, ExposedType = typeof(FeatureSpecification), LifeCycle = IocScopeEnum.Transiant)]
    public class EtlDiagramFeature : FeatureSpecification
    {

        public EtlDiagramFeature()
            : base(new Guid(Filter),
            "data flow diagram",
            "Design data flow for your module",
            new Guid(ModuleDataFlow.Filter),
            typeof(Diagram)
                  )
        {

            this.Page = typeof(DiagramPage);

        }


        public override object GetModel(FeatureInstance featureInstance)
        {

            var result =  (Diagram)base.GetModel(featureInstance);

            var tools = GetTools();
            foreach (var item in tools)
                result.Specifications.Add(item);

            return result;

        }

        public override void SetModel(FeatureInstance featureInstance, object model)
        {
            base.SetModel(featureInstance, model);
        }

        /// <summary>
        /// Return all modules
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DiagramItemSpecificationBase> GetTools()
        {

            if (_items == null)
                lock (_lock)
                    if (_items == null)
                    {

                        var filter = Filter;
                        var items = new Dictionary<Guid, DiagramItemSpecificationBase>();

                        var types = ComponentModel.TypeDiscovery.Instance
                            .GetTypesWithAttributes<ExposeClassAttribute>(typeof(object),
                            c => c.ExposedType == typeof(DiagramItemSpecificationBase) && c.Context == filter).ToList();

                        foreach (var item in types)
                        {
                            var module = (DiagramItemSpecificationBase)Activator.CreateInstance(item);
                            items.Add(module.Uuid, module);
                        }

                        _items = items;
                    }

            return _items.Values;

        }

        public const string Filter = "C9119B69-5DD9-45D2-A28A-617D6CB9D7F9";

        private volatile object _lock = new object();
        private Dictionary<Guid, DiagramItemSpecificationBase> _items;

    }



}
