
using Bb.Diagrams;
using Blazor.Diagrams.Core.Models.Base;

namespace Bb.Commands
{

    public interface IRestorableModel
    {

        /// <summary>
        /// Restores the current instance from the specified model.
        /// </summary>
        /// <param name="transaction">The transaction to restore the commands from.</param>
        bool Restore(object model, RefreshContext ctx, RefreshStrategy strategy = RefreshStrategy.All);

    }

    [Flags]
    public enum RefreshStrategy
    {
        Remove = 1,
        Add = 2,
        Update = 4,
        All = Remove | Add | Update,
    }


}
