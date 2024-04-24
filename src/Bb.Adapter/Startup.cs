using System.Runtime.InteropServices;
using Bb.Servers.Web.Models;
using Bb.Servers.Web;
using MudBlazor.Services;
using Bb.ComponentModel.Factories;
using Microsoft.AspNetCore.Components;
using Bb.Adapter;
using Bb.Wizards;


namespace Bb.MockService
{


    /// <summary>
    /// Startup class par parameter
    /// </summary>
    public class Startup : ServiceRunnerStartup
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
            : base(configuration)
        {

            ObjectCreatorByIoc.SetInjectionAttribute<InjectAttribute>();

        }

        public override void ConfigureServices(IServiceCollection services)
        {

            string root = string.Empty;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                root = "c:\\".Combine("tmp", "mocks");
            else
                root = Path.DirectorySeparatorChar + "tmp".Combine("mocks");

            GlobalConfiguration.CurrentDirectoryToWriteGenerators = root.Combine("contracts");
            GlobalConfiguration.DirectoryToTrace = root.Combine("logs");
            GlobalConfiguration.CurrentDirectoryToWriteGenerators.CreateFolderIfNotExists();
            GlobalConfiguration.DirectoryToTrace.CreateFolderIfNotExists();
            GlobalConfiguration.DirectoryToTrace += Path.DirectorySeparatorChar;

            base.ConfigureServices(services);


            //RegisterServicesPolicies(services);

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddMudServices();

        }

        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public override void ConfigureApplication(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {

            var logger = loggerFactory.CreateLogger<Startup>();

            logger.LogInformation("setting directory data stores  : " + GlobalConfiguration.CurrentDirectoryToWriteGenerators);
            logger.LogInformation("setting directory output logs : " + GlobalConfiguration.DirectoryToTrace);

            base.ConfigureApplication(app, env, loggerFactory);

            app
                .UseHttpsRedirection()
                .UseStaticFiles()
                .UseRouting()
                ;

            var web = app as WebApplication;
            web.MapBlazorHub();
            web.MapFallbackToPage("/_Host");


            
            //web.MapRazorComponents<Startup>()
            //    .AddInteractiveServerRenderMode()
                
            //    //.AddAdditionalAssemblies(new [] { typeof(WizardPage).Assembly })
            //    ;

        }


        public override void RegisterTypes(IServiceCollection services)
        {
            base.RegisterTypes(services);
        }

    }



}
