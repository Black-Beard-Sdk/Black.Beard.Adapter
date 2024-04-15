using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;

namespace Bb.UIComponents.Guards
{


    [ExposeClass(UIConstants.Service, ExposedType = typeof(GuardMenuProvider), LifeCycle = IocScopeEnum.Singleton)]
    public class GuardMenuProvider
    {

        public GuardMenuProvider(IServiceProvider service)
        {

            _service = service;

        }

        internal IGuardMenu Get(Type type)
        {
            var result = (IGuardMenu)_service.GetService(type);
            result.Inititalize(this);
            return result;
        }

        private IServiceProvider _service;

        public bool Get(GuardContainer guard)
        {
            return guard.Evaluate(this);
        }

    }


}
