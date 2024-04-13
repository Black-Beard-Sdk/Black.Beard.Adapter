using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Loaders;

namespace Bb.Loaders
{


    [ExposeClass(ConstantsCore.Initialization, LifeCycle = IocScopeEnum.Transiant)]
    public class EtlInitializeWebApplicationBuilder : IApplicationBuilderInitializer<WebApplicationBuilder>
    {

        public EtlInitializeWebApplicationBuilder()
        {


        }

        public string FriendlyName => GetType().Name;

        public int OrderPriority => 1;

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
