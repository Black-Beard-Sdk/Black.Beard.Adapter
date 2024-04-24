using Bb.MockService;
using Bb.Servers.Web;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;

namespace Bb.Adapter
{

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

}