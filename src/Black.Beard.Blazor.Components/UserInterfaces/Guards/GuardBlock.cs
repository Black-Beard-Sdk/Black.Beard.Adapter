using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using System.Security.Claims;
using System.Security.Principal;

namespace Bb.UIComponents.Guards
{

    [ExposeClass("Service", LifeCycle = IocScopeEnum.Transiant)]
    public class GuardBlock
    {

        public GuardBlock(IServiceProvider serviceProvider)
        {

            if (serviceProvider == null)
                throw new System.ArgumentNullException(nameof(serviceProvider));

            this._provider = serviceProvider;
            this._guards = new List<IGuard>();
            this._functionGuards = new List<Func<GuardBlock, IGuard>>();
        }

        public bool Evaluate(ClaimsPrincipal? principal)
        {

            if (_functionGuards.Count > 0)
                lock (_lock)
                    if (_functionGuards.Count > 0)
                    {
                        var lst = _functionGuards.ToList();
                        foreach (var item in lst    )
                        {
                            _guards.Add(item(this));
                            _functionGuards.Remove(item);
                        }
                    }

            if (principal != null)
                foreach (var item in _guards)
                    if (!item.Evaluate(principal))
                        return false;

            return true;

        }


        public void Add(Func<IPrincipal, bool> guardMethod)
        {
            _functionGuards.Add((a) =>
            {
                var guard = (GuardDelegate)a.Provider.GetService(typeof(GuardDelegate));
                guard.Initialize(guardMethod);
                return guard;
            });
        }


        public void Add(params string[] guardPolicies)
        {
            _functionGuards.Add((a) =>
            {
                var guard = (GuardPolicy)a.Provider.GetService(typeof(GuardPolicy));
                guard.Initialize(guardPolicies);
                return guard;
            });
        }


        public static GuardBlock operator +(GuardBlock left, GuardBlock right)
        {
            var result = (GuardBlock)left._provider.GetService(typeof(GuardBlock));
            result._guards.AddRange(left._guards);
            result._guards.AddRange(right._guards);
            return result;
        }


        public static implicit operator bool(GuardBlock guard)
        {
            return guard.Evaluate(System.Threading.Thread.CurrentPrincipal as ClaimsPrincipal);
        }


        public IServiceProvider Provider => _provider;

        private volatile object _lock = new object();
        private readonly IServiceProvider _provider;
        private readonly List<IGuard> _guards;
        private readonly List<Func<GuardBlock, IGuard>> _functionGuards;

    }



}
