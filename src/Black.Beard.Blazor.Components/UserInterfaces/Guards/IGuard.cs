using System.Security.Claims;
using System.Security.Principal;

namespace Bb.UIComponents.Guards
{
    public interface IGuard
    {

        bool Evaluate(ClaimsPrincipal principal);

    }


}
