﻿using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Loaders;
using Microsoft.Fast.Components.FluentUI;
using MudBlazor.Services;
using NLog;
using NLog.Web;
using Site.Data;
using Site.Loaders.SiteExtensions;

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

            builder.SetIoc();

            var services = builder.Services;
            // Add services to the container.
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();
            services.AddMudServices();
            services.AddFluentUIComponents();

        }



        public Logger Logger { get; set; }



    }


}
