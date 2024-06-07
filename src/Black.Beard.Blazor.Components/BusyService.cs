using Bb.ComponentModel.Attributes;
using Bb.ComponentModel;
using Microsoft.AspNetCore.Components;
using System.Timers;
using System.Threading;

namespace Bb
{

    [ExposeClass(ConstantsCore.Service, ExposedType = typeof(IBusyService), LifeCycle = IocScopeEnum.Scoped)]
    public class BusyService : IBusyService
    {


        public BusySession IsBusyFor(object instance, string title, Action<BusySession> action)
        {
            var session = new BusySession(this, title, instance, action);
            Task.Run(() => Update(session));
            return session;
        }


        public async Task Update(BusySession session)
        {
            BusyChanged?.Invoke(this, new BusyEventArgs(session));
            await Task.Yield();
        }


        public event EventHandler<BusyEventArgs> BusyChanged;

    }

    public enum BusyEnum
    {
        New,
        Started,
        Running,
        Completed,
    }


}
