using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Loaders;
using Bb.UIComponents;
using ICSharpCode.Decompiler.Util;
using Microsoft.Extensions.DependencyInjection;

namespace Bb.Loaders
{


    [ExposeClass(ConstantsCore.Initialization, LifeCycle = IocScopeEnum.Transiant)]
    public class ModuleInitializeWebApplication : IApplicationBuilderInitializer<WebApplication>
    {

        public ModuleInitializeWebApplication()
        {
            
        }

        public string FriendlyName => GetType().Name;

        public int OrderPriority => 0;

        public bool Executed { get; set; }

        public Type Type => typeof(WebApplication);

        public bool CanExecute(WebApplication builder, InitializationLoader<WebApplication> initializer)
        {
            return true;
        }
                
        public void Execute(WebApplication builder)
        {

            var uiService = builder.Services.GetService<UIService>(); 
            var loader = new InjectionLoader<UIService>(builder.Services, UIConstants.LeftMenu);
            loader.LoadModules(c =>
            {

            }).Execute(uiService);

        }

    }

}


