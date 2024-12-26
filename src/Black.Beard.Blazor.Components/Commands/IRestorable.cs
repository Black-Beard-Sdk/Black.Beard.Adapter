
using System.Collections;

namespace Bb.Commands
{

    public interface IRestorable
    {

        /// <summary>
        /// remove values from the current instance from the specified model.
        /// </summary>
        /// <param name="right"></param>
        /// <param name="ctx"></param>
        bool Restore(object right, RefreshContext ctx);

    }   

    public enum RefreshStrategy
    {
        Removed,
        Added,
        Updated,
    }


}
