using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Loaders;

namespace Bb.Loaders
{



    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IApplicationBuilderInitializer<WebApplicationBuilder>), LifeCycle = IocScopeEnum.Transiant)]
    public class ModuleInitializeWebApplicationBuilder : IApplicationBuilderInitializer<WebApplicationBuilder>
    {

        public ModuleInitializeWebApplicationBuilder()
        {


        }

        public string FriendlyName => GetType().Name;

        public int OrderPriority => 0;

        public bool Executed { get; set; }

        public Type Type => typeof(WebApplicationBuilder);

        public bool CanExecute(WebApplicationBuilder builder, InitializationLoader<WebApplicationBuilder> initializer)
        {
            return true;
        }
                
        public void Execute(WebApplicationBuilder builder)
        {
         
        }

    }

}
