using Bb.Commands;
using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;

namespace Bb
{

    [ExposeClass(ConstantsCore.Service, ExposedType = typeof(IFocusedService<ITransactionManager>), LifeCycle = IocScopeEnum.Scoped)]
    public class TransactionManagerFocusService : FocusServiceBase<ITransactionManager>
    {

    }

}
