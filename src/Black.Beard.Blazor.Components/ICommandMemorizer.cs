using static Bb.CommandTransactionManager;

namespace Bb
{

    public interface ICommandMemorizer
    {

        bool CanMemorize { get; }

        void Memorize(Stream stream);

        void Restore(CommandTransaction transaction);

        ICommandTransactionManager CommandManager { get; }

    }

}
