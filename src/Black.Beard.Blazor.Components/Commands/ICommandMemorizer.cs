
namespace Bb.Commands
{


    /// <summary>
    /// Interface for a command memorizer.
    /// </summary>
    public interface ICommandMemorizer
    {


        /// <summary>
        /// Gets the mode of the memorizer.
        /// </summary>
        MemorizerEnum Mode { get; }

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
        void Restore(CommandTransaction transaction);

        /// <summary>
        /// Gets the command transaction manager associated with the memorizer.
        /// </summary>
        ICommandTransactionManager CommandManager { get; }

    }

    /// <summary>
    /// Enumeration for the mode of the memorizer.
    /// </summary>
    public enum MemorizerEnum
    {
        Global,
        Snapshot,
    }


}
