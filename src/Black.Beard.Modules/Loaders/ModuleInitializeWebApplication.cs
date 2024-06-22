using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Loaders;
using Bb.UIComponents;

namespace Bb.Loaders
{

    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IApplicationBuilderInitializer<WebApplication>), LifeCycle = IocScopeEnum.Transiant)]
    public class ModuleInitializeWebApplication : ApplicationInitializerBase<WebApplication>
    {

        public override void Execute(WebApplication builder)
        {

            using (var scope = builder.Services.CreateScope())
            {

                var uiService = scope.ServiceProvider.GetService<MenuService>();
                if (uiService != null)
                {
                    var loader = new InjectionLoader<MenuService>(UIConstants.LeftMenu, scope.ServiceProvider)
                        .LoadModules()
                        .Execute(uiService);
                }

            }
        }

    }

}


