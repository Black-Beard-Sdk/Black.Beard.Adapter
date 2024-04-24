using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Loaders;
using MudBlazor.Services;
using NLog;
using Site.Data;
using Site.SiteExtensions;

namespace Site.Loaders
{

    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IApplicationBuilderInitializer<WebApplicationBuilder>), LifeCycle = IocScopeEnum.Transiant)]
    public class WebApplicationBuilderInitializer : ApplicationInitializerBase<WebApplicationBuilder>
    {


        public WebApplicationBuilderInitializer()
        {
            Logger = LogManager.GetLogger(nameof(WebApplicationBuilderInitializer));
        }

        public override void Execute(WebApplicationBuilder builder)
        {

            var services = builder.Services;

            builder.WebHost.ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.LoadConfigurationFile(Logger, hostingContext)
                      .ConfigureApplication(Logger, hostingContext);
            });


            // Auto discover all types with attribute [ExposeClass] for register in ioc.
            services.UseTypeExposedByAttribute(builder.Configuration, ConstantsCore.Configuration, c =>
            {
                services.BindConfiguration(c, builder.Configuration);
            })
                    .UseTypeExposedByAttribute(builder.Configuration, ConstantsCore.Model)
                    .UseTypeExposedByAttribute(builder.Configuration, ConstantsCore.Service);


            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddSingleton<WeatherForecastService>();
            builder.Services.AddMudServices();


        }

        public Logger Logger { get; set; }



    }

}
