namespace Bb.Commands
{
    public interface IRestorableMapper
    {

        /// <summary>
        /// Restore left value with data of right value.
        /// </summary>
        /// <param name="right"></param>
        /// <param name="ctx"></param>
        bool Restore(object left, object right, RefreshContext ctx);

    }

}
