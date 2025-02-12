using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Microsoft.Fast.Components.FluentUI;
using MudBlazor.Services;
using NLog;
using Site.Loaders.SiteExtensions;

namespace Site.Loaders
{

    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IInjectBuilder<WebApplicationBuilder>), LifeCycle = IocScopeEnum.Transiant)]
    public class WebApplicationBuilderInitializer : InjectBuilder<WebApplicationBuilder>
    {

        public WebApplicationBuilderInitializer()
        {
            Logger = LogManager.GetLogger(nameof(WebApplicationBuilderInitializer));
        }

        public override object Execute(WebApplicationBuilder builder)
        {

            builder.SetAllIoc((c, d) => true);

            var services = builder.Services;
            // Add services to the container.
            services.AddRazorPages();
            services.AddServerSideBlazor();
            //services.AddSingleton<WeatherForecastService>();
            services.AddMudServices();
            services.AddFluentUIComponents();

            return null;

        }

        public Logger Logger { get; set; }

    }


}
