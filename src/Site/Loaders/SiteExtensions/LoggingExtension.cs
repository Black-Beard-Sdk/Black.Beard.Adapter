using NLog.Web;

namespace Site.Loaders.SiteExtensions
{
    public static class LoggingExtension
    {

        public static void SetLogging(this ConfigureWebHostBuilder builder, NLogAspNetCoreOptions options)
        {

            builder.ConfigureLogging(l =>
            {
                l.ClearProviders();
            })
            .UseNLog(options);

        }

    }
}
