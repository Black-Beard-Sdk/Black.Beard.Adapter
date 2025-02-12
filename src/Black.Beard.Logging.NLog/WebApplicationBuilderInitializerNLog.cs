using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Microsoft.AspNetCore.Builder;
using NLog.Web;
using Bb.ComponentModel.Loaders;

namespace Bb.Logging.NLog
{
    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IInjectBuilder<WebApplicationBuilder>), LifeCycle = IocScopeEnum.Transiant)]
    public class WebApplicationBuilderInitializerNLog : InjectBuilder<WebApplicationBuilder>
    {      

        public override object Execute(WebApplicationBuilder builder)
        {
            var options = new NLogAspNetCoreOptions() 
            {
                IncludeScopes = true, 
                IncludeActivityIdsWithBeginScope = true,
                
            };
            builder.WebHost.UseNLog(options);
            return null;    
        }


    }

}
