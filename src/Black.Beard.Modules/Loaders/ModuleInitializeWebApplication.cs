using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Loaders;
using Bb.UIComponents;

namespace Bb.Loaders
{

    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IInjectBuilder<WebApplication>), LifeCycle = IocScopeEnum.Transiant)]
    public class ModuleInitializeWebApplication : IInjectBuilder<WebApplication>
    {
        public string FriendlyName => typeof(ModuleInitializeWebApplication).Name;

        public Type Type => typeof(WebApplication);

        public bool CanExecute(WebApplication context)
        {
            return true;
        }

        public bool CanExecute(object context)
        {
            return CanExecute((WebApplication)context); 
        }

        public object Execute(WebApplication builder)
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

            return null;

        }

        public object Execute(object context)
        {
            return Execute((WebApplication)context);
        }

    }

}


