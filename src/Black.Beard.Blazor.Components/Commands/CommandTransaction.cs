namespace Bb.Commands
{

    /// <summary>
    /// Represents a command transaction that can be committed or rolled back.
    /// </summary>
    public class CommandTransaction : IDisposable
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandTransaction"/> class.
        /// </summary>
        /// <param name="manager">The manager responsible for handling the transaction.</param>
        /// <param name="name">The name of the transaction.</param>
        /// <param name="index">The index of the transaction.</param>
        internal CommandTransaction(CommandTransactionManager manager, string name, int index)
        {
            _manager = manager;
            Index = index;
            Label = name;
        }

        /// <summary>
        /// Initializes the value for the transaction.
        /// </summary>
        /// <param name="target">The target command memorizer.</param>
        internal void InitializeValue(ICommandMemorizer target)
        {

            var path = _manager.TargetFolder.AsDirectory();
            if (!path.Exists)
                path.Create();
            _path = path.Combine(Index.ToString());

            var f = _path.AsFile();
            if (f.Exists)
                f.Delete();
            using (var stream = f.OpenWrite())
            {
                target.Memorize(stream);
            }

        }

        /// <summary>
        /// Gets or sets the label of the transaction.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets the index of the transaction.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Gets the stream associated with the transaction.
        /// </summary>
        public Stream Stream
        {
            get
            {
                return _path.AsFile().OpenRead();
            }
        }

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public void Commit()
        {

        }

        /// <summary>
        /// Disposes the transaction and rolls back any changes.
        /// </summary>
        public void Dispose()
        {
            _manager.Rollback();
        }

        /// <summary>
        /// Gets a view of the transaction.
        /// </summary>
        /// <returns>A <see cref="CommandTransactionView"/> representing the transaction view.</returns>
        public CommandTransactionView GetView()
        {
            return new CommandTransactionView(this);
        }


        private string _path;
        private readonly CommandTransactionManager _manager;

        //private readonly int _crc;
    }

}
