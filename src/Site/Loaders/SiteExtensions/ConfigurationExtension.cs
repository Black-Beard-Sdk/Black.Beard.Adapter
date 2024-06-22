using Bb;
using Bb.ComponentModel.Factories;
using Bb.ComponentModel.Loaders;
using Bb.ComponentModel;
using System.Reflection;
using NLog;

namespace Site.Loaders.SiteExtensions
{

    public static class ConfigurationExtension
    {

        static ConfigurationExtension()
        {
            _logger = LogManager.GetLogger(nameof(ConfigurationExtension));
        }

        public static void SetConfiguration(this ConfigureWebHostBuilder builder)
        {

            var paths = new List<string> 
            {
                "Configs"
            };

            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.LoadConfigurationFile(hostingContext, paths.ToArray(), null, null)
                      .ConfigureApplication(hostingContext);
            });

        }


        public static IConfigurationBuilder ConfigureApplication(this IConfigurationBuilder config, WebHostBuilderContext hostingContext)
        {

            var provider = new LocalServiceProvider()
                .Add(typeof(WebHostBuilderContext), hostingContext)
                .Add(typeof(Logger), _logger)
                ;

            var loader = new InjectionLoader<IConfigurationBuilder>(ConstantsCore.Initialization, provider)
                .LoadModules()
                .Execute(config);

            return config;

        }


        public static IConfigurationBuilder LoadConfigurationFile(this IConfigurationBuilder config,
            WebHostBuilderContext hostingContext,
            string[] paths,
            string pattern = null,
            Func<FileInfo, bool> filter = null)
        {

            var env = hostingContext.HostingEnvironment;
            var dirs = new List<DirectoryInfo>()
            {
                env.ContentRootPath.AsDirectory()
            };

            if (paths != null)
                foreach (var path in paths)
                {

                    DirectoryInfo c;

                    if (path.FilePathIsAbsolute())
                        c = path.AsDirectory();
                    else
                    {
                        var p = env.ContentRootPath;
                        if (!string.IsNullOrEmpty(path))
                            p = p.Combine(path);
                        c = p.AsDirectory();
                    }

                    if (c != null && c.Exists)
                        dirs.Add(c);

                }

            if (string.IsNullOrEmpty(pattern))
                pattern = $"*.{env.EnvironmentName}.json";

            foreach (var dir in dirs)
            {
                foreach (var file in dir.GetFiles(pattern))
                    if (filter == null || filter(file))
                    {
                        config.AddJsonFile(file.FullName, optional: false, reloadOnChange: false);
                        _logger.Debug($"configuration file {file.FullName} is loaded.");
                    }
            }


            config.AddUserSecrets(Assembly.GetEntryAssembly())
                  .AddEnvironmentVariables()
            //.AddCommandLine(_args)
            ;

            return config;

        }

        public static bool FilePathIsAbsolute(this string path)
        {

            if (!string.IsNullOrEmpty(path))
            {
                var f = path.AsFile();
                if (f.FullName == path)
                    return true;
            }

            return false;

        }

        public static bool DirectoryPathIsAbsolute(this string path)
        {

            if (!string.IsNullOrEmpty(path))
            {
                var f = path.AsDirectory();
                if (f.FullName == path)
                    return true;
            }

            return false;

        }

        private static readonly Logger _logger;

    }
}
