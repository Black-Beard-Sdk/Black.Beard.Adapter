using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Microsoft.Fast.Components.FluentUI;
using MudBlazor.Services;
using NLog;
using Site.Loaders.SiteExtensions;

namespace Site.Loaders
{

    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IInjectBuilder<WebApplicationBuilder>), LifeCycle = IocScopeEnum.Transiant)]
    public class WebApplicationBuilderInitializer : IInjectBuilder<WebApplicationBuilder>
    {

        public WebApplicationBuilderInitializer()
        {
            Logger = LogManager.GetLogger(nameof(WebApplicationBuilderInitializer));
        }

        public object Execute(WebApplicationBuilder builder)
        {            

            builder.SetIoc();

            var services = builder.Services;
            // Add services to the container.
            services.AddRazorPages();
            services.AddServerSideBlazor();
            //services.AddSingleton<WeatherForecastService>();
            services.AddMudServices();
            services.AddFluentUIComponents();

            return null;

        }

        public bool CanExecute(WebApplicationBuilder context)
        {
            return true;
        }

        public object Execute(object context)
        {
            return Execute((WebApplicationBuilder)context);
        }

        public bool CanExecute(object context)
        {
            return CanExecute((WebApplicationBuilder)context);
        }

        public Logger Logger { get; set; }

        public string FriendlyName => typeof(WebApplicationBuilderInitializer).Name;

        public Type Type => typeof(WebApplicationBuilder);
    
    }


}
