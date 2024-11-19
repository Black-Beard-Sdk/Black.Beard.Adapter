using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using System.Reflection.Metadata;
using System.Transactions;

namespace Bb
{


    [ExposeClass(ConstantsCore.Service, ExposedType = typeof(IRefreshService), LifeCycle = IocScopeEnum.Scoped)]
    public class RefreshService : IRefreshService
    {

        public event EventHandler<RefreshEventArgs>? RefreshRequested;

        public void CallRequestRefresh(object sender, params object[] toRefresh)
        {
            RefreshRequested?.Invoke(sender, new RefreshEventArgs(toRefresh));
        }

    }

}
