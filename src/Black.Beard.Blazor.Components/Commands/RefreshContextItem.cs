namespace Bb.Commands
{

    public class RefreshContextItem
    {

        public RefreshContextItem(string key, object model)
        {
            this.Key = key;
            this.Model = model;
            this._extendedModel = new List<(object, RefreshStrategy)>();
        }

        public RefreshContextItem Strategy(RefreshStrategy strategy)
        {
            this.Change |= strategy;
            return this;
        }

        public string Key { get; }

        public object Model { get; set; }

        public void AddExtendedData(object model, RefreshStrategy strategy)
        {
            _extendedModel.Add((model, strategy));
        }

        public IEnumerable<(object, RefreshStrategy)> ExtendedModel => _extendedModel;

        public RefreshStrategy Change { get; private set; }

        private List<(object, RefreshStrategy)> _extendedModel { get; set; }

    }


}
