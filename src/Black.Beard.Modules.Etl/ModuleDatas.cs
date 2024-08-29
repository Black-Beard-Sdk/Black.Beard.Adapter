using Bb.Addons;
using Bb.ComponentModel.Attributes;

namespace Bb.Modules
{

    [ExposeClass(ComponentModel.ConstantsCore.Plugin, ExposedType = typeof(AddOnLibrary), LifeCycle = IocScopeEnum.Singleton)]
    public class ModuleDatas : AddOnLibrary
    {

        public ModuleDatas() :
            base(
                new Guid(Filter),
                "Datas managements",
                "Datas module")
        {

        }

        public const string Filter = "C9119B69-5DD9-45D2-A28A-617D6CB9D7F9";

    }


}
