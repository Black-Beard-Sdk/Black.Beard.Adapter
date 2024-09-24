namespace Bb
{
    /// <summary>
    /// Base class for focused service
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FocusServiceBase<T> : IFocusedService<T>
    {

        /// <summary>
        /// Notify the focus change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="test"></param>
        public virtual void FocusChange(object sender, Func<T, object, bool>? test)
        {
            FocusChange(sender, test, null);
        }

        /// <summary>
        /// Notify the focus change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="test"></param>
        /// <param name="action"></param>
        public virtual void FocusChange(object sender, Func<T, object, bool>? test, Action<T, object>? action)
        {

            if (_lastFocused != sender)
                lock (_lock)
                    if (_lastFocused != sender)
                    {
                        _lastFocused = sender;
                        FocusChanged?.Invoke(sender, new EvaluatorEventArgs<T>(test, action));
                    }

        }

        /// <summary>
        /// event raised when the focus change
        /// </summary>
        public event EventHandler<EvaluatorEventArgs<T>>? FocusChanged;


        private object? _lastFocused;
        private volatile object _lock = new object();
    
    }


}
