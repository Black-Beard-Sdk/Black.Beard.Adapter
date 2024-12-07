namespace Bb.Commands
{

    /// <summary>
    /// Represents an item that is to changed in the restore processing.
    /// </summary>
    public class RefreshContextItem
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshContextItem"/> class.
        /// </summary>
        /// <param name="key">key of the object</param>
        /// <param name="model">model has changed</param>
        public RefreshContextItem(string key, object model)
        {
            this.Key = key;
            this.Model = model;
            this._extendedModel = new List<(object, RefreshStrategy)>();
        }

        /// <summary>
        /// Impact of the model
        /// </summary>
        /// <param name="strategy"></param>
        /// <returns></returns>
        public RefreshContextItem Strategy(RefreshStrategy strategy)
        {
            this.Change |= strategy;
            return this;
        }

        /// <summary>
        /// Key of the model
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// The model has changed
        /// </summary>
        public object Model { get; set; }

        /// <summary>
        /// Add an extended model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="strategy"></param>
        public void AddExtendedData(object model, RefreshStrategy strategy)
        {
            _extendedModel.Add((model, strategy));
        }

        /// <summary>
        /// Extended models list
        /// </summary>
        public IEnumerable<(object, RefreshStrategy)> ExtendedModel => _extendedModel;

        /// <summary>
        /// Impact of the model
        /// </summary>
        public RefreshStrategy Change { get; private set; }

        private List<(object, RefreshStrategy)> _extendedModel { get; set; }

    }


}
