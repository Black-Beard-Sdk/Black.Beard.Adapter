namespace Bb.Commands
{

    /// <summary>
    /// Represents a view of a command transaction.
    /// </summary>
    public class TransactionView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionView"/> class.
        /// </summary>
        /// <param name="transaction">The command transaction to create a view for.</param>
        public TransactionView(Transaction transaction)
        {
            Index = transaction.Index;
            Label = transaction.Label;
        }

        /// <summary>
        /// Gets or sets the index of the transaction.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the label of the transaction.
        /// </summary>
        public string Label { get; set; }
    }

}
