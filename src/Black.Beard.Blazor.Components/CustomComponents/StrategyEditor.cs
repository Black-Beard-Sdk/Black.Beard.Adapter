
namespace Bb.CustomComponents
{

    public class StrategyEditor
    {

        public StrategyEditor(PropertyKingView propertyKingView, Type sourceType, Type componentView, Func<object> createInstance)
            : this(propertyKingView.ToString(), sourceType, componentView, createInstance)
        {

        }

        public StrategyEditor(string propertyKingView, Type sourceType, Type componentView, Func<object> createInstance)
        {
            this.PropertyKingView = propertyKingView.ToString();
            this.SourceType = sourceType;
            this.ComponentView = componentView;
            this.CreateInstance = createInstance;
            Initializers = new List<Action<Type, StrategyMapper, PropertyObjectDescriptor>>();
        }

        public string PropertyKingView { get; }
        public Type SourceType { get; }

        public Type ComponentView { get; }

        public Func<object>? CreateInstance { get; }

        public string Source { get; internal set; }

        public Dictionary<Type, Action<Attribute, StrategyMapper, PropertyObjectDescriptor>> Initializer { get; internal set; }

        public List<Action<Type, StrategyMapper, PropertyObjectDescriptor>> Initializers { get; }

    }

}
