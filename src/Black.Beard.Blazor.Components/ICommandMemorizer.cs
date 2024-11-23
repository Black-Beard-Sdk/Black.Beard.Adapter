using static Bb.CommandTransactionManager;

namespace Bb
{


    public interface ICommandMemorizer
    {


        MemorizerEnum Mode { get; }

        bool CanMemorize { get; }

        void Memorize(Stream stream);

        void Restore(CommandTransaction transaction);

        ICommandTransactionManager CommandManager { get; }

    }

    public enum MemorizerEnum
    {
        Global,
        Snapshot,
    }


}
