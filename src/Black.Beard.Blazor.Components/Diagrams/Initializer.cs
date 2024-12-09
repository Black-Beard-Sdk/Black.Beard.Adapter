namespace Bb.Diagrams
{

    public class Initializer<T> : Initializer
        where T : UIModel
    {

        private Action<T> _initializer;
        private Action<T> _initializerAfterAdded;


        public Initializer<T> Initialize(Action<T> initializer)
        {
            _initializer = initializer;
            return this;
        }

        internal override void ExecuteInitialize(UIModel toolbox)
        {
            if (_initializer != null)
                _initializer((T)toolbox);
        }

        public Initializer<T> AfterAdded(Action<T> initializer)
        {
            _initializerAfterAdded = initializer;
            return this;
        }

        internal override void ExecuteInitializeAfterAdded(UIModel toolbox)
        {
            if (_initializerAfterAdded != null)
                _initializerAfterAdded((T)toolbox);
        }

    }


    public abstract class Initializer
    {

        internal abstract void ExecuteInitialize(UIModel toolbox);

        internal abstract void ExecuteInitializeAfterAdded(UIModel toolbox);

    }

}
