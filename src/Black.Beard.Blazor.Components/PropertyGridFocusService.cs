using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.PropertyGrid;

namespace Bb
{

    [ExposeClass(ConstantsCore.Service, ExposedType = typeof(IFocusedService<PropertyGridView>), LifeCycle = IocScopeEnum.Scoped)]
    public class PropertyGridFocusService : FocusServiceBase<PropertyGridView>
    {

    }

}
