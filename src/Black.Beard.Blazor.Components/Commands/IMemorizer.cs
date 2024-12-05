
namespace Bb.Commands
{

    /// <summary>
    /// Interface for a command memorizer.
    /// </summary>
    public interface IMemorizer
    {

        /// <summary>
        /// Gets a value indicating whether the memorizer can memorize commands.
        /// </summary>
        bool CanMemorize { get; }

        /// <summary>
        /// Memorizes the commands to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to memorize the commands to.</param>
        void Memorize(Stream stream);

        /// <summary>
        /// Restores the commands from the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction to restore the commands from.</param>
        /// <param name="context">List of object that has been changed.</param>
        void Restore(RefreshContext context);

        /// <summary>
        /// Gets the command transaction manager associated with the memorizer.
        /// </summary>
        ITransactionManager CommandManager { get; }

    }


}
