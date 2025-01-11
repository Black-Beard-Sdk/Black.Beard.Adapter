
using Bb;
using Bb.ComponentModel.Factories;
using Bb.ComponentModel.Loaders;
using Bb.Logging.NLog;
using Microsoft.AspNetCore.Components;
using Site.Services;

namespace Site.Loaders.SiteExtensions
{

    public static class _Initializer
    {

        public static void PrepareInitialization()
        {

            string[] configPaths = ConfigurationFolder.GetPaths();

            var configuration = new ConfigurationBuilder().LoadConfiguration()
             .SetBasePath(AppContext.BaseDirectory)
             .Build();

            var SchemasPaths = configPaths.First() + "\\schemas";
            var idTemplate = "http://Black.Beard.com/schema/{0}";

            SchemaGenerator.Initialize(SchemasPaths, idTemplate);
            ObjectCreatorByIoc.SetInjectionAttribute<InjectAttribute>();
            Assemblies.Load("ExposedAssemblyRepositories.json", configPaths); // Ensure all required assemblies are loaded

            Initializer.Initialize(c =>
            {

                c.Add(typeof(IConfiguration), configuration);

                c.OnInitialization = (i) =>
                {

                };

            }, Environment.GetCommandLineArgs());
        
        }


    }

}
