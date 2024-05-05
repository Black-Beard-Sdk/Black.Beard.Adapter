namespace Bb
{
    public class RefreshEventArgs : EventArgs
    {

        public RefreshEventArgs(params object[] toRefresh)
        {
            this._objectToRefresh = new HashSet<object>(toRefresh);
        }

        public bool MustRefresh(object o)
        {
            return _objectToRefresh.Contains(o);
        }

        private readonly HashSet<object> _objectToRefresh;

    }

}
