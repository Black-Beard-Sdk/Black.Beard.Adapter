namespace Bb.UIComponents.Guards
{
    public class GuardContainer<TIGuardMenu> : GuardContainer
            where TIGuardMenu : IGuardMenu
    {


        public GuardContainer(Func<TIGuardMenu, bool> evaluator)
        {
            _evaluator = evaluator;
        }

        public override bool Evaluate(GuardMenuProvider provider)
        {
            TIGuardMenu service = (TIGuardMenu)provider.Get(typeof(TIGuardMenu));
            var result = _evaluator(service);
            return result;
        }

        private readonly Func<TIGuardMenu, bool> _evaluator;

    }


    public abstract class GuardContainer
    {
        public abstract bool Evaluate(GuardMenuProvider provider);

    }


}
