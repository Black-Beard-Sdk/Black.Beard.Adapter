using NLog.Web;
using Site.Pages.Pages.Authentication;

namespace Site.Loaders.SiteExtensions
{


    public static class LoggingExtension
    {


        public static WebApplicationBuilder ConfigureTrace(this WebApplicationBuilder builder)
        {
            builder.WebHost.SetLogging(new NLogAspNetCoreOptions() { IncludeScopes = true, IncludeActivityIdsWithBeginScope = true });
            return builder;
        }


        public static void SetLogging(this ConfigureWebHostBuilder builder, NLogAspNetCoreOptions options)
        {

            var Logger = Loggers.InitializeLogger();

            builder.ConfigureLogging(l =>
            {
                l.ClearProviders();
                l.SetMinimumLevel(LogLevel.Trace);
            })
            .UseNLog(options);

        }

    }


}
