using Bb.ComponentModel.Attributes;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Security.Principal;

namespace Bb.UIComponents.Guards
{


    [ExposeClass("Service", LifeCycle = IocScopeEnum.Transiant)]
    public class GuardDelegate : IGuard
    {

        public GuardDelegate()
        {

        }    

        public void Initialize(Func<IPrincipal, bool> guard)
        {
            _guard = guard;
        }

        public bool Evaluate(ClaimsPrincipal principal)
        {
            return _guard(principal);
        }

        private Func<IPrincipal, bool> _guard;

    }
   

}
