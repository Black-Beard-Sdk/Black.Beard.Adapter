using Bb.ComponentModel.Attributes;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Security.Principal;

namespace Bb.UIComponents.Guards
{


    [ExposeClass("Service", LifeCycle = IocScopeEnum.Transiant)]
    public class GuardPolicy : IGuard
    {

        public GuardPolicy(IAuthorizationService authorizationService)
        {
            this._authorizationService = authorizationService;
        }

        public void Initialize(string[] requiredPolicies)
        {
            _requiredPolicies = requiredPolicies;
        }

        public bool Evaluate(ClaimsPrincipal principal)
        {

            foreach (var policy in _requiredPolicies)
            {
                var result = _authorizationService.AuthorizeAsync(principal, policy);
                if (!result.Result.Succeeded)
                    return false;
            }
            
            return true;

        }

        private string[] _requiredPolicies;
        private readonly IAuthorizationService _authorizationService;

    }


}
