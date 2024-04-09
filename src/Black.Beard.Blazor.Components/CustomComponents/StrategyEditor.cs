namespace Bb.CustomComponents
{
    public class StrategyEditor
    {

        public StrategyEditor(PropertyKingView propertyKingView, Type componentView, Func<object> createInstance)
        {
            this.PropertyKingView = propertyKingView;
            this.ComponentView = componentView;
            this.CreateInstance = createInstance;
        }

        public PropertyKingView PropertyKingView { get; }

        public Type ComponentView { get; }

        public Func<object> CreateInstance { get; }


    }

}
