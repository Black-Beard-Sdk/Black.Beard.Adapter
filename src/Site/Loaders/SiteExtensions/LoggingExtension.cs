using Bb.ComponentModel.Factories;
using Bb.ComponentModel.Loaders;

namespace Site.Loaders.SiteExtensions
{


    public static class LoggingExtension
    {


        public static WebApplicationBuilder ConfigureTrace(this WebApplicationBuilder builder)
        {

            builder.WebHost.ConfigureLogging(logging =>
            {
                logging.Initialize(new LocalServiceProvider(builder.Services.BuildServiceProvider()));
            });

            return builder;
        
        }

    }


}
