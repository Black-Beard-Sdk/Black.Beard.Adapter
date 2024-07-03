using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Microsoft.AspNetCore.Builder;
using NLog.Web;
using Bb.ComponentModel.Loaders;

namespace Bb.Logging.NLog
{
    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IInjectBuilder<WebApplicationBuilder>), LifeCycle = IocScopeEnum.Transiant)]
    public class WebApplicationBuilderInitializerNLog : IInjectBuilder<WebApplicationBuilder>
    {

        public WebApplicationBuilderInitializerNLog()
        {
        }

        public string FriendlyName => typeof(WebApplicationBuilderInitializerNLog).Name;

        public Type Type => typeof(WebApplicationBuilder);

        public bool CanExecute(WebApplicationBuilder builder)
        {
            return true;
        }

        public bool CanExecute(object context)
        {
            return CanExecute((WebApplicationBuilder)context);
        }

        public object Execute(WebApplicationBuilder builder)
        {
            var options = new NLogAspNetCoreOptions() { IncludeScopes = true, IncludeActivityIdsWithBeginScope = true };
            builder.WebHost.UseNLog(options);
            return null;    
        }

        public object Execute(object context)
        {
            return Execute((WebApplicationBuilder)context);
        }

    }

}
