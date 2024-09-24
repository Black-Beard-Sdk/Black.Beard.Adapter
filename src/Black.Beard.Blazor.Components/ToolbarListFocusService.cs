using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.Toolbars;

namespace Bb
{


    [ExposeClass(ConstantsCore.Service, ExposedType = typeof(IFocusedService<ToolbarList>), LifeCycle = IocScopeEnum.Scoped)]
    public class ToolbarListFocusService : FocusServiceBase<ToolbarList>
    {
             

    }


}
