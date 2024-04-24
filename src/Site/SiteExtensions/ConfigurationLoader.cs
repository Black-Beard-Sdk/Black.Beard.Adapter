using Bb;
using Bb.ComponentModel.Factories;
using Bb.ComponentModel.Loaders;
using Bb.ComponentModel;
using System.Reflection;
using NLog;

namespace Site.SiteExtensions
{

    public static class ConfigurationLoader
    {


        public static IConfigurationBuilder ConfigureApplication(this IConfigurationBuilder config, Logger logger, WebHostBuilderContext hostingContext)
        {

            var provider = new LocalServiceProvider()
                .Add(typeof(WebHostBuilderContext), hostingContext)
                .Add(typeof(Logger), logger)
                ;

            var loader = new InjectionLoader<IConfigurationBuilder>(ConstantsCore.Initialization, provider)
                .LoadModules()
                .Execute(config);

            return config;

        }

        public static IConfigurationBuilder LoadConfigurationFile(this IConfigurationBuilder config, Logger logger, WebHostBuilderContext hostingContext)
        {

            var env = hostingContext.HostingEnvironment;
            var dirs = new List<DirectoryInfo>() { env.ContentRootPath.AsDirectory() };

            var c = env.ContentRootPath.Combine("Configs").AsDirectory();
            if (c.Exists)
                dirs.Add(c);

            foreach (var dir in dirs)
            {
                foreach (var file in dir.GetFiles($"*.{env.EnvironmentName}.json"))
                {
                    config.AddJsonFile(file.FullName, optional: false, reloadOnChange: false);
                    logger.Debug($"configuration file {file.FullName} is loaded.");
                }
            }


            config.AddUserSecrets(Assembly.GetEntryAssembly())
                  .AddEnvironmentVariables()
            //.AddCommandLine(_args)
            ;

            return config;

        }



    }
}
