using Bb.Adapter.Data;
using Bb.MockService;
using Bb.Modules;
using Bb.Modules.Etl;
using Bb.Servers.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Net;
using System.Reflection;

namespace Bb.Adapter
{
    public class Program
    {


        /// <summary>
        /// Main entry point of the application.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var yy = typeof(ModuleSpecifications);
            var zz = typeof(ModuleDataFlow);

            var service = GetService(args);
            service.Run();
        }

        public static ServiceAdapterRunner GetService(string[] args)
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            Directory.SetCurrentDirectory(Path.GetDirectoryName(currentAssembly.Location));
            var result = new ServiceAdapterRunner(args);
            return result;
        }



        public static void Main2(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddSingleton<WeatherForecastService>();
            builder.Services.AddFluentUIComponents();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");


            app.Run();
        }
    }






    public class ServiceAdapterRunner : ServiceRunner<Startup>
    {

        public ServiceAdapterRunner(string[] args)
            : base(args)
        {
            this.IsWebApplication = true;
        }


        protected override void ConfigureApplication(WebApplication wbuilder, IWebHostEnvironment environment, ILoggerFactory? loggerFactory)
        {
            base.ConfigureApplication(wbuilder, environment, loggerFactory);
            StaticWebAssetsLoader.UseStaticWebAssets(wbuilder.Environment, wbuilder.Configuration);
        }



    }

    public static class ServiceRunnerBaseExtensions
    {

        /// <summary>
        /// Runs asynchronous service
        /// </summary>
        /// <param name="waitRunning">if set to <c>true</c> [wait service running].</param>
        /// <returns></returns
        public static T Start<T>(this T self, bool waitRunning = true)
            where T : ServiceRunnerBase
        {

            self.RunAsync();

            if (waitRunning)
                while (self.Status != ServiceRunnerStatus.Running)
                {
                    Thread.Sleep(0);
                }

            return self;

        }

        /// <summary>
        /// wait the predicate is true before continue
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T Wait<T>(this T self, Func<ServiceRunnerBase, bool> predicate)
            where T : ServiceRunnerBase
        {

            while (predicate(self))
            {
                Thread.Sleep(0);
            }

            return self;

        }

        /// <summary>
        /// Runs asynchronous service
        /// </summary>
        /// <param name="waitRunning">if set to <c>true</c> [wait service running].</param>
        /// <returns></returns
        public static T RunAsync<T>(this T self)
            where T : ServiceRunnerBase
        {

            if (self.Status != ServiceRunnerStatus.Stopped)
                throw new InvalidOperationException("Service is already running");

            Task.Run(() => self.Run(), self.CancellationToken);

            return self;

        }

    }

}