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
                logging.Initialize(builder.Services.BuildServiceProvider());
            });

            return builder;

        }

    }


}
