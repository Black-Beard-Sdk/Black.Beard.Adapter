using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Microsoft.AspNetCore.Builder;
using NLog.Web;
using Bb.ComponentModel.Loaders;

namespace Bb.Logging.NLog
{
    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IApplicationBuilderInitializer<WebApplicationBuilder>), LifeCycle = IocScopeEnum.Transiant)]
    public class WebApplicationBuilderInitializerNLog : ApplicationInitializerBase<WebApplicationBuilder>
    {

        public WebApplicationBuilderInitializerNLog()
        {
        }

        public override bool CanExecute(WebApplicationBuilder builder, InitializationLoader<WebApplicationBuilder> initializer)
        {
            return base.CanExecute(builder, initializer);
        }

        public override void Execute(WebApplicationBuilder builder)
        {
            var options = new NLogAspNetCoreOptions() { IncludeScopes = true, IncludeActivityIdsWithBeginScope = true };
            builder.WebHost.UseNLog(options);
        }

    }

}
