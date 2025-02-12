
using Bb;
using Bb.Configuration;
using Bb.ComponentModel.Factories;
using Bb.ComponentModel.Loaders;
using Site.Services;
using Bb.ComponentModel.Attributes;
using Bb.ComponentModel;
using System.Diagnostics;

namespace Site.Loaders.SiteExtensions
{

    public static class _Initializer
    {

        public static IServiceProvider PrepareInitialization(string typeToExcludeInProviderService)
        {

            Stopwatch stopwatch = Stopwatch.StartNew();

            var args = Environment.GetCommandLineArgs();
            string[] configPaths = ConfigurationFolder.GetPaths();
            var SchemasPaths = configPaths.First() + "\\schemas";
            var idTemplate = "http://Black.Beard.com/schema/{0}";

            SchemaGenerator.Initialize(SchemasPaths, idTemplate);
            ObjectCreatorByIoc.SetInjectionAttribute<InjectAttribute>();

            // Ensure all required assemblies are loaded
            new AddonsResolver()
                .With(Assemblies.Resolve("ExposedAssemblyRepositories.json", configPaths))
                .WhereAssemblyReference(typeof(ExposeClassAttribute))
                .SearchAssemblies()
                .EnsureIsLoaded()
                ;


            var strings = new HashSet<string>(typeToExcludeInProviderService.Split(';').Select(c => c.Trim()));
            // Create a new web application
            var builder = WebApplication
                .CreateBuilder(args)
                .LoadConfiguration()
                .SetAllIoc((type, context) =>
                {

                    if (strings.Contains(type.FullName))
                        return false;

                    return true;
                });

            var preApp = builder.Build();

            Initializer.Initialize(c =>
            {
                c.With(preApp.Services)
                 .OnInitialization = (i) =>
                 {

                 };

            }, args);

            new AddonsResolver()
                .WhereAssemblyReference(typeof(ExposeClassAttribute))
                .SearchAssemblies()
                .EnsureIsLoaded()
                ;

            stopwatch.Stop();

            Trace.WriteLine($"Initialization in {stopwatch.Elapsed.ToString("mm\\:ss")} milliseconds");

            return preApp.Services;

        }




    }

}
