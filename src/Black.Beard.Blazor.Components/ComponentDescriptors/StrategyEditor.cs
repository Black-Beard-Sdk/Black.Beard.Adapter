
using System.Collections;
using System.Reflection.Metadata;
using static MudBlazor.Colors;

namespace Bb.ComponentDescriptors
{

    public class StrategyEditor
    {

        public StrategyEditor(PropertyKindView propertyKingView, Type sourceType, (Type, Action<StrategyMapper, Descriptor>) target, Func<object> createInstance)
            : this(propertyKingView.ToString(), sourceType, target, createInstance)
        {

        }

        public StrategyEditor(string propertyKingView, Type sourceType, (Type, Action<StrategyMapper, Descriptor>) target, Func<object>? createInstance)
        {
            this.PropertyKindView = propertyKingView;
            this.SourceType = sourceType;
            this.ComponentView = target.Item1;
            this.Initializer = target.Item2;
            this.CreateInstance = createInstance;
            Initializers = new List<Action<Type, StrategyMapper, Descriptor>>();
            this.IsEnumerable = typeof(IEnumerable).IsAssignableFrom(sourceType);
        }

        public string PropertyKindView { get; }

        public Type SourceType { get; }

        public Type ComponentView { get; }

        public Func<object>? CreateInstance { get; }

        public string Source { get; internal set; }

        public Dictionary<Type, Action<Attribute, StrategyMapper, Descriptor>> AttributeInitializers { get; internal set; }

        public List<Action<Type, StrategyMapper, Descriptor>> Initializers { get; }
        public bool IsEnumerable { get; }

        public Action<StrategyMapper, Descriptor> Initializer { get; }
        
        public bool Unknown { get; internal set; }
    
    }

}
