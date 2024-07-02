using Bb.ComponentModel.Factories;
using Bb.ComponentModel.Loaders;
using Bb.ComponentModel;
using NLog.Web;
using Site.Pages.Pages.Authentication;

namespace Site.Loaders.SiteExtensions
{


    public static class LoggingExtension
    {


        public static WebApplicationBuilder ConfigureTrace(this WebApplicationBuilder builder)
        {

            builder.WebHost.ConfigureLogging(logging =>
            {

                var provider = new LocalServiceProvider(builder.Services.BuildServiceProvider());

                var loader = new InjectionLoader<ILoggingBuilder>(ConstantsCore.Initialization, provider)
                    .LoadModules()
                    .Execute(logging);

            });

            return builder;
        
        }

    }


}
