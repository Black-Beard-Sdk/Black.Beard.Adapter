using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;

namespace Bb
{
    [ExposeClass(ConstantsCore.Service, ExposedType = typeof(IFocusedService), LifeCycle = IocScopeEnum.Scoped)]
    public class FocusService : IFocusedService
    {

        public event EventHandler<EventArgs>? FocusChanged;

        public void FocusChange(object sender)
        {

            if (_lastFocused != sender)
                lock (_lock)
                    if (_lastFocused != sender)
                    {
                        _lastFocused = sender;
                        FocusChanged?.Invoke(sender, EventArgs.Empty);
                    }
        }


        private object _lastFocused;
        private volatile object _lock = new object();
    }


}
