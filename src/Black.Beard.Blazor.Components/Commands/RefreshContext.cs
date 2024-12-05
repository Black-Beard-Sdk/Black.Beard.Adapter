namespace Bb.Commands
{
    public class RefreshContext
    {

        public RefreshContext(string sessionId, Transaction transaction)
        {
            this.SessionId = sessionId;
            this.Transaction = transaction;
            this._dic = new Dictionary<string, RefreshContextItem>();
        }

        public void Add(Guid key, object model, RefreshStrategy change)
        {
            Add(key.ToString(), model, change);
        }

        public void Add(string key, object model, RefreshStrategy change)
        {

            if (!this._dic.TryGetValue(key, out var c))
                this._dic.Add(key, c = new RefreshContextItem(key, model));
            else
                c.Model = model;

            c.Strategy(change);

        }

        public bool Evaluate(Guid uuid, Func<RefreshContextItem, bool> filter = null)
        {
            return Evaluate(uuid.ToString(), filter);
        }

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

        public string SessionId { get; }

        public Transaction Transaction { get; }

        public bool HasChange => _dic.Count > 0;

        private readonly Dictionary<string, RefreshContextItem> _dic;
    }


}
