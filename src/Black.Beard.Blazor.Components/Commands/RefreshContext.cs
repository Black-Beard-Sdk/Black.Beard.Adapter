namespace Bb.Commands
{

    /// <summary>
    /// Represents a context for refreshing objects.
    /// </summary>
    public class RefreshContext
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshContext"/> class.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="transaction"></param>
        public RefreshContext(string sessionId, Transaction transaction)
        {
            this.SessionId = sessionId;
            this.Transaction = transaction;
            this._dic = new Dictionary<string, RefreshContextItem>();
        }

        /// <summary>
        /// Adds a model to the context.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="model"></param>
        /// <param name="change"></param>
        public void Add(Guid key, object model, RefreshStrategy change)
        {
            Add(key.ToString(), model, change);
        }

        /// <summary>
        /// Adds a model to the context.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="model"></param>
        /// <param name="change"></param>
        public void Add(string key, object model, RefreshStrategy change)
        {

            if (!this._dic.TryGetValue(key, out var c))
                this._dic.Add(key, c = new RefreshContextItem(key, model));
            else
                c.Model = model;

            c.Strategy(change);

        }

        /// <summary>
        /// Evaluate if the key already exists in the context.
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public bool Evaluate(Guid uuid, Func<RefreshContextItem, bool> filter = null)
        {
            return Evaluate(uuid.ToString(), filter);
        }

        /// <summary>
        /// Evaluate if the key already exists in the context.
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public bool Evaluate(string uuid, Func<RefreshContextItem, bool> filter = null)
        {

            if (this._dic.TryGetValue(uuid, out var c))
            {
                if (filter != null)
                    return filter(c);
                else
                    return true;
            }

            return false;

        }

        /// <summary>
        /// Gets the session id of command manager
        /// </summary>
        public string SessionId { get; }

        /// <summary>
        /// Gets the transaction to restore associated with the context.
        /// </summary>
        public Transaction Transaction { get; }

        public bool HasChange => _dic.Count > 0;
        private readonly Dictionary<string, RefreshContextItem> _dic;

    }


}
